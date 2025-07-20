using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cronometro.DataAccess.Repositories
{
    public class ScriptDataBase
    {
        public static string Cronmetro_Listar = "[].[]";
        public static string Cronmetro_Insertar = "[Prod].[SP_Proyectos_Tiempos_Iniciar]";
    }
}
