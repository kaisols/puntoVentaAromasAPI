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
    public class Productos : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Categoria? miCategoria { get; set; }
        public Impuesto? miImpuesto { get; set; }
        public Unidadmedida? miUnidadMedida { get; set; }
        public string? nombre { get; set; }
        public string? codBarras { get; set; }
        public decimal stock { get; set; }
        public bool reduceInventario { get; set; }
        public decimal precioCompra { get; set; }
        public decimal precioVenta { get; set; }
        public decimal utilidad { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }
        public List<Proveedor>? Proveedores { get; set; }

        #endregion ============================= PROPIEDADES =============================

        public Respuesta Actualizar()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;

                if (tipoconsulta == 1)
                {
                    consulta = $@"DECLARE @id as int =  '{id}';

                                   UPDATE T_Productos SET nombre = '{this.nombre}', 
                                    idCategoria = '{miCategoria.id}', 
                                    idUnidadMedida = '{miUnidadMedida.id}', 
                                    idImpuesto = '{miImpuesto.id}',
                                    reduceInventario = '{reduceInventario}', 
                                    precioCompra = '{precioCompra.ToString(new CultureInfo("en-US"))}',
                                    precioVenta = '{precioVenta.ToString(new CultureInfo("en-US"))}',
                                    utilidad = '{utilidad.ToString(new CultureInfo("en-US"))}'
                                   WHERE id = @id ;
 
                                   {GetProveedores()}
                        

                                   {miAuditoria.ObtenerAuditoria()}
                                   
                                   SELECT 2;";
                }

                else if (tipoconsulta == 2)
                {

                    consulta = $@"UPDATE T_Productos SET estado = '{this.estado}' WHERE id = '{this.id}'
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
                List<Productos> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT 
                                    P.id, P.nombre, P.codBarras, P.stock, P.reduceInventario, P.precioCompra, P.precioVenta, P.utilidad, P.fecha_registro, P.estado,
                                    C.id as 'miCategoria.id', C.nombre 'miCategoria.nombre',
                                    I.id as 'miImpuesto.id', I.nombre as 'miImpuesto.nombre', I.PorcentajeIVA as 'miImpuesto.PorcentajeIVA',
                                    U.id as 'miUnidadMedida.miUnidadMedida', U.nombre as 'miUnidadMedida.nombre'
                                    FROM T_Productos P
                                    INNER JOIN T_Categoria C on C.id = P.idCategoria
                                    INNER JOIN T_UnidadMedida U on U.id = P.idUnidadMedida
                                    INNER JOIN T_Impuesto I on I.id = P.idImpuesto
                                    WHERE P.estado = '{Convert.ToByte(this.estado)}'
                                    FOR JSON PATH";
                    }
                     

                    objEncontrado = JsonConvert.DeserializeObject<List<Productos>>(objDatos.HacerSelectJSONPATH(consulta));


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
                    consulta = $@"IF((SELECT COUNT(*) FROM T_Productos WHERE codBarras = '{codBarras}') <= 0)
	                                BEGIN
                                        
                                        DECLARE @id as int = (SELECT ISNULL(MAX(id), 0) + 1 from T_Productos);

		                                INSERT INTO T_Productos (id, idCategoria, idImpuesto, idUnidadMedida, nombre, codBarras, stock, reduceInventario, precioCompra, precioVenta,utilidad)
                                        VALUES(@id, '{miCategoria.id}', '{miImpuesto.id}', '{miUnidadMedida.id}', '{nombre}', '{codBarras}', '0', '{reduceInventario}', '{precioCompra.ToString().Replace(",", ".")}', '{precioVenta.ToString().Replace(",", ".")}', '{utilidad.ToString().Replace(",", ".")}')

                                        {GetProveedores()}


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

                if (miRespuesta.codigoError == -50)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Ya tienes un registro con ese valor.";
                }

                else if (miRespuesta.codigoError == -3)
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
                Productos objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT 
                                    P.id, P.nombre, P.codBarras, P.stock, P.reduceInventario, P.precioCompra, P.precioVenta, P.utilidad, P.fecha_registro, P.estado,
                                    C.id as 'miCategoria.id', C.nombre 'miCategoria.nombre',
                                    I.id as 'miImpuesto.id', I.nombre as 'miImpuesto.nombre', I.PorcentajeIVA as 'miImpuesto.PorcentajeIVA',
                                    U.id as 'miUnidadMedida.id', U.nombre as 'miUnidadMedida.nombre',
                                    (SELECT 
                                        PX.id, PX.nombre, PX.cedula, PX.telefono, PX.correo, PX.fecha_registro, PX.estado 
                                        FROM T_ProductoProveedor PP
                                        INNER JOIN T_Proveedor PX on PP.idProveedor = PX.id
                                        WHERE PP.idProducto = P.id
                                        FOR JSON PATH) as  Proveedores
                                    FROM T_Productos P
                                    INNER JOIN T_Categoria C on C.id = P.idCategoria
                                    INNER JOIN T_UnidadMedida U on U.id = P.idUnidadMedida
                                    INNER JOIN T_Impuesto I on I.id = P.idImpuesto
                                    WHERE P.estado = '{Convert.ToByte(this.estado)}' AND P.id = '{id}'
                                    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<Productos>(objDatos.HacerSelectJSONPATH(consulta));


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

        public string GetProveedores()
        {
            string consulta = "";
            consulta = "DELETE FROM T_ProductoProveedor WHERE idProducto = @id" + Environment.NewLine; 

            if (Proveedores != null && Proveedores.Count > 0)
            { 
                foreach (var item in Proveedores)
                {
                    consulta += $@"INSERT INTO T_ProductoProveedor (id,idProveedor, idProducto)
                                  VALUES((SELECT ISNULL(MAX(id), 0) + 1 from T_ProductoProveedor), '{item.id}', @id)" + Environment.NewLine;
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




    }
}
