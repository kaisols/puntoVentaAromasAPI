using AccesoDatos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Cliente : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public string? cedula { get; set; }
        public string? nombre { get; set; }
        public string? correo { get; set; }
        public string? telefono { get; set; } 
        public string? direccion { get; set; } 
        public DateTime? fecha_registro { get; set; }
        public bool estado { get; set; }
        public int tipoConsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }

        #endregion ============================= PROPIEDADES =============================


        public Respuesta Actualizar()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;

                if (tipoConsulta == 1)
                {
                    consulta = $@" DECLARE @id as int = '{id}';

                                   UPDATE T_CLIENTE SET nombre = '{this.nombre}', correo = '{correo}', telefono = '{telefono}', direccion = '{direccion}'
                                   WHERE id = @id;
 
                                   {miAuditoria.ObtenerAuditoria()}
                                   
                                   SELECT 2;";
                }

                else if (tipoConsulta == 2)
                {

                    consulta = $@"UPDATE T_CLIENTE SET estado = '{this.estado}' WHERE id = '{this.id}'
                                            {miAuditoria.ObtenerAuditoria()}                                
                                            SELECT 2;";
                }




                miRespuesta = transaccion(consulta);

                if (miRespuesta.codigoError == -3)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la Acción!";
                }
                else if (miRespuesta.codigoError == 2)
                {
                    miRespuesta.resultado = true;
                    miRespuesta.mensaje = "Actualizado con exito!";
                }
                else
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la acción!";
                }
            }
            catch (Exception es)
            {
                miRespuesta.mensaje = "Ocurrio un error al realizar la acción.";
                miRespuesta.resultado = false;
                miRespuesta.codigoError = -2;
            }

            return miRespuesta;
        }

        public Respuesta ConsultaMasiva()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                List<Cliente> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoConsulta == 1)
                    {
                        consulta = $@"SELECT id, nombre, correo, telefono, cedula, direccion, fecha_registro, estado
                                    FROM T_CLIENTE C
                                    WHERE estado = '{Convert.ToByte(this.estado)}'
                                    FOR JSON PATH";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<List<Cliente>>(objDatos.HacerSelectJSONPATH(consulta));


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

        public Respuesta Guardar()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;

                if (tipoConsulta == 1)
                {
                    consulta = $@"IF((SELECT COUNT(*) FROM T_CLIENTE WHERE nombre = '{nombre}') <= 0)
	                                BEGIN
                                        
                                        DECLARE @id as int = (SELECT ISNULL(MAX(id), 0) + 1 from T_CLIENTE);

		                                INSERT INTO T_CLIENTE (id, nombre, correo, telefono, direccion, cedula)
		                                VALUES(@id, '{nombre}' ,'{correo}', '{telefono}', '{direccion}', '{cedula}'); 

                                         {miAuditoria.ObtenerAuditoria()}

                                          SELECT 2;


	                                END
                                ELSE
	                                BEGIN
                                        SELECT -50;
                                    END

                                           ";
                }




                miRespuesta = transaccion(consulta);

                if (miRespuesta.codigoError == -3)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la Acción!";
                }
                else if (miRespuesta.codigoError == 2)
                {
                    miRespuesta.resultado = true;
                    miRespuesta.mensaje = "Guardado con exito!";
                }
                else
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la acción!";
                }
            }
            catch (Exception es)
            {
                miRespuesta.mensaje = "Ocurrio un error al realizar la acción.";
                miRespuesta.resultado = false;
                miRespuesta.codigoError = -2;
            }

            return miRespuesta;
        }

        public Respuesta Obtener()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                Cliente objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoConsulta == 1)
                    {
                        consulta = $@"SELECT id, nombre, correo, telefono, cedula, direccion, fecha_registro, estado
                                    FROM T_CLIENTE C
                                    WHERE estado = '{Convert.ToByte(this.estado)}' AND id = '{this.id}'
                                    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<Cliente>(objDatos.HacerSelectJSONPATH(consulta));


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
