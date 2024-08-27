using AccesoDatos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class ArqueoCaja : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Usuario? miUsuario { get; set; }
        public Aperturacaja? miApertura { get; set; }
        public decimal? total5 { get; set; }
        public decimal? total10 { get; set; }
        public decimal? total25 { get; set; }
        public decimal? total50 { get; set; }
        public decimal? total100 { get; set; }
        public decimal? total500 { get; set; }
        public decimal? total1000 { get; set; }
        public decimal? total2000 { get; set; }
        public decimal? total5000 { get; set; }
        public decimal? total10000 { get; set; }
        public decimal? total20000 { get; set; }
        public decimal? totalSinpe { get; set; }
        public decimal? totalTransferencias { get; set; }
        public decimal? totalCheque { get; set; }
        public decimal montoTotal { get; set; }
        public decimal? sobranteCaja { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }

        #endregion ============================= PROPIEDADES =============================


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
                List<ArqueoCaja> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@" SELECT A.id, 
                                    A.total5, A.Total10, A.Total25, A.total50, A.total100, A.total500, A.total1000, A.total2000, A.total5000, A.total10000, A.total20000, A.totalSinpe, 
                                    A.totalTransferencias, A.totalCheque, A.montoTotal, A.sobranteCaja, A.fecha_registro, A.estado,
                                    A.idUsuario 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos',
                                    A.idApertura 'miApertura.id', AP.idCaja 'miApertura.miCaja.id', C.nombreCaja 'miApertura.miCaja.nombreCaja', C.numCaja 'miApertura.miCaja.numCaja'
                                    FROM T_ArqueoCaja A
                                    INNER JOIN T_USUARIO U on U.id = A.idUsuario
                                    INNER JOIN T_AperturaCaja AP on AP.id = A.idApertura
                                    INNER JOIN T_Caja C on C.id = AP.idCaja
                                    WHERE A.estado = '{Convert.ToByte(this.estado)}'
                                    FOR JSON PATH";
                    } 


                    objEncontrado = JsonConvert.DeserializeObject<List<ArqueoCaja>>(objDatos.HacerSelectJSONPATH(consulta));


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

                if (tipoconsulta == 1)
                {
                    consulta = $@" 
                                        DECLARE @id as int = (SELECT ISNULL(MAX(id), 0) + 1 from T_ArqueoCaja);


                                        DECLARE @montoFacturas as decimal(18, 5) = (SELECT SUM(montoTotal)
                                        FROM T_Factura WHERE estado = 1
                                        AND CONVERT(DATE, fecha_registro, 121) = CONVERT(date, GETDATE(), 121))

		                                 INSERT INTO T_ArqueoCaja (id, idUsuario, idApertura, total5, total10, total25, total50, total100, total500, total1000, total2000, 
                                          total5000, total10000, total20000, totalSinpe, totalTransferencias, totalCheque, montoTotal, sobranteCaja)
                                          VALUES(@id, 
                                           '{miUsuario.id}',
                                           '{miApertura.id}', 
                                           '{total5.ToString().Replace(",", ".")}',
                                           '{total10.ToString().Replace(",", ".")}',
                                           '{total25.ToString().Replace(",", ".")}',
                                           '{total50.ToString().Replace(",", ".")}',
                                           '{total100.ToString().Replace(",", ".")}',
                                           '{total500.ToString().Replace(",", ".")}',
                                           '{total1000.ToString().Replace(",", ".")}',
                                           '{total2000.ToString().Replace(",", ".")}',
                                           '{total5000.ToString().Replace(",", ".")}',
                                           '{total10000.ToString().Replace(",", ".")}',
                                           '{total20000.ToString().Replace(",", ".")}',
                                           '{totalSinpe.ToString().Replace(",", ".")}',
                                           '{totalTransferencias.ToString().Replace(",", ".")}',
                                           '{totalCheque.ToString().Replace(",", ".")}',
                                           '{montoTotal.ToString().Replace(",", ".")}',
                                           ({montoTotal.ToString().Replace(",", ".")} - @montoFacturas)
                                           )


                                         {miAuditoria.ObtenerAuditoria()}

                                          SELECT 2;
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
                ArqueoCaja objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@" SELECT A.id, 
                                    A.total5, A.Total10, A.Total25, A.total50, A.total100, A.total500, A.total1000, A.total2000, A.total5000, A.total10000, A.total20000, A.totalSinpe, 
                                    A.totalTransferencias, A.totalCheque, A.montoTotal, A.sobranteCaja, A.fecha_registro, A.estado,
                                    A.idUsuario 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos',
                                    A.idApertura 'miApertura.id', AP.idCaja 'miApertura.miCaja.id', C.nombreCaja 'miApertura.miCaja.nombreCaja', C.numCaja 'miApertura.miCaja.numCaja'
                                    FROM T_ArqueoCaja A
                                    INNER JOIN T_USUARIO U on U.id = A.idUsuario
                                    INNER JOIN T_AperturaCaja AP on AP.id = A.idApertura
                                    INNER JOIN T_Caja C on C.id = AP.idCaja
                                    WHERE A.estado = '{Convert.ToByte(this.estado)}' AND A.id = '{id}'
                                    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<ArqueoCaja>(objDatos.HacerSelectJSONPATH(consulta));


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
