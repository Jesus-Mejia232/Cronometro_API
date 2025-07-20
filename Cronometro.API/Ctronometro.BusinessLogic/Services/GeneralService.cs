using Cronometro.BusinessLogic;
using Cronometro.DataAccess.Repositories;
using Cronometro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctronometro.BusinessLogic.Services
{
    public class GeneralService
    {
        private readonly CronometroReposotory _cronometroReposotory;


        public GeneralService(CronometroReposotory cronometroReposotory)
        {
            _cronometroReposotory = cronometroReposotory;
        }

        public ServiceResult IniciarCronometro(tbProyectosTiempos item)
        {
            var result = new ServiceResult();
            try
            {
                var insert = _cronometroReposotory.Insert(item);
                return result.Ok(insert);
            }
            catch (Exception ex)
            {
                return result.Error(ex.Message);
            }
        }

        public ServiceResult FinalizarCronometro(int registroID, TimeSpan horaFin)
        {
            var result = new ServiceResult();
            try
            {
                var update = _cronometroReposotory.Finalizar(registroID, horaFin);
                return result.Ok(update);
            }
            catch (Exception ex)
            {
                return result.Error(ex.Message);
            }
        }


    }
}
