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
    public class Aperturacaja : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Usuario miUsuario { get; set; }
        public Cierrecaja? miCierreCaja { get; set; }
        public Caja? miCaja { get; set; }
        public decimal montoCaja { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }


        #endregion ============================= PROPIEDADES =============================

        public Respuesta Guardar()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;

                if (tipoconsulta == 1)
                {
                    consulta = $@"                                
                                IF NOT EXISTS (SELECT 1 FROM T_AperturaCaja
                                WHERE idUsuario = '{miUsuario}' AND CONVERT(DATE, fecha_registro, 121) = CONVERT(DATE, GETDATE(), 121)
                                AND estado = 1 AND idCierreCaja IS NOT NULL AND idCaja = '{miCaja.id}'
                                )

	                                BEGIN
                                     DECLARE @id int; set @id = (SELECT ISNULL(MAX(id), 0) + 1 from T_AperturaCaja);

                                     INSERT INTO T_AperturaCaja (id, idUsuario, montoCaja, fecha_registro, estado, idCaja)
                                     VALUES(@id, '{miUsuario.id}', '{montoCaja.ToString().Replace(',', '.')}', GETDATE(), 1, '{miCaja.id}');

                                     {miAuditoria.ObtenerAuditoria()}

                                     SELECT @id;
	
	                                END
                                ELSE
	                                BEGIN
                                        SELECT -50;
	                                END


                                 ";
                }




                miRespuesta = transaccion(consulta);
 
                if(miRespuesta.codigoError == -3)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Ya tienes una apertura para hoy.";
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
                    miRespuesta.objeto = new Aperturacaja { id = miRespuesta.codigoError, tipoconsulta = 2, estado = true }.Obtener().objeto;
                }
                else
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "No se pudo Guardar, Intente nuevamente!";
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

        #region ************************************** OBTENER **************************************
        public Respuesta Obtener()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                Aperturacaja objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {

                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT TOP 1
                                A.id, A.montoCaja, A.fecha_registro, A.estado,
                                U.id as 'miUsuario.id',  U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos',
                                C.id 'miCierreCaja.id', C.montoCaja 'miCierreCaja.montoCaja', C.fecha_registro 'miCierreCaja.fecha_registro',
                                CC.id as 'miCaja.id', CC.nombreCaja as 'miCaja.nombreCaja', CC.numCaja as 'miCaja.numCaja'
                                FROM T_AperturaCaja A 
                                INNER JOIN T_USUARIO U on U.id = A.idUsuario
                                INNER JOIN T_Caja CC on CC.id = A.idCaja
                                LEFT JOIN T_CierreCaja C on C.id = A.idCierreCaja
                                WHERE A.estado = '{Convert.ToByte(this.estado)}' and A.idUsuario = '{this.miUsuario.id}' 
                                AND CONVERT(DATE, A.fecha_registro, 121) = CONVERT(DATE, GETDATE(), 121) AND A.idCierreCaja IS NULL and A.estado = 1
                                ORDER BY A.id DESC
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }

                    if (tipoconsulta == 2)
                    {
                        consulta = $@"SELECT 
                                A.id, A.montoCaja, A.fecha_registro, A.estado,
                                U.id as 'miUsuario.id',  U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos',
                                C.id 'miCierreCaja.id', C.montoCaja 'miCierreCaja.montoCaja', C.fecha_registro 'miCierreCaja.fecha_registro',
                                CC.id as 'miCaja.id', CC.nombreCaja as 'miCaja.nombreCaja', CC.numCaja as 'miCaja.numCaja'
                                FROM T_AperturaCaja A 
                                INNER JOIN T_USUARIO U on U.id = A.idUsuario
                                INNER JOIN T_Caja CC on CC.id = A.idCaja
                                LEFT JOIN T_CierreCaja C on C.id = A.idCierreCaja
                                WHERE A.estado = '{Convert.ToByte(this.estado)}' and A.id = '{this.id}' 
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }



                    objEncontrado = JsonConvert.DeserializeObject<Aperturacaja>(objDatos.HacerSelectJSONPATH(consulta));


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
        #endregion ************************************** OBTENER **************************************

        #region ************************************** ACTUALIZAR **************************************
        public Respuesta Actualizar()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;

                if (tipoconsulta == 1)
                {
                    consulta = $@"UPDATE T_AperturaCaja set idCierre = '{this.miCierreCaja.id}' where id = '{this.id}'
                                            {miAuditoria.ObtenerAuditoria()}                                
                                            SELECT 2;";
                } 

                else if (tipoconsulta == 2)
                {
                    consulta = $@"UPDATE T_AperturaCaja set estado = '{this.estado}' where id = '{this.id}'
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
                miRespuesta.mensaje = "Ocurrio un error al actualizar el Usuario.";
                miRespuesta.resultado = false;
                miRespuesta.codigoError = -2;
            }

            return miRespuesta;
        }
        #endregion

        public Respuesta ConsultaMasiva()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                List<Aperturacaja> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {

                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT 
                                        A.id, A.montoCaja, A.fecha_registro, A.estado,
                                        U.id as 'miUsuario.id',  U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos',
                                        C.id 'miCierreCaja.', C.montoCaja 'miCierreCaja.montoCaja', C.fecha_registro 'miCierreCaja.fecha_registro',
                                        CC.id as 'miCaja.id', CC.nombreCaja as 'miCaja.nombreCaja', CC.numCaja as 'miCaja.numCaja'
                                        FROM T_AperturaCaja A 
                                        INNER JOIN T_USUARIO U on U.id = A.idUsuario
                                        INNER JOIN T_Caja CC on CC.id = A.idCaja
                                        LEFT JOIN T_CierreCaja C on C.id = A.idCierreCaja
                                        WHERE A.estado =  '{Convert.ToByte(this.estado)}'
                                      FOR JSON PATH ";
                    } 



                    objEncontrado = JsonConvert.DeserializeObject<List<Aperturacaja>>(objDatos.HacerSelectJSONPATH(consulta));


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
