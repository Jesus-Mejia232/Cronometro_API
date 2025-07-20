using AutoMapper;
using Cronometro.API_.Models;
using Cronometro.DataAccess.Context;
using Cronometro.Entities.Entities;
using Ctronometro.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cronometro.API_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ApiController]
    //[Route("[controller]")]
    //[ApiKey]
    public class CronometroController : Controller
    {
        public readonly GeneralService _generalService;
        public readonly IMapper _mapper;

        public CronometroController(GeneralService generalService, IMapper mapper)
        {
            _generalService = generalService;
            _mapper = mapper;
        }



        [HttpPost("Insertar")]
        public IActionResult IniciarCronometro([FromBody] ProyectoTiempoParcialDto item)
        {
            var mapped = _mapper.Map<tbProyectosTiempos>(item);
            var insert = _generalService.IniciarCronometro(mapped);
            return Ok(insert);
        }


        //[HttpPut("Finalizar")]
        //public IActionResult FinalizarCronometro([FromBody] ProyectosTiemposViewModel item)
        //{
        //    var mapped = _mapper.Map<tbProyectosTiempos>(item);
        //    var update = _generalService.FinalizarCronometro(mapped);
        //    return Ok(update);
        //}

        [HttpPut("Finalizar/{registroID}/{horaFin}")]
        public IActionResult FinalizarCronometro(int registroID, TimeSpan horaFin)
        {
            var result = _generalService.FinalizarCronometro(registroID, horaFin);
            return Ok(result);
        }

    }
}
