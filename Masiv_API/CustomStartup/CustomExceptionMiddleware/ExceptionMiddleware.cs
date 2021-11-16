using Masiv_API.Models.MGenericReponse;
using Masiv_API.Services.Logger;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Masiv_API.CustomStartup.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("-------------------------------------------------------------------------------------{0} Error: {1}{0}", Environment.NewLine, ex));
                await HandleExceptionAsync(httpContext);
            }
        }

        private Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(new GenericResponse<bool>()
            {
                IsSuccesful = false,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Internal Server Error 500"
            }.ToString());
        }
    }
}
