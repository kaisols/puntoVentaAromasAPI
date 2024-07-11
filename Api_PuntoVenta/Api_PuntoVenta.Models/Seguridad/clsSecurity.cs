using Microsoft.IdentityModel.Tokens;
using Api_PuntoVenta.Models;
using System;
using System.Configuration; 
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web; 
using System.IdentityModel.Tokens.Jwt;

namespace Api_PuntoVenta.Models.Seguridad
{ 
    public static class clsSecurity 
    {

        public static string encodePassword(string password)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] inputBytes = (new UnicodeEncoding()).GetBytes(password);
            byte[] hash = sha1.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }

        public static string GenerateToken(Usuario _userData)
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _userData.correo),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                          new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                        new Claim("UserId", _userData.id.ToString()),
                        new Claim("DisplayName", _userData.ToString()),
                        new Claim("UserName", _userData.ToString()),
                        new Claim("Email", _userData.correo)
                    }; 


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clsDatos.JWT_SECRET_KEY));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
               clsDatos.JWT_ISSUER_TOKEN,
                clsDatos.JWT_AUDIENCE_TOKEN,
                claims,
                expires: DateTime.UtcNow.AddMinutes(clsDatos.JWT_EXPIRE_MINUTES),
                signingCredentials: signIn);


            String _Token = new JwtSecurityTokenHandler().WriteToken(token);

            return _Token;
        }
         

        public static int ValidateToken(string authToken)
        {
            try
            {
                SecurityToken validatedToken;
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);

                return 10;

            }
            catch (SecurityTokenValidationException)
            {
                return 11;
                //statusCode = ;
            }
            catch (System.ArgumentNullException)
            {
                return 12;
            }
            catch (Exception)
            {
                return 13;
            }
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            var secretKey = clsDatos.JWT_SECRET_KEY;
            var audienceToken = clsDatos.JWT_AUDIENCE_TOKEN;
            var issuerToken = clsDatos.JWT_ISSUER_TOKEN;
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

            return new TokenValidationParameters()
            {
                ValidAudience = audienceToken,
                ValidIssuer = issuerToken,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey
            };
        }

    }
}
