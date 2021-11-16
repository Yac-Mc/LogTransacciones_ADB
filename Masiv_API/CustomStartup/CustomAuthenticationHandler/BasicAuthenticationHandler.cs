using Masiv_API.Services.Logger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Masiv_API.CustomStartup.CustomAuthenticationHandler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public IServiceProvider _serviceProvider;

        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider serviceProvider)
            : base(options, logger, encoder, clock)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string header = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(header))
            {
                return Task.FromResult(AuthenticateResult.Fail("Username and Passwor is null"));
            }
            bool isAuthenticate = true; //ValidateUser(header);
            GenerateLog(Request);
            if (!isAuthenticate)
            {
                return Task.FromResult(AuthenticateResult.Fail($"User not authorize: {header}"));
            }
            var claims = new[] { new Claim("Basic", header) };
            var identity = new ClaimsIdentity(claims, nameof(BasicAuthenticationHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);
            
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private void GenerateLog(HttpRequest request)
        {
            if (request.Path.Value.Contains("createroulette") || request.Path.Value.Contains("closedbet"))
            {
                string msg = request.Path.Value.Contains("createroulette") ? "Ruleta creada por:" : $"Ruleta con {request.QueryString.Value.Remove(0,1)} cerrada por:";
                var loggerInfo = new LoggerManager();
                string header = Request.Headers["Authorization"];
                string encodedUsernamePassword = header.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                loggerInfo.LogInfo(
                        string.Format("-------------------------------------------------------------------------------------{0} {1} {2} -- Fecha: {3} - hora: {4}{0}",
                            Environment.NewLine,
                            msg,
                            usernamePassword.Split(':')[0],
                            DateTime.Now.ToString("dd/MMM/yyyy"),
                            DateTime.Now.ToString("HH:MM")
                        ));
            }
        }

        private bool ValidateUser(string header)
        {
            string encodedUsernamePassword = header.Substring("Basic ".Length).Trim();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
            string username = Environment.GetEnvironmentVariable("Username");
            string password = Environment.GetEnvironmentVariable("Password");
            if (usernamePassword.Split(':')[0].Equals(username) && usernamePassword.Split(':')[1].Equals(password))
            {
                return true;
            }

            return false;
        }
    }

    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public static class SchemesNameConst
    {
        public const string BasicAuthenticationDefaultScheme = "TokenAuthenticationScheme";
    }
}
