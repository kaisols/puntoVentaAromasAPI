using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace AccesoDatos
{
    public class Conexion
    {
        #region propiedades
        //Creamos un objeto SqlConnection que es necesario para realizar la conexion
        SqlConnection cnx = null;
        SqlCommand comando = new SqlCommand();
        #endregion propiedades

        #region Constructor
        public Conexion()
        {
            string cadenaConexion = @"Server=127.0.0.1;DataBase=PuntoVentaAromas;User Id=sa;Password=2407;"; 
 
            //instaciamos el objeto cnx con la cadena de conexion
            cnx = new SqlConnection(cadenaConexion);
        }

        #endregion Constructor

        #region Destructor
        ~Conexion()
        {
            cnx = null;
        }
        #endregion Destructor

        #region AbrirConexion
        /// <summary>
        /// Abre la Conexion
        /// </summary>
        /// <returns>boolean</returns> 
        public bool AbrirConexion()
        {
            //Variable creada para saber el tipo de dato devuelto
            bool exito = true;

            try
            {
                cnx.Open();
            }
            catch (Exception ex)
            {
                exito = false;
            }
            return exito;
        }
        #endregion AbrirConexion

        #region CerrarConexion
        /// <summary>
        /// Cierra la Conexion
        /// </summary>
        /// <returns>boolean</returns>
        public bool CerrarConexion()
        {
            bool exito = true;
            try
            {
                cnx.Close();
            }
            catch (Exception)
            {
                exito = false;
            }
            return exito;
        }
        #endregion CerrarConexion   

        #region HacerSelect
        /// <summary>
        /// Realiza consultas de tipo "select"
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns>DataTable</returns>
        public DataTable HacerSelect(string consulta)
        {
            //Se instancia el objeto dtResultados
            DataTable dtResultados = new DataTable("Resultado");
            //se instancia un objeto Sqlcommand que recibe dos parametros
            //uno es una consulta y el otro es una conexion
            SqlCommand comando = new SqlCommand(consulta, cnx);
            //Se crea un objeto SqlDataAdapter y se le da como parametro
            //el objeto SqlCommand creado anteriormente
            SqlDataAdapter adapter = new SqlDataAdapter(comando);
            //se usa el objeto adapter para llenar la DataTable "dtResultado"
            //con el metodo ".Fill" que recibe como parametro el objeto
            //datTable a llenar
            adapter.Fill(dtResultados);

            //Se devuelve la dataTable dtResultados
            return dtResultados;
        }
        #endregion HacerSelect

        #region HacerHit
        /// <summary>
        /// Metodo que insertar,borrar,actualiza datos de la base de datos
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns>int</returns>
        public int HacerHit(string consulta)
        {
            int numeroRegistros = 0;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //instanciamos un objeto sqlcommand con los dos parametros que necesita
                    // que son una consulta y una conexion
                    comando = new SqlCommand(consulta, cnx);
                    try
                    {
                        //Ejecucion de la sentencia
                        numeroRegistros = comando.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        numeroRegistros = -1;
                    }

                    scope.Complete();
                }
            }
            catch (TransactionAbortedException)
            {
                numeroRegistros = -1;
            }
            catch (ApplicationException)
            {
                numeroRegistros = -1;
            }

            //si la consulta se llevo sin problemas esta devolvera 
            //el numero total de registros afectados que seran de 0 en adelante
            return numeroRegistros;
        }
        #endregion HIT

        #region HacerHitScalar
        /// <summary>
        /// Metodo que insertar,borrar,actualiza datos de la base de datos
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns>int</returns>
        /// Este hace hit pudiendo agregar consultas complejas de SQL
        /// 
        public int HacerHitScalar(string consulta)
        {
            int numeroRegistros = 0;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    //instanciamos un objeto sqlcommand con los dos parametros que necesita
                    // que son una consulta y una conexion
                    comando = new SqlCommand(consulta, cnx);

                    try
                    {
                        //Ejecucion de la sentencia
                        numeroRegistros = Convert.ToInt32(comando.ExecuteScalar());
                    }
                    catch (Exception)
                    {
                        numeroRegistros = -1;
                    }

                    scope.Complete();
                }
            }
            catch (TransactionAbortedException)
            {
                numeroRegistros = -1;
            }
            catch (ApplicationException)
            {
                numeroRegistros = -1;
            }

            //si la consulta se llevo sin problemas esta devolvera 
            //el numero total de registros afectados que seran de 0 en adelante
            return numeroRegistros;
        }
        #endregion HIT

        #region HacerHitScalarString
        /// <summary>
        /// Metodo que insertar,borrar,actualiza datos de la base de datos
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns>int</returns>
        /// Este hace hit pudiendo agregar consultas complejas de SQL
        /// 
        public string HacerHitScalarString(string consulta)
        {
            string jsonResponse = "";

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    //instanciamos un objeto sqlcommand con los dos parametros que necesita
                    // que son una consulta y una conexion
                    comando = new SqlCommand(consulta, cnx);

                    try
                    {
                        //Ejecucion de la sentencia
                        jsonResponse = comando.ExecuteScalar().ToString();
                    }
                    catch (Exception)
                    {
                        jsonResponse = "{\"codigoError\":3,\"objeto\":null,\"resultado\":false}";
                    }

                    scope.Complete();
                }
            }
            catch (TransactionAbortedException)
            {
                jsonResponse = "{\"codigoError\":3,\"objeto\":null,\"resultado\":false}";
            }
            catch (ApplicationException)
            {
                jsonResponse = "{\"codigoError\":3,\"objeto\":null,\"resultado\":false}";
            }

            //si la consulta se llevo sin problemas esta devolvera 
            //el numero total de registros afectados que seran de 0 en adelante
            return jsonResponse;
        }
        #endregion HIT        

        #region llmar parametros


        /// <summary>
        /// Metodo ejecuta un store prodecure, con los parametros
        /// </summary>
        /// <param name="consulta"></param>
        /// <param name="parameters"></param>
        /// <returns>int</returns>
        /// Este hace hit pudiendo agregar consultas complejas de SQL
        /// 
        public DataTable HacerProcedure(string consulta, List<KeyValuePair<string, string>> parameters)
        {
            //Se instancia el objeto dtResultados
            DataTable dtResultados = new DataTable("Resultado");

            //se instancia un objeto Sqlcommand que recibe dos parametros
            //uno es una consulta y el otro es una conexion

            comando = new SqlCommand(consulta, cnx);

            foreach (var item in parameters)
            {
                comando.Parameters.AddWithValue("@" + item.Key, item.Value);
            }
            comando.CommandType = CommandType.StoredProcedure;
            //Se crea un objeto SqlDataAdapter y se le da como parametro
            //el objeto SqlCommand creado anteriormente
            SqlDataReader dr = comando.ExecuteReader();
            //Se devuelve la dataTable dtResultados

            dtResultados.Load(dr);

            return dtResultados;
        }
        #endregion

        #region HACER HIT String
        /// <summary>
        ///  Metodo para consultar y que devulva un string como resultado
        ///  la respuesta puede ser un nombre de alguna columna, una operacion, etc.
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns></returns>
        public string hacerHitString(string consulta)
        {
            string result = "";

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    comando = new SqlCommand(consulta, cnx);

                    try
                    {
                        //Ejecucion de la sentencia
                        result = comando.ExecuteScalar().ToString();
                    }
                    catch (Exception)
                    {
                        result = "-1";
                    }

                    scope.Complete();
                }
            }
            catch (TransactionAbortedException)
            {
                result = "-1";
            }
            catch (ApplicationException)
            {
                result = "-1";
            }

            return result;
        }

        #endregion

        #region HacerSelectJSONPATH
        /// <summary>
        /// METODO PARA TRAER UN JSON DESDE SQL SERVER, DEVUELVE EL STRING QUE ES EL JSON
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns>string</returns>
        public string HacerSelectJSONPATH(string consulta)
        {
            //Se instancia el objeto dtResultados
            DataTable dtResultados = new DataTable("Resultado");
            //se instancia un objeto Sqlcommand que recibe dos parametros
            //uno es una consulta y el otro es una conexion
            SqlCommand comando = new SqlCommand(consulta, cnx);
            //Se crea un objeto SqlDataAdapter y se le da como parametro
            //el objeto SqlCommand creado anteriormente
            SqlDataAdapter adapter = new SqlDataAdapter(comando);
            //se usa el objeto adapter para llenar la DataTable "dtResultado"
            //con el metodo ".Fill" que recibe como parametro el objeto
            //datTable a llenar
            adapter.Fill(dtResultados);

            return string.Join("", dtResultados.AsEnumerable()
                                             .Select(x => x[0].ToString())
                                             .ToArray());
        }
        #endregion HacerSelect

        #region llmar parametros


        /// <summary>
        /// Metodo ejecuta un store prodecure, con los parametros
        /// </summary>
        /// <param name="consulta"></param>
        /// <param name="parameters"></param>
        /// <returns>int</returns>
        /// Este hace hit pudiendo agregar consultas complejas de SQL
        /// 
        public String callProcedureJson(string consulta, List<KeyValuePair<string, string>> parameters)
        {
            //Se instancia el objeto dtResultados
            DataTable dtResultados = new DataTable("Resultado");

            //se instancia un objeto Sqlcommand que recibe dos parametros
            //uno es una consulta y el otro es una conexion

            comando = new SqlCommand(consulta, cnx);

            foreach (var item in parameters)
            {
                comando.Parameters.AddWithValue("@" + item.Key, item.Value);
            }
            comando.CommandType = CommandType.StoredProcedure;
            //Se crea un objeto SqlDataAdapter y se le da como parametro
            //el objeto SqlCommand creado anteriormente
            SqlDataReader dr = comando.ExecuteReader();
            //Se devuelve la dataTable dtResultados

            dtResultados.Load(dr);

            return string.Join("", dtResultados.AsEnumerable()
                                            .Select(x => x[0].ToString())
                                            .ToArray());
        }
        #endregion

    }
}
