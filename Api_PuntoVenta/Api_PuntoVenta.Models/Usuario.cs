using AccesoDatos; 
using Newtonsoft.Json;

namespace Api_PuntoVenta.Models
{
    [ValidateNever]
    public class Usuario : IOrdenConsulta
    {
        #region ============================= PROPIEDADES =============================

        public int? id { get; set; }
        public string? nombre { get; set; }
        public Rol? miRol { get; set; } = null;
        public string? cedula { get; set; }
        public string? apellidos { get; set; }
        public string? telefono { get; set; }
        public string? user_name { get; set; }
        public string? password { get; set; }
        public string? correo { get; set; }
        public string? Token { get; set; }
        public DateTime? fecha_registro { get; set; }
        public bool? estado { get; set; }
        public int? tipoconsulta { get; set; }
        public Auditoria? miAuditoria { get; set; } 
        public Aperturacaja? miApertura { get; set; }

        #endregion ============================= PROPIEDADES =============================


        #region ************************************** METODOS **************************************

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
                    consulta = $@"IF((select COUNT(*) from T_Usuario where UPPER(correo) = UPPER('{this.correo}') and id <> '{id}') = 0)
                                        BEGIN

                                            UPDATE T_Usuario set nombre = '{this.nombre}', apellidos = '{apellidos}', correo = '{this.correo}',
                                            telefono = '{telefono}',
                                            idRol = '{miRol.id}'
                                            where id = '{id}'

                                            {miAuditoria.ObtenerAuditoria()}

                                            SELECT 2;
                                        END
                                            ELSE
                                                SELECT -50;";
                }

                else if (tipoconsulta == 1)
                {
                    consulta = $@"UPDATE T_Usuario set estado = '{Convert.ToByte(this.estado)}' where id = '{this.id}'
                                            {miAuditoria.ObtenerAuditoria()}                                
                                            SELECT 2;";
                }

                else if (tipoconsulta == 2)
                {
                    consulta = $@"UPDATE T_Usuario set password = '{this.password}' where id = '{this.id}'
                                            {miAuditoria.ObtenerAuditoria()}                                
                                            SELECT 2;";
                }




                miRespuesta = transaccion(consulta);

                if (miRespuesta.codigoError == -50)
                {
                    miRespuesta.resultado = false;
                    miRespuesta.mensaje = "Ya tienes un usuario con ese correo!";
                }
                else if (miRespuesta.codigoError == -3)
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

        #region ************************************** CONSULTA MASIVA **************************************
        public Respuesta ConsultaMasiva()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                List<Usuario> objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {

                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT CU.id, CU.nombre, CU.apellidos, CU.cedula, CU.correo, CU.telefono, CU.user_name, CU.password, CU.fecha_registro,
                                        CU.estado, 
                                        R.id 'miRol.id', R.Nombre 'miRol.nombre',  JSON_QUERY(R.permisos) 'miRol.permisos'
                                        FROM T_USUARIO CU
                                        INNER JOIN T_ROL R ON R.id = CU.idRol 
                                      Where CU.estado = '{Convert.ToByte(this.estado)}'
                                      FOR JSON PATH ";
                    } 
                    else if (tipoconsulta == 2)
                    {
                        consulta = $@"SELECT CU.id, CONCAT(CU.nombre, ' ' ,CU.apellidos) as nombre, CU.cedula, CU.correo, CU.telefono, CU.user_name, CU.password, CU.fecha_registro,
                                        CU.estado, 
                                        R.id 'miRol.id', R.Nombre 'miRol.nombre', JSON_QUERY(R.permisos) 'miRol.permisos'
                                        FROM T_USUARIO CU
                                        INNER JOIN T_ROL R ON R.id = CU.idRol 
                                      Where CU.estado = '{Convert.ToByte(this.estado)}'
                                      FOR JSON PATH ";
                    }



                    objEncontrado = JsonConvert.DeserializeObject<List<Usuario>>(objDatos.HacerSelectJSONPATH(consulta));


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
        #endregion

        #region ************************************** OBTENER **************************************
        public Respuesta Obtener()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                Usuario objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {

                    if (tipoconsulta == 1)
                    {
                        consulta = $@"SELECT CU.id, CU.nombre, CU.apellidos, CU.cedula, CU.correo, CU.telefono, CU.user_name, CU.password, CU.fecha_registro,
                                        CU.estado, 
                                        R.id 'miRol.id', R.Nombre 'miRol.nombre', JSON_QUERY(R.permisos) 'miRol.permisos'
                                        FROM T_USUARIO CU
                                        INNER JOIN T_ROL R ON R.id = CU.idRol 
                                      Where CU.estado = '{Convert.ToByte(this.estado)}' and cu.id = '{this.id}' 
                                      FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";
                    }



                    objEncontrado = JsonConvert.DeserializeObject<Usuario>(objDatos.HacerSelectJSONPATH(consulta));


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


        #region ************************************** LOGIN **************************************
        public Respuesta Login()
        {
            //Luego instanciamos la clase conexion para su suso
            AccesoDatos.Conexion objDatos = new AccesoDatos.Conexion();
            Respuesta miRespuesta = new Respuesta();

            try
            {
                Usuario objEncontrado = null;
                string consulta = "";

                if (objDatos.AbrirConexion())
                {

                    consulta = $@"SELECT U.id, U.nombre, U.apellidos, U.cedula, U.telefono, U.correo, U.user_name, U.password, U.estado,
                                R.id as 'miRol.id', R.nombre 'miRol.nombre', JSON_QUERY(R.permisos) 'miRol.permisos'
                                FROM T_USUARIO U
                                INNER JOIN T_ROL R on R.id = U.idRol
                                WHERE UPPER(U.user_name) = UPPER('{this.user_name}') AND U.estado = 1 
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER";


                    objEncontrado = JsonConvert.DeserializeObject<Usuario>(objDatos.HacerSelectJSONPATH(consulta));


                    if (objEncontrado != null)
                    {
                        if (objEncontrado.password.Equals(this.password))
                        {
                            String Token = Seguridad.clsSecurity.GenerateToken(objEncontrado);

                            if (!String.IsNullOrEmpty(Token))
                            {
                                var auditoria = new Auditoria
                                {
                                    accion = 100,
                                    id_usuario = objEncontrado.id.Value,
                                    movimiento = "Inicio de Sesión Exitoso."
                                }.guardarAuditoria();


                                objEncontrado.Token = Token;

                                Respuesta respApertura = new Aperturacaja { miUsuario = objEncontrado, tipoconsulta = 1, estado = true}.Obtener();

                                Aperturacaja aperturaCaja = null;

                                if(respApertura.resultado && respApertura.objeto != null)
                                {
                                    aperturaCaja = (Aperturacaja)respApertura.objeto;
                                }

                                objEncontrado.miApertura = aperturaCaja;
                                miRespuesta.resultado = true;
                                miRespuesta.mensaje = "Exito";
                                miRespuesta.codigoError = 2;
                                miRespuesta.objeto = objEncontrado;
                            }
                            else
                            {
                                miRespuesta.resultado = false;
                                miRespuesta.mensaje = "No se pudo generar el Token!";
                                miRespuesta.codigoError = -1;
                                miRespuesta.objeto = null;
                            }
                        }
                        else
                        {
                            miRespuesta.resultado = false;
                            miRespuesta.mensaje = "Las Contraseñas no coinciden!";
                            miRespuesta.codigoError = -1;
                            miRespuesta.objeto = null;
                        }

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
        #endregion ************************************** LOGIN **************************************

        #region ************************************** GUARDAR **************************************
        public Respuesta Guardar()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                //Cantidad de registros afectados por la consulta 
                string consulta = string.Empty;

                if (tipoconsulta == 1)
                {
                    consulta = $@"DECLARE @id int; set @id = (SELECT ISNULL(MAX(id), 0) + 1 from T_Usuario);

                                    IF((select COUNT(*) from T_Usuario where UPPER(cedula) = UPPER('{this.cedula}')) = 0)
                                        BEGIN
                                            
                                            INSERT INTO T_Usuario (id, nombre, cedula, apellidos, user_name, password, correo, telefono, idRol)
                                            VALUES(@id, '{this.nombre}', '{this.cedula}', '{this.apellidos}', '{this.user_name}', 
                                            '{this.password}', '{this.correo}', '{telefono}', '{miRol.id}')

                                            {miAuditoria.ObtenerAuditoria()}

                                            SELECT @id;
                                        END
                                            ELSE
                                                SELECT -50;";
                }




                miRespuesta = transaccion(consulta);

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
        #endregion

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






        public override string ToString()
        {
            return nombre + " " + apellidos;
        }
    }
}