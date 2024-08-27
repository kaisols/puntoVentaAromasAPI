using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Factura_Detalle  
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Factura? miFactura { get; set; }
        public Productos? miProducto { get; set; }
        public decimal cantidad { get; set; }
        public decimal subtotal { get; set; }
        public decimal montoTotal { get; set; }
        public decimal ivaTotal { get; set; }
        public decimal descuento { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }

        #endregion ============================= PROPIEDADES =============================

    }
}
