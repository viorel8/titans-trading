using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Duende.IdentityServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedCore.Configuration;

namespace IdentityServer.Configuration
{
    public static partial class ServiceExtension
    {
        public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection serviceCollection,
        IWebHostEnvironment env,
        IConfiguration cfg)
        {

            List<string> allowed_origins = new List<string>();

            if (env.IsDevelopment() && !cfg.IsTyeEnviroment())
            {
                allowed_origins.AddRange(new string[]{
                    "https://localhost:5001",
                    "https://localhost:5015",
                    "https://localhost:5021",
                    "https://localhost:5022",
                    "https://localhost:5070",
                    "https://localhost",
                    "host.docker.internal",
                    "http://localhost"
                });
            }

            var host_uri = cfg.GetHostUrl();

            if (!string.IsNullOrWhiteSpace(host_uri))
            {
                allowed_origins.Add(host_uri);
            }

            var identity_uri = cfg.GetIdentityServerUri();

            if (!string.IsNullOrWhiteSpace(identity_uri))
            {
                allowed_origins.Add(identity_uri);
            }

            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("cors_policy", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    //------------------------------------
                    policy.WithOrigins(allowed_origins.ToArray());
                    //policy.AllowAnyOrigin()
                    //------------------------------------
                    policy.AllowCredentials();
                    policy.SetPreflightMaxAge(TimeSpan.FromSeconds(10000));
                });
            });

            // This is IdentityServer part
            serviceCollection.AddSingleton<ICorsPolicyService>((container) =>
            {
                var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();

                return new DefaultCorsPolicyService(logger)
                {
                    AllowedOrigins = allowed_origins
                    //AllowAll = true
                };
            });

            return serviceCollection;

        }
    }
}