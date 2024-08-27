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
    public class Cierrecaja : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Usuario? miUsuario { get; set; }
        public decimal montoCaja { get; set; }
        public decimal totalFacturas { get; set; }
        public decimal totalArqueos { get; set; }
        public decimal diferencias { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }
        public Aperturacaja? miApertura { get; set; }
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
                                       if((SELECT idCierreCaja FROM T_AperturaCaja  WHERE id= '{miApertura.id}') IS NULL)
                                            BEGIN
                                                    DECLARE @id as int = (SELECT ISNULL(MAX(id), 0) + 1 from T_CierreCaja);

                                                     INSERT INTO T_CierreCaja (id, idUsuario, montoCaja, fecha_registro, estado, totalFacturas, totalArqueos, diferencias)
                                                     VALUES(@id, '{miUsuario.id}', 
                                                     '{montoCaja.ToString().Replace(",", ".")}', 
                                                      GETDATE(), 
                                                      1, 
                                                     '{totalFacturas.ToString().Replace(",", ".")}', 
                                                     '{totalArqueos.ToString().Replace(",", ".")}',
                                                     '{diferencias.ToString().Replace(",", ".")}')

                                                     UPDATE T_AperturaCaja SET idCierreCaja = @id where id = '{miApertura.id}'


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

                if(miRespuesta.codigoError == -50)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Esta Apertura ya tiene un cierre.";
                }
                else if (miRespuesta.codigoError == -3)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la Acción!";
                }
                else if (miRespuesta.codigoError == 2)
                {
                    miRespuesta.resultado = true;
                    miRespuesta.mensaje = "Guardado con exito; se procede a cerrar la Caja!";
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
                Cierrecaja objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT
                                    C.id, C.montoCaja, C.fecha_registro, c.estado, c.totalFacturas, C.totalArqueos, C.diferencias,
                                    U.id 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.ápellidos',
                                    A.id 'miApertura.id', A.fecha_registro 'miApertura.fecha_registro', 
                                    A.idCaja 'miApertura.miCaja.id', CA.nombreCaja 'miApertura.miCaja.nombreCaja', CA.numCaja 'miApertura.miCaja.numCaja'
                                    FROM T_CierreCaja C
                                    INNER JOIN T_USUARIO U on U.id = C.idUsuario
                                    INNER JOIN T_AperturaCaja A on A.idCierreCaja = C.id
                                    INNER JOIN T_Caja CA on CA.id = A.idCaja
                                    WHERE A.estado = 1 AND C.estado = 1 AND C.id = '{id}'
                                    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }


                    if (tipoconsulta == 2)
                    {
                        consulta = $@"SELECT 
                                        (SELECT ISNULL( SUM(montoTotal), 0) FROM T_Factura WHERE idApertura = '{miApertura.id}') as totalFacturas,
                                        (SELECT ISNULL(SUM(montoTotal), 0) FROM T_ArqueoCaja WHERE idApertura = '{miApertura.id}') as totalArqueos
                                    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                                    ";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<Cierrecaja>(objDatos.HacerSelectJSONPATH(consulta));


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
                List<Cierrecaja> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT
                                    C.id, C.montoCaja, C.fecha_registro, c.estado, c.totalFacturas, C.totalArqueos, C.diferencias,
                                    U.id 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.ápellidos',
                                    A.id 'miApertura.id', A.fecha_registro 'miApertura.fecha_registro', 
                                    A.idCaja 'miApertura.miCaja.id', CA.nombreCaja 'miApertura.miCaja.nombreCaja', CA.numCaja 'miApertura.miCaja.numCaja'
                                    FROM T_CierreCaja C
                                    INNER JOIN T_USUARIO U on U.id = C.idUsuario
                                    INNER JOIN T_AperturaCaja A on A.idCierreCaja = C.id
                                    INNER JOIN T_Caja CA on CA.id = A.idCaja
                                    WHERE A.estado = 1 AND C.estado = 1
                                    FOR JSON PATH";
                    }


                    objEncontrado = JsonConvert.DeserializeObject<List<Cierrecaja>>(objDatos.HacerSelectJSONPATH(consulta));


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
    }
}
