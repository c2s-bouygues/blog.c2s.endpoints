using c2s.endpoints.RequestDelegates.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace c2s.endpoints.Routes
{
    public static class APIRoutes
    {
        private const string _apiName = "api/environments";

        internal static IEndpointRouteBuilder UseEnvironmentEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet($"/{_apiName}", GetUsersDelegate.Delegate);

            endpoints.MapGet($"/{_apiName}/{{id}}", GetUserByIdDelegate.Delegate);

            endpoints.MapPost($"/{_apiName}", PostUserDelegate.Delegate);

            return endpoints;
        }
    }
}
