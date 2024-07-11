using AccesoDatos; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Auditoria
    {
        public int id { get; set; }
        public int accion { get; set; }
        public string movimiento { get; set; }
        public DateTime fecha { get; set; }
        public int id_usuario { get; set; } 



        public string ObtenerAuditoria()
        {
            string audi = string.Format(@"insert into T_Auditoria (fecha, accion, idUser, tipoMovimiento)
                                values(GETDATE(),'{0}','{1}', '{2}');",
                                 accion,
                                 id_usuario,
                                 this.movimiento);

            return audi;
        }

        public Respuesta guardarAuditoria()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;
                  

                miRespuesta = transaccion(this.ObtenerAuditoria());

                if (miRespuesta.codigoError == -50)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Ya tienes una usuario con esa Cedula!";
                }

                else if (miRespuesta.codigoError == -3)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la Acción!";
                }

                else if (miRespuesta.codigoError > 0)
                {
                    miRespuesta.resultado = true;
                    miRespuesta.mensaje = "Guardado con exito!";
                }
                else
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "No se pudo Guardar, Intente nuevamente!";
                }
            }
            catch (Exception es)
            {
                miRespuesta.mensaje = "Ocurrio un error al Guardar el Usuario.";
                miRespuesta.resultado = false;
                miRespuesta.codigoError = -2;
            }

            return miRespuesta;
        }

        #region ************************************** TRANSACTION **************************************
        public Respuesta transaccion(string query)
        {
            Respuesta miRespuesta = new Respuesta();


            Conexion objCnx = new Conexion();

            if (objCnx.AbrirConexion())
            {

                string scalar =
                    string.Format(@" BEGIN TRANSACTION
                        BEGIN TRY
                        BEGIN
                        SET DATEFORMAT ymd

                        {0}	         

                        COMMIT; 
                        END
                        END TRY
                        BEGIN CATCH
                        ROLLBACK;
                        Select -3
                        END CATCH;", query);

                miRespuesta.codigoError = objCnx.HacerHitScalar(scalar);

                objCnx.CerrarConexion();
            }

            objCnx = null;

            return miRespuesta;
        }


        #endregion
    }
}
