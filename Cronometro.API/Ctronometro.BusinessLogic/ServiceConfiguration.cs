using Cronometro.DataAccess;
using Cronometro.DataAccess.Context;
using Cronometro.DataAccess.Repositories;
using Ctronometro.BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctronometro.BusinessLogic
{
    public static class ServiceConfiguration
    {
        public static void DataAccess(this IServiceCollection services, string connectionString)
        {
            //Acá se agregan todos los repositorios
            services.AddScoped<CronometroReposotory>();


            CronometroAPI_DBContext.BuildConnectionString(connectionString);
        }

        public static void BusinessLogic(this IServiceCollection services)
        {
            // Acá van los servicios
            services.AddScoped<GeneralService>();
        }

    }
}
