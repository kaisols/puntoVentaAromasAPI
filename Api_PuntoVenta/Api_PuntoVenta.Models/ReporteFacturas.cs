using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    public class ReporteFacturas
    {
        public Respuesta ConsultaMasiva(ReporteBase In)
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                List<Factura> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (In.tipoConsulta == 1)
                    {
                           consulta = $@" SELECT F.id, F.subtotal, F.montoTotal, F.ivaTotal, F.descuento, F.fecha_registro, F.estado,
                                         TF.id 'miTipoFactura.id', TF.nombre 'miTipoFactura.nombre',
                                         C.id 'miCliente.id', C.nombre 'miCliente.nombre', C.cedula 'miCliente.Cedula', C.telefono 'miCliente.telefono',
                                         U.id 'miUsuario.id', U.nombre 'miUsuario.nombre', U.apellidos 'miUsuario.apellidos'
                                         FROM T_Factura F
                                         INNER JOIN T_TipoFactura TF ON TF.id = F.idTipoFactura
                                         INNER JOIN T_Cliente C on C.id = F.idCliente
                                         INNER JOIN T_USUARIO U on U.id = F.idUsuario
                                          WHERE CONVERT(DATE, F.fecha_registro, 121) BETWEEN CONVERT(DATE, '{In.Fecha_Inicio.ToString("yyyy-MM-dd")}', 121) 
                                         AND CONVERT(DATE, '{In.Fecha_Fin.ToString("yyyy-MM-dd")}', 121)  
                                    FOR JSON PATH";
                    }
                     


                    objEncontrado = JsonConvert.DeserializeObject<List<Factura>>(objDatos.HacerSelectJSONPATH(consulta));


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
    }
}
