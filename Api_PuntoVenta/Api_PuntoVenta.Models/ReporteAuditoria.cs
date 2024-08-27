using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_PuntoVenta.Models
{
    public class ReporteAuditoria
    {
        public int id { get; set; }
        public string tipoMovimiento { get; set; }
        public DateTime fecha { get; set; }
        public string Usuario { get; set; }
        public string Accion { get; set; }



        public Respuesta ConsultaMasiva(ReporteBase In)
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                List<ReporteAuditoria> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {
                    if (In.tipoConsulta == 1)
                    {
                        consulta = $@"SELECT A.id, A.tipoMovimiento, A.fecha, 
                                    CONCAT(U.nombre, ' ', U.apellidos) as Usuario,
                                    AX.descripcion as Accion
                                    FROM T_Auditoria A
                                    INNER JOIN T_ACCIONES AX on AX.id = A.accion
                                    INNER JOIN T_USUARIO U on U.id = A.idUser
                                    WHERE CONVERT(DATE, A.fecha, 121) BETWEEN CONVERT(DATE, '{In.Fecha_Inicio.ToString("yyyy-MM-dd")}', 121) 
                                    AND CONVERT(DATE, '{In.Fecha_Fin.ToString("yyyy-MM-dd")}', 121) 
                                    FOR JSON PATH";
                    }
                    else if (In.tipoConsulta == 2)
                    {
                        consulta = $@"SELECT A.id, A.tipoMovimiento, A.fecha, 
                                    CONCAT(U.nombre, ' ', U.apellidos) as Usuario,
                                    AX.descripcion as Accion
                                    FROM T_Auditoria A
                                    INNER JOIN T_ACCIONES AX on AX.id = A.accion
                                    INNER JOIN T_USUARIO U on U.id = A.idUser
                                    WHERE CONVERT(DATE, A.fecha, 121) BETWEEN CONVERT(DATE, '{In.Fecha_Inicio.ToString("yyyy-MM-dd")}', 121) 
                                    AND CONVERT(DATE, '{In.Fecha_Fin.ToString("yyyy-MM-dd")}', 121) 
                                    AND U.id = {In.id}
                                    FOR JSON PATH";
                    } 



                    objEncontrado = JsonConvert.DeserializeObject<List<ReporteAuditoria>>(objDatos.HacerSelectJSONPATH(consulta));


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
