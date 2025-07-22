using Cronometro.DataAccess.Context;
using Cronometro.API_.Extensions;
using Microsoft.EntityFrameworkCore;
using static Ctronometro.BusinessLogic.Services.GeneralService;
using Cronometro.API_.Controllers;
using Ctronometro.BusinessLogic;
using Microsoft.OpenApi.Models; // <-- Aseg�rese de tener este usi
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CronometroCONN");

// Database Context
builder.Services.AddDbContext<db_abbf9d_cronometroapiContext>(option =>
    option.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();

// Repositorios y l�gica de negocio
builder.Services.DataAccess(connectionString);
builder.Services.BusinessLogic();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfileExtensions));

// Controladores con soporte para TimeOnly
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// WebSocket Manager
builder.Services.AddSingleton<WebSocketConnectionManager>();

// CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                "http://localhost:8080", 
                                "http://127.0.0.1:5500",
                                "https://localhost:44346") // frontend local
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
}); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cron�metro API",
        Version = "v1",
        Description = "API para control de tiempos por proyecto usando cron�metro.",
        Contact = new OpenApiContact
        {
            Name = "Soporte T�cnico",
            Email = "soporte@example.com"
        }
    });
});

var app = builder.Build();

// ===== Middleware Pipeline =====

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar WebSocket ANTES de routing y middleware personalizados
app.UseWebSockets();

// Middleware personalizado para WebSocket
app.UseMiddleware<WebSocketMiddleware>();

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
