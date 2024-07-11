using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    public class Respuesta
    {
        public int codigoError { get; set; }
        public string mensaje { get; set; }
        public object objeto { get; set; }
        public bool resultado { get; set; }
    }
}
