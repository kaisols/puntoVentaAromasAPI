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
    public class ReportesController : ControllerBase
    {

        [EnableCors("MyPolicy")]
        [HttpPost]
        [Route("ReporteAuditoria")]
        public Respuesta ReporteAuditoria(ReporteBase request)
        {
            Respuesta miRespuesta = new Respuesta();

            if (request != null)
            {
                miRespuesta = new ReporteAuditoria().ConsultaMasiva(request);
            }
            else
            {
                miRespuesta.codigoError = -15;
                miRespuesta.mensaje = "Ocurrio un error al conectar el servidor";
                miRespuesta.resultado = false;
            }

            return miRespuesta;
        }


        [EnableCors("MyPolicy")]
        [HttpPost]
        [Route("ReporteFacturas")]
        public Respuesta ReporteFacturas(ReporteBase request)
        {
            Respuesta miRespuesta = new Respuesta();

            if (request != null)
            {
                miRespuesta = new ReporteFacturas().ConsultaMasiva(request);
            }
            else
            {
                miRespuesta.codigoError = -15;
                miRespuesta.mensaje = "Ocurrio un error al conectar el servidor";
                miRespuesta.resultado = false;
            }

            return miRespuesta;
        }
    }
}
