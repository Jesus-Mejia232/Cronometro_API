using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cronometro.DataAccess.Repositories
{
    public class RequestStatus
    {
        public int code_Status { get; set; }
        public string message_Status { get; set; }
        public int? RegistroID { get; set; } // NUEVO
    }
}
