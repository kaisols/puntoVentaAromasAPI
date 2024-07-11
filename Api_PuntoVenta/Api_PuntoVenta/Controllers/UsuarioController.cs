using Api_PuntoVenta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api_PuntoVenta.Controllers
{
    [ApiController]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        [EnableCors("MyPolicy")]
        [HttpPost]
        [Route("Login"), AllowAnonymous]
        public Respuesta Login(Usuario request)
        {
            Respuesta miRespuesta = new Respuesta();

            if (request != null)
            {
                miRespuesta = request.Login();
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
        [Route("Listar"), Authorize]
        public Respuesta Listar(Usuario request)
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
        [Route("Obtener"), Authorize]
        public Respuesta Obtener(Usuario request)
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


        [HttpPost]
        [Route("Guardar"), Authorize]
        public Respuesta Guardar(Usuario request)
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