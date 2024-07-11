using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Cierrecaja : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Usuario miUsuario { get; set; }
        public decimal montoCaja { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }

        #endregion ============================= PROPIEDADES =============================


        public Respuesta Guardar()
        {
            throw new NotImplementedException();
        }

        public Respuesta Obtener()
        {
            throw new NotImplementedException();
        }

        public Respuesta Actualizar()
        {
            throw new NotImplementedException();
        }

        public Respuesta ConsultaMasiva()
        {
            throw new NotImplementedException();
        }

        public Respuesta transaccion(string query)
        {
            throw new NotImplementedException();
        }
    }
}
