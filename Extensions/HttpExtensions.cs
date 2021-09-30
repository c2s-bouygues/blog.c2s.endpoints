using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Http.Extensions
{
    public static class HttpExtensions
    {
        public static string FromRoute(this HttpContext context, string name)
        {
            var routeValue = context.Request.RouteValues[name];

            if (routeValue == null)
                throw new ArgumentNullException(name);

            if (routeValue.GetType() != typeof(string))
                throw new ArgumentOutOfRangeException(name);

            return routeValue as string;
        }

        public static T FromRoute<T>(this HttpContext context, string name)
            where T : struct
        {
            var routeValue = context.Request.RouteValues[name];

            return ConvertTo<T>(routeValue, name) ?? throw new ArgumentNullException(name);
        }

        public static string? FromQuery(this HttpContext context, string name)
        {
            var stringValues = context.Request.Query[name];

            return !StringValues.IsNullOrEmpty(stringValues)
                ? stringValues.ToString()
                : null;
        }


        public static T? FromQuery<T>(this HttpContext context, string name)
            where T : struct
        {
            var stringValues = context.Request.Query[name];

            return !StringValues.IsNullOrEmpty(stringValues)
                ? ConvertTo<T>(stringValues.ToString(), name)
                : null;
        }

        public static async Task<T> FromBody<T>(this HttpContext context)
        {
            using (var reader
                  = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
            throw new ArgumentNullException("request");
        }

        public static T? ConvertTo<T>(object? value, string name)
            where T : struct
        {
            if (value == null)
                return null;

            T? result;
            try
            {
                result = (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
            }
            catch
            {
                throw new ArgumentOutOfRangeException(name);
            }

            return result;
        }

        public static Task OK<T>(this HttpContext context, T result)
            => context.ReturnJSON(result);

        public static Task Created<T>(this HttpContext context, T id, string? location = null)
        {
            context.Response.Headers[HeaderNames.Location] = location ?? $"{context.Request.Path}{id}";

            return context.ReturnJSON(id, HttpStatusCode.Created);
        }

        public static void NotFound(this HttpContext context)
            => context.Response.StatusCode = (int)HttpStatusCode.NotFound;

        public static async Task ReturnJSON<T>(this HttpContext context, T result,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            context.Response.StatusCode = (int)statusCode;

            if (result == null)
                return;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        public static Task KO(this HttpContext context, Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => context.ReturnJSON(exception, statusCode);

        public static async Task ReturnJSON(this HttpContext context, Exception exception,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            context.Response.StatusCode = (int)statusCode;
            if (exception == null)
                await context.Response.WriteAsync("Erreur inconnue au bataillon");
            else
                await context.Response.WriteAsync(exception.Message);
        }
    }
}
