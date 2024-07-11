using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    public interface IOrdenConsulta
    {
        Respuesta Guardar();
        Respuesta Obtener();
        Respuesta Actualizar();
        Respuesta ConsultaMasiva();
        Respuesta transaccion(string query);
    }
}
