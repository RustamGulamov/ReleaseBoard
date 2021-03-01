using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Prometheus;
using ReleaseBoard.Application.CommandHandlers;
using ReleaseBoard.Application.Factories;
using ReleaseBoard.Domain;
using ReleaseBoard.Domain.Builds.Settings;
using ReleaseBoard.Infrastructure;
using ReleaseBoard.Web.Authorization;
using ReleaseBoard.Web.Authorization.Security;
using ReleaseBoard.Web.Enums;
using ReleaseBoard.Web.Factories;
using ReleaseBoard.Web.HostedServices;
using ReleaseBoard.Web.HostedServices.StartupTasks;
using ReleaseBoard.Web.Middlewares;
using ReleaseBoard.Web.Providers;
using ReleaseBoard.Web.Services;
using ReleaseBoard.Web.Services.Interfaces;
using ReleaseBoard.Web.Settings;
using ReleaseBoard.Web.SignalR;

namespace ReleaseBoard.Web
{
    /// <summary>
    /// Класс конфигурации веб-приложения.
    /// </summary>
    public class Startup
    {
        private const string CorsPolicyName = "CorsPolicy";
        private const string ServiceName = "ReleaseBoard";

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="configuration">Конфигурация.</param>
        /// <param name="env">Параметры окружения.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Окружение.
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Конфигурирует pipeline запросов.
        /// </summary>
        /// <param name="app">Строитель приложения.</param>
        /// <param name="env">Параметры окружения.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ServiceName} API v1"); });
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpMetrics();
            app.UseMiddleware<ValidationExceptionsHandler>();
            app.UseMiddleware<HttpErrorResponseExceptionsHandler>();
            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notifications");
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireAnonymousAuthorizationFilter() } });
            app.UseHttpsRedirection();
        }

        /// <summary>
        /// Конфигурирует все сервисы приложения.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.BindConfig<BuildSettings>());

            services.RegisterEventMessageHandlers();
            services.RegisterInfrastructure(Configuration);
            services.RegisterDomain();

            RegisterFactories(services);
            RegisterCorsPolicy(services);
            RegisterAuthentication(services);
            RegisterAuthorization(services);
            RegisterServices(services);

            var assemblies = GetReleaseBoardAssemblies();
            services.AddMediatR(assemblies);
            services.AddAutoMapper(assemblies);
            services.AddValidatorsFromAssemblies(assemblies);
            
            services.AddHostedService<ReleaseBoardHostedService>();

            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    fv.ImplicitlyValidateChildProperties = true;
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                });

            services.AddSwaggerGen();
            services.AddHttpContextAccessor();
            services
                .AddSignalR()
                .AddNewtonsoftJsonProtocol();

            services.AddHealthChecks()
                .ForwardToPrometheus();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.BindConfig<BuildStorageUrlSettings>());
            services.AddScoped<IBuildStorageUrlProvider, BuildStorageUrlProvider>();
        }

        private void RegisterAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.CanManageDistributions, policy => policy.RequireRole(Role.Admin.ToString("G")));
                options.AddPolicy(
                    PolicyNames.CanAccessBuildSyncApi,
                    p => p.RequireAssertion(ctx =>
                    {
                        IEnumerable<Claim> claims = ctx.User.FindAll("scope");

                        return claims.Any(c => c.Value == "build-sync-api");
                    })
                );
            });
        }

        private void RegisterFactories(IServiceCollection services)
        {
            services.AddScoped<IDistributionCommandMapper, DistributionCommandMapper>();
            services.AddScoped<IDistributionBuildsSpecificationFactory, DistributionBuildsSpecificationFactory>();
        }

        private void RegisterCorsPolicy(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowed(x => true));
            });
        }

        private void RegisterAuthentication(IServiceCollection services)
        {
            services.AddScoped<IAuthorizedUserProvider, AuthorizedUserProvider>();
            var authOptions = Configuration
                .GetSection(nameof(AuthenticationOptions))
                .Get<AuthenticationOptions>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtClaimTypes.Name, ClaimTypes.Name);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtClaimTypes.Role, ClaimTypes.Role);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add(JwtClaimTypes.Subject, ClaimTypes.Sid);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    bearerOptions =>
                    {
                        bearerOptions.Authority = authOptions.Authority;
                        bearerOptions.Audience = authOptions.Audience;

                        bearerOptions.RequireHttpsMetadata = Environment.IsProduction();

                        bearerOptions.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                StringValues accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        private Assembly[] GetReleaseBoardAssemblies()
        {
            return
                AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Where(x => x.FullName.StartsWith($"{ServiceName}"))
                    .Append(typeof(CommandHandlerBase<>).GetTypeInfo().Assembly)
                    .Distinct()
                    .ToArray();
        }
    }
}
