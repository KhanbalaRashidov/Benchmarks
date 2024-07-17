// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Benchmarks.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Benchmarks.Middleware
{
    public class PlaintextMiddleware(RequestDelegate next)
    {
        private static readonly PathString _path = new PathString(Scenarios.GetPath(s => s.Plaintext));
        private static readonly byte[] _helloWorldPayload = Encoding.UTF8.GetBytes("Hello, World!");

        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path, StringComparison.Ordinal))
            {
                // Parse the query string if it exists to benchmark query parsing
                if (httpContext.Request.QueryString.HasValue)
                {
                    var _ = httpContext.Request.Query;
                }

                return WriteResponse(httpContext.Response);
            }

            return _next(httpContext);
        }

        public static Task WriteResponse(HttpResponse response)
        {
            var payloadLength = _helloWorldPayload.Length;
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            response.ContentLength = payloadLength;
            return response.Body.WriteAsync(_helloWorldPayload, 0, payloadLength);
        }
    }

    public static class PlaintextMiddlewareExtensions
    {
        public static IApplicationBuilder UsePlainText(this IApplicationBuilder builder) =>
            builder.UseMiddleware<PlaintextMiddleware>();
    }
}
