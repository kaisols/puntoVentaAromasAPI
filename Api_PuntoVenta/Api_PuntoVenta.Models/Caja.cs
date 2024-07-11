using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Caja : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int? id { get; set; }
        public string? nombreCaja { get; set; }
        public int? numCaja { get; set; }
        public DateTime? fecha_registro { get; set; }
        public bool? estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }

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
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                List<Caja> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {

                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT 
                                     id, nombreCaja, numCaja, estado
                                     FROM T_Caja
                                     WHERE estado =  '{Convert.ToByte(this.estado)}'
                                     FOR JSON PATH ";
                    }



                    objEncontrado = JsonConvert.DeserializeObject<List<Caja>>(objDatos.HacerSelectJSONPATH(consulta));


                    if (objEncontrado != null)
                    {
                        miRespuesta.resultado = true;
                        miRespuesta.mensaje = "Exito";
                        miRespuesta.codigoError = 2;
                        miRespuesta.objeto = objEncontrado;

                    }
                    else
                    {
                        miRespuesta.resultado = false;
                        miRespuesta.mensaje = "Sin Datos";
                        miRespuesta.codigoError = -1;
                        miRespuesta.objeto = null;
                    }

                    //Se cierra la conexion
                    objDatos.CerrarConexion();
                }
                else
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Abrir la Conexion.";
                    miRespuesta.codigoError = -2;
                    miRespuesta.objeto = null;
                }
                //El objeto objDatos es destruido
                objDatos = null;


            }
            catch (Exception es)
            {
                objDatos.CerrarConexion();
                miRespuesta.resultado = false;
                miRespuesta.mensaje = "Error en el Servicio, Intente de Nuevo.";
                miRespuesta.codigoError = -3;
                miRespuesta.objeto = null;
            }


            return miRespuesta;
        }

        public Respuesta transaccion(string query)
        {
            throw new NotImplementedException();
        }
    }
}
