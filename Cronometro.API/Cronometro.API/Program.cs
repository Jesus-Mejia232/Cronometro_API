using Cronometro.DataAccess.Context;
using Cronometro.API_.Extensions;
using Microsoft.EntityFrameworkCore;
using Ctronometro.BusinessLogic.Services;
using Cronometro.DataAccess.Repositories;
using Cronometro.BusinessLogic;
using Cronometro.API_.Extensions;
using Ctronometro.BusinessLogic;
using Cronometro.API_.Controllers;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CronometroCONN");

// Database Context
builder.Services.AddDbContext<db_abbf9d_cronometroapiContext>(option => option.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();

// Register repositories and services
builder.Services.DataAccess(connectionString);
builder.Services.BusinessLogic();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfileExtensions));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
