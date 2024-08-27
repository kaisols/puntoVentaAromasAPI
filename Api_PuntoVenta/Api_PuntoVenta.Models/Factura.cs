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
    public class Factura : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Usuario? miUsuario { get; set; }
        public Cliente? miCliente { get; set; }
        public Aperturacaja? miApertura { get; set; }
        public Tipofactura? miTipoFactura { get; set; }
        public decimal subtotal { get; set; }
        public decimal montoTotal { get; set; }
        public decimal ivaTotal { get; set; }
        public decimal descuento { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }
        public List<Factura_Detalle>? Detalles { get; set; }  

        #endregion ============================= PROPIEDADES =============================


        #region METODOS 
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
                List<Entradainventario> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@" SELECT F.id, F.subtotal, F.montoTotal, F.ivaTotal, F.descuento, F.fecha_registro, F.estado,
                                         TF.id 'miTipoFactura.id', TF.nombre 'miTipoFactura.nombre',
                                         C.id 'miCliente.id', C.nombre 'miCliente.nombre', C.cedula 'miCliente.Cedula', C.telefono 'miCliente.telefono',
                                         U.id 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos', F.idApertura as 'miApertura.id'
                                         FROM T_Factura F
                                         INNER JOIN T_TipoFactura TF ON TF.id = F.idTipoFactura
                                         INNER JOIN T_Cliente C on C.id = F.idCliente
                                         INNER JOIN T_USUARIO U on U.id = F.idUsuario 
                                    FOR JSON PATH";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<List<Entradainventario>>(objDatos.HacerSelectJSONPATH(consulta));


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
                    consulta = $@" DECLARE @id as int = (SELECT ISNULL(MAX(id), 0) + 1 from T_Factura);

		                               INSERT INTO T_Factura (id, idUsuario, idCliente, idTipoFactura, subtotal, montoTotal, ivaTotal, descuento, idApertura)
                                       VALUES(@id, '{miUsuario.id}', '{miCliente.id}', '{miTipoFactura.id}', '{subtotal.ToString().Replace(",", ".")}',
                                              '{montoTotal.ToString().Replace(",", ".")}', '{ivaTotal.ToString().Replace(",", ".")}', '{descuento.ToString().Replace(",", ".")}',
                                              '{miApertura.id}'
                                              )

                            
                                        {InsertDetalles()}


                                         {miAuditoria.ObtenerAuditoria()}

                                          SELECT @id;";
                }




                miRespuesta = transaccion(consulta);

                if (miRespuesta.codigoError == -3)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Error al Realizar la Acción!";
                }
                else if (miRespuesta.codigoError > 0)
                {
                    miRespuesta.resultado = true;
                    miRespuesta.mensaje = "Factura Generada con Exito; Factura # "+ miRespuesta.codigoError.ToString();
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
                Factura objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@" SELECT F.id, F.subtotal, F.montoTotal, F.ivaTotal, F.descuento, F.fecha_registro, F.estado,
                                         TF.id 'miTipoFactura.id', TF.nombre 'miTipoFactura.nombre',
                                         C.id 'miCliente.id', C.nombre 'miCliente.nombre', C.cedula 'miCliente.Cedula', C.telefono 'miCliente.telefono',
                                         U.id 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos',
                                         (SELECT D.id, D.cantidad, D.subtotal, D.montoTotal, D.ivaTotal, D.descuento, D.fecha_registro, D.estado,
                                         P.id 'miProducto.id', P.nombre 'miProducto.nombre', F.idApertura 'miApertura.id'
                                         FROM T_Factura_Detalles D
                                         INNER JOIN T_Productos P on P.id = D.idProducto
                                         WHERE D.idFactura = F.id
                                         FOR JSON PATH) as Detalles
                                         FROM T_Factura F
                                         INNER JOIN T_TipoFactura TF ON TF.id = F.idTipoFactura
                                         INNER JOIN T_Cliente C on C.id = F.idCliente
                                         INNER JOIN T_USUARIO U on U.id = F.idUsuario
                                         WHERE F.id= {id}

                                      FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<Factura>(objDatos.HacerSelectJSONPATH(consulta));


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

        public string InsertDetalles()
        {
            string consulta = "";

            if (Detalles != null && Detalles.Count > 0)
            {
                foreach (var item in Detalles)
                {
                    consulta += $@"INSERT INTO T_Factura_Detalles (id, idFactura, idProducto, cantidad, subtotal, montoTotal, ivaTotal, descuento)
                                    VALUES((SELECT ISNULL(MAX(id), 0) + 1 from T_Factura_Detalles), @id, '{item.miProducto.id}', 
                                       '{item.cantidad.ToString().Replace(",", ".")}', '{item.subtotal.ToString().Replace(",", ".")}'
                                        ,'{item.montoTotal.ToString().Replace(",", ".")}'
                                        ,'{item.ivaTotal.ToString().Replace(",", ".")}'
                                        ,'{item.descuento.ToString().Replace(",", ".")}'
                                        )
                                    
                                 UPDATE T_Productos SET stock -= {item.cantidad.ToString().Replace(",", ".")} WHERE id = '{item.miProducto.id}' 
                                " + Environment.NewLine + Environment.NewLine;
                }
            }

            return consulta;
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

        #endregion
    }

}
