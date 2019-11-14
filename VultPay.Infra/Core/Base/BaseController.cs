using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VultPay.Infra.Core.Services;
using VultPay.Domain.ViewModel;
using VultPay.Domain.Enums;
using Salaros.Configuration;
using System.Globalization;

namespace VultPay.Infra.Core.Base
{
    public class BaseController : ControllerBase
    {
        internal ConfigParser ConfigGlobal { get; private set; }

        protected SecurityKey SecurityKey { get; set; }

        protected JwtSecurityTokenHandler TokenHandler { get; set; }

        protected IConfiguration Configuration { get; set; }

        public BaseController()
        {

        }

        protected BaseController(IConfiguration configuration)
        {
            //
            Configuration = configuration;
            //
            ConfigGlobal = new ConfigParser(Configuration.GetSection("GlobalConfiguration").Value, new ConfigParserSettings
            {
                MultiLineValues = MultiLineValues.Simple | MultiLineValues.AllowValuelessKeys | MultiLineValues.QuoteDelimitedValues | MultiLineValues.AllowEmptyTopSection,
                Culture = new CultureInfo("en-US")
            });
            //
            SecurityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(ConfigGlobal.GetValue("Application", "AppSecretKey")));
            TokenHandler = new JwtSecurityTokenHandler();
        }

        protected ApplicationDetails GetSubscriberContext(Microsoft.AspNetCore.Http.HttpContext userContext)
        {
            return GetSubscriberContext(userContext.User);
        }

        public ApplicationDetails GetSubscriberContext(ClaimsPrincipal userContext)
        {
            ClaimsIdentity claims = new ClaimsIdentity(userContext.Claims);

            if (claims.Claims.Count() > 0)
            {

                ApplicationDetails transaction = new ApplicationDetails
                {
                    AppId = int.Parse(claims.Claims.Where(c => c.Type == ClaimTypes.PrimarySid).First().Value),
                    AppUid = Guid.Parse(claims.Claims.Where(c => c.Type == ClaimTypes.SerialNumber).First().Value),
                    AppDescription = claims.Claims.Where(c => c.Type == ClaimTypes.UserData).First().Value,
                    Owner = (Owner)int.Parse(claims.Claims.Where(c => c.Type == ClaimTypes.Role).First().Value),
                    AccountStatus = (AccountStatus)int.Parse(claims.Claims.Where(c => c.Type == "AccountStatus").First().Value),
                };

                if (transaction.AppId == 0)
                {
                    throw new OperationCanceledException();
                }

                return transaction;
            }

            return null;
        }

        protected SecurityTokenResult GetSubscribersToken(ApplicationDetails appAuth)
        {
            try
            {
                SecurityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(ConfigGlobal.GetValue("Application", "AppSecretKey")));
                TokenHandler = new JwtSecurityTokenHandler();

                if (appAuth != null &&
                    appAuth.AppId > 0)
                {
                    ///Criamos uma identidade
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity();

                    ///Claim de acesso
                    Collection<Claim> claims = new Collection<Claim>
                    {
                        new Claim(ClaimTypes.PrimarySid, appAuth.AppId.ToString()),
                        new Claim(ClaimTypes.SerialNumber, appAuth.AppUid.ToString()),
                        new Claim(ClaimTypes.UserData, appAuth.AppDescription.ToString()),
                        new Claim(ClaimTypes.Role,  ((int)appAuth.Owner).ToString()),
                        new Claim("AccountStatus", ((int)appAuth.AccountStatus).ToString()),
                    };

                    ///Configura os Token
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = ConfigGlobal.GetValue("Application", "Issuer"),
                        Subject = claimsIdentity,
                        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest)
                    };

                    ///Gera o Token e retorna a string
                    SecurityToken token = TokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = TokenHandler.WriteToken(token);

                    ///Retorna
                    return new SecurityTokenResult
                    {
                        Create = DateTime.UtcNow,
                        Token = tokenString,
                        Expires = DateTime.UtcNow.AddHours(1),
                    };

                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
