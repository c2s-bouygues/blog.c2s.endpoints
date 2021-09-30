
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace c2s.endpoints.RequestDelegates.Environment
{
    public class GetUsersDelegate
    {
        public static RequestDelegate Delegate => async context =>
        {
            var serviceProvider = context.RequestServices;
            var logger = serviceProvider.GetService<ILogger<GetUsersDelegate>>();
            try
            {
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
