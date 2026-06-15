using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartEvent.Shared.Logging.Extensions;
using System.Text;
using Yarp.ReverseProxy.Transforms;
using YarpApiGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Serilog adding
builder.AddSmartEventLogging("ApiGateway");

// YARP Reverse Proxy adding
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(context =>
    {
        // Add a transformation to have token in other services
        context.AddRequestTransform(transformContext =>
        {
            // Copy the Authorization header from the original request
            var authHeader = transformContext.HttpContext.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                transformContext.ProxyRequest.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        authHeader.Replace("Bearer ", ""));
            }

            // cookies copy
            foreach (var cookie in transformContext.HttpContext.Request.Cookies)
            {
                transformContext.ProxyRequest.Headers.Add("Cookie", $"{cookie.Key}={cookie.Value}");
            }

            return ValueTask.CompletedTask;
        });
    });

// adding CORS for frontend
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSmartEventLogging();

// Middleware pipeline
app.UseCors("AllowFrontend");

// Token Refresh adding before YARP
app.UseTokenRefresh();

// YARP mapping
app.MapReverseProxy();

app.Run();