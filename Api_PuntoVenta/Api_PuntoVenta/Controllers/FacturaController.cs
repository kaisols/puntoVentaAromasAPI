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
    public class FacturaController : ControllerBase
    {
        [EnableCors("MyPolicy")]
        [HttpPost]
        [Route("Listar")]
        public Respuesta Listar(Factura request)
        {
            Respuesta miRespuesta = new Respuesta();

            if (request != null)
            {
                miRespuesta = request.ConsultaMasiva();
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
        [Route("Obtener")]
        public Respuesta Obtener(Factura request)
        {
            Respuesta miRespuesta = new Respuesta();

            if (request != null)
            {
                miRespuesta = request.Obtener();
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
        [Route("Guardar")]
        public Respuesta Guardar(Factura request)
        {
            Respuesta miRespuesta = new Respuesta();

            if (request != null)
            {
                if (request.id > 0)
                {
                    miRespuesta = request.Actualizar();
                }
                else
                {
                    miRespuesta = request.Guardar();
                }
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
