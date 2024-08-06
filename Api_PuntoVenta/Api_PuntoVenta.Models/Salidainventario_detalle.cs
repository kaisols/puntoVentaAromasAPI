using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    public class Salidainventario_detalle
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Salidainventario? miSalida { get; set; }
        public Productos miProducto { get; set; }
        public decimal cantidad { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }

        #endregion ============================= PROPIEDADES =============================
    }
}
