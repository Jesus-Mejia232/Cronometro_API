using Cronometro.Entities.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cronometro.DataAccess.Repositories
{
    public class CronometroReposotory : IRepository<tbProyectosTiempos>
    {
        public tbProyectosTiempos Find(int? id)
        {
            throw new NotImplementedException();
        }

        public RequestStatus Insert(tbProyectosTiempos item)
        {
            if (item == null)
            {
                return new RequestStatus { code_Status = 0, message_Status = "Los datos llegaron vacios o datos erroneos." };
            }
            var parameter = new DynamicParameters();
            parameter.Add("@ProyectoCode", item.ProyectoCode, System.Data.DbType.String, System.Data.ParameterDirection.Input);
            parameter.Add("@Descripcion", item.Descripcion, System.Data.DbType.String, System.Data.ParameterDirection.Input);
            parameter.Add("@HoraInicio", item.HoraInicio, System.Data.DbType.Time, System.Data.ParameterDirection.Input);
            //parameter.Add("@HoraFin", item.HoraFin, System.Data.DbType.Time, System.Data.ParameterDirection.Input);
            //parameter.Add("@TotalHoras", item.TotalHoras, System.Data.DbType.Time, System.Data.ParameterDirection.Input);
            parameter.Add("@FechaWork", item.FechaWork, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameter.Add("@FechaSystema", item.FechaSystema, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            parameter.Add("@NombreUsuario", item.Nombreusuario, System.Data.DbType.String, System.Data.ParameterDirection.Input);
            parameter.Add("@Referencia", item.Referencia, System.Data.DbType.String, System.Data.ParameterDirection.Input);

            try
            {
                using var db = new SqlConnection(CronometroAPI_DBContext.ConnectionString);
                var result = db.QueryFirstOrDefault<RequestStatus>(ScriptDataBase.Cronmetro_Insertar, parameter, commandType: System.Data.CommandType.StoredProcedure);
                if (result == null)
                {
                    return new RequestStatus { code_Status = 0, message_Status = "Error desconocido" };
                }
                return result;
            }
            catch (Exception ex)
            {
                return new RequestStatus { code_Status = 0, message_Status = $"Error inesperado: {ex.Message}" };
            }

        }



        public RequestStatus Finalizar(int registroID, TimeSpan horaFin)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@RegistroID", registroID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            parameter.Add("@HoraFin", horaFin, System.Data.DbType.Time, System.Data.ParameterDirection.Input);

            try
            {
                using var db = new SqlConnection(CronometroAPI_DBContext.ConnectionString);
                var result = db.QueryFirstOrDefault<RequestStatus>(
                    ScriptDataBase.Cronometro_Finalizar,
                    parameter,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result ?? new RequestStatus
                {
                    code_Status = 0,
                    message_Status = "Error desconocido"
                };
            }
            catch (Exception ex)
            {
                return new RequestStatus
                {
                    code_Status = 0,
                    message_Status = $"Error inesperado: {ex.Message}"
                };
            }
        }

        public IEnumerable<tbProyectosTiempos> List()
        {
            throw new NotImplementedException();
        }

        public RequestStatus Update(tbProyectosTiempos item)
        {
            throw new NotImplementedException();
        }
    }
}
