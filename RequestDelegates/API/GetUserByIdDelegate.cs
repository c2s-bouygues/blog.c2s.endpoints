using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;

namespace blog.c2s.endpoints.RequestDelegates.Environment
{
    public class GetUserByIdDelegate
    {
        public static RequestDelegate Delegate => async context =>
        {
            var serviceProvider = context.RequestServices;
            var logger = serviceProvider.GetService<ILogger<GetUserByIdDelegate>>();
            try
            {
                // On récupère l'Id depuis la route
                var stringValues = context.Request.RouteValues["id"].ToString();
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                logger.LogTrace(ex.StackTrace);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }
        };
    }
}
