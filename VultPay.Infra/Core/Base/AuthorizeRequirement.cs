using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace VultPay.Infra.Core.Base
{
    public class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public IServiceProvider ServiceProvider { get; set; }

        public TokenAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider serviceProvider)
            : base(options, logger, encoder, clock)
        {
            ServiceProvider = serviceProvider;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (Request.Path.HasValue && Request.Path.Value.ToUpper().Contains("/PUBLIC/"))
                {
                    return Task.FromResult(AuthenticateResult.NoResult());
                }

                //Token de autenticação
                var tokenString = Request.Headers["Authorization"].ToString();

                if (tokenString.Contains("Bearer"))
                {
                    //Manipulador do token
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                    ///Remove a string inválida
                    tokenString = tokenString.Replace("Bearer", "").Trim();

                    ///Parametros de validação
                    var validationParameters = new TokenValidationParameters()
                    {
                        //Emissor de confiança
                        ValidIssuer = ServiceProvider.GetService<IConfiguration>().GetSection("Issuer").Value,
                        //Chave privada simétrica para validação
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(ServiceProvider.GetService<IConfiguration>().GetSection("AppSecretKey").Value)),
                        //Não valida a origem
                        ValidateAudience = false,
                        //Delegate que valida a assinatura do token
                        SignatureValidator = delegate (string _token, TokenValidationParameters parameters)
                        {
                            //Pega a chave privada do app
                            var clientSecret = ServiceProvider.GetService<IConfiguration>().GetSection("AppSecretKey").Value;
                            //Inicia o objeto Jwt
                            var jwt = new JwtSecurityToken(_token);
                            //Calcula o hash da chave privada
                            var hmac = new HMACSHA256(Encoding.Default.GetBytes(clientSecret));
                            //Obtem as credenciais
                            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(hmac.Key), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
                            //Obtem a classe com os dados da assinatura
                            var signKey = signingCredentials.Key as SymmetricSecurityKey;
                            //Token JWT
                            var encodedData = jwt.EncodedHeader + "." + jwt.EncodedPayload;
                            //Gera uma assinatura
                            var compiledSignature = Encode(encodedData, signKey.Key);

                            //Validar a assinatura jwt do cabeçalho e compara com a do token
                            if (compiledSignature != jwt.RawSignature)
                            {
                                throw new Exception("Token signature validation failed.");
                            }

                            return jwt;
                        }
                    };

                    ///Valida o token
                    tokenHandler.ValidateToken(tokenString, validationParameters, out SecurityToken validatedToken);

                    //
                    if (validatedToken != null)
                    {
                        //Criamos uma identidade
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new JwtSecurityToken(tokenString).Claims) { Label = "Principal" };
                        //Cria o ticket de autenticação
                        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                }

                //Por padrão aborta a requisição...
                Request.HttpContext.Abort();
                return null;
            }
            catch
            {
                Request.HttpContext.Abort();
                return null;
            }
        }

        string Encode(string input, byte[] key)
        {
            HMACSHA256 myhmacsha = new HMACSHA256(key);
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            byte[] hashValue = myhmacsha.ComputeHash(stream);
            return Base64UrlEncoder.Encode(hashValue);
        }
    }

    public class AuthorizeRequirement : IAuthorizationRequirement
    {
        public Domain.Enums.Owner Owner { get; private set; }

        public AuthorizeRequirement(Domain.Enums.Owner owner)
        {
            Owner = owner;
        }
    }

    public class AuthorizeHandler : AuthorizationHandler<AuthorizeRequirement>
    {
        /// <summary>
        /// Middleware usado para validação de requisições nos módulos do sistema
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeRequirement requirement)
        {
            try
            {

                ///Criamos uma identidade
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(context.User.Claims) { Label = "Principal" };

                ///Busca as permissões
                if (claimsIdentity.HasClaim(c => c.Type.ToLower() == "role"))
                {
                    ///Se o usuário for um Influencer permite acesso completo...
                    if (IsAuthorized(context, requirement))
                    {
                        ///Adicionamos ao contexto da aplicação
                        ((ActionContext)context.Resource).HttpContext.User.AddIdentity(claimsIdentity);
                        context.Succeed(requirement);
                    }
                    //Por padrão nega acesso ao sistema
                    else
                    {
                        context.Fail();
                    }
                }
                else
                {
                    context.Fail();
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                context.Fail();
                return Task.FromException(ex);
            }
        }

        bool IsAuthorized(AuthorizationHandlerContext context, AuthorizeRequirement requirement)
        {
            var userContext = new BaseController().GetSubscriberContext(context.User);
            return requirement.Owner == userContext.Owner;
        }
    }
}