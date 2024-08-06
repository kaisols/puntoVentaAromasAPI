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
    public class Salidainventario : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int id { get; set; }
        public Motivo_salida? miMotivo { get; set; }
        public Usuario? miUsuario { get; set; }
        public DateTime? fecha_registro { get; set; }
        public bool estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; }
        public List<Salidainventario_detalle>? Detalles { get; set; }

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
                List<Salidainventario> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT S.id, S.fecha_registro, M.id as 'miMotivo.id', M.nombre as 'miMotivo.nombre',
                                        U.id as 'miUsuario.id', U.nombre as 'miUsuario.nombre', U.apellidos as 'miUsuario.apellidos'
                                        FROM T_SalidaInventario S 
                                        INNER JOIN T_Motivo_Salida M on M.id = S.idMotivo
                                        INNER JOIN T_USUARIO U on U.id = S.idUsuario
                                        WHERE S.estado = 1
                                    FOR JSON PATH";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<List<Salidainventario>>(objDatos.HacerSelectJSONPATH(consulta));


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
                    consulta = $@" DECLARE @id as int = (SELECT ISNULL(MAX(id), 0) + 1 from T_SalidaInventario);

		                               
                                        INSERT INTO T_SalidaInventario (id, idMotivo, idUsuario)
                                        VALUES(@id, '{miMotivo.id}', '{miUsuario.id}')

                            
                                        {InsertDetalles()}


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
                Salidainventario objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT S.id, S.fecha_registro, M.id as 'miMotivo.id', M.nombre as 'miMotivo.nombre',
                                U.id as 'miUsuario.id', U.nombre as 'miUsuario.nombre', U.apellidos as 'miUsuario.apellidos',
                                (SELECT D.id, D.cantidad, D.fecha_registro, D.estado, D.idSalida 'miSalida.id',
                                P.id 'miProducto.id', P.nombre 'miProducto.nombre', P.codBarras 'miProducto.codBarras'
                                FROM T_SalidaInventario_Detalle D
                                INNER JOIN T_Productos P on P.id = D.idProducto
                                WHERE D.idSalida = S.id
                                FOR JSON PATH) as Detalles
                                FROM T_SalidaInventario S 
                                INNER JOIN T_Motivo_Salida M on M.id = S.idMotivo
                                INNER JOIN T_USUARIO U on U.id = S.idUsuario
                                WHERE S.id = '{this.id}'
                                      FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }

                    objEncontrado = JsonConvert.DeserializeObject<Salidainventario>(objDatos.HacerSelectJSONPATH(consulta));


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
                    consulta += $@"INSERT INTO T_SalidaInventario_Detalle (id, idSalida, idProducto, cantidad)
                                    VALUES((SELECT ISNULL(MAX(id), 0) + 1 from T_SalidaInventario_Detalle), @id, '{item.miProducto.id}', 
                                       '{item.cantidad.ToString().Replace(",", ".")}')
                                    
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
