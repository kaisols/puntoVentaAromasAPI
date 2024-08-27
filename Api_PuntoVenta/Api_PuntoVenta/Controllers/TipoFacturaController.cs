using Api_PuntoVenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_PuntoVenta.Controllers
{
    [Authorize]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoFacturaController : ControllerBase
    {
        [EnableCors("MyPolicy")]
        [HttpPost]
        [Route("Listar")]
        public Respuesta Listar()
        {
            Respuesta miRespuesta = new Respuesta();

            miRespuesta = new Tipofactura { tipoconsulta = 1 }.ConsultaMasiva();

            return miRespuesta;
        }
    }
}
