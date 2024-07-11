using Api_PuntoVenta.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Permisos
    { 
        public Permiso_Usuario Permiso_Usuario { get; set; } = new Permiso_Usuario();
        public Permiso_Roles Permiso_Roles { get; set; } = new Permiso_Roles();
    }
     

    public class Permiso_Usuario
    {
        public bool Guardar { get; set; }
        public bool Editar { get; set; }
        public bool Acceso { get; set; }
        public bool Eliminar { get; set; }
        public bool Restablecer_Contrasenna { get; set; }
    }
    public class Permiso_Roles
    {
        public bool Guardar { get; set; }
        public bool Editar { get; set; }
        public bool Acceso { get; set; }
        public bool Eliminar { get; set; }
    }
}
