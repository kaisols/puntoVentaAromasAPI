using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class ReporteBase
    {
        public int id { get; set; }
        public int tipoConsulta { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public string? valor { get; set; }
        public bool estado1 { get; set; }
        public bool estado2 { get; set; }
    }
}
