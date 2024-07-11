using Newtonsoft.Json;

namespace Api_PuntoVenta.Models.Seguridad
{
    public static class clsDatos
    {
        public static string JWT_SECRET_KEY = "S1ST3M4TICKETES_20201231290asdklmnasd1-0";
        public static string JWT_AUDIENCE_TOKEN = "https://localhost/";
        public static string JWT_ISSUER_TOKEN = "https://localhost/";

        /// <summary>
        /// EXPIRA EN 7 HORAS
        /// </summary>
        public static int JWT_EXPIRE_MINUTES = (int)TimeSpan.FromHours(18).TotalMinutes;

        public static JsonSerializerSettings Jsonsettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };


        public static string RemoveCharacters(String input)
        {
            string output = "";

            output = input.Replace("'", "")
                          .Replace("Select", "")
                          .Replace("Select", "")
                          .Replace("CREATE", "")
                          .Replace("DELETE", "")
                          .Replace("DROP", "")
                          .Replace("UPDATE", "")
                          .Replace("ALTER", "")
                          .Replace("INSERT", "")
                          .Replace("*", "")
                          .Replace("FROM", "")
                          .Replace("&", "")
                          .Replace("<>", "");

            return output;
        }


    }
}
