using System;
using FluentValidation;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ReleaseBoard.Messages.EventMessageHandlers;

namespace ReleaseBoard.Messages
{
    /// <summary>
    /// Конфигурация проекта.
    /// </summary>
    public static class StartupServiceCollectionExtensions
    {
        /// <summary>
        /// Регистриурет обработчиков и диспатчеров.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        public static void RegisterEventMessageHandlers(this IServiceCollection services)
        {
            services.AddScoped<IEventMessageHandler<NewBuildEvent>, NewBuildEventHandler>();
            services.AddScoped<IEventMessageHandler<DeleteBuildEvent>, DeleteBuildEventHandler>();
            services.AddScoped<IEventMessageHandler<UpdateBuildEvent>, UpdateBuildEventHandler>();

            services.AddSingleton<IEventMessageHandler<IEventMessage>, EventMessagesDispatcher>();

            services.AddSingleton<IHandlersRegistrator, HandlersRegistrator>();

            RegisterValidators(services);
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            services.Configure<ApiBehaviorOptions>(o =>
                o.InvalidModelStateResponseFactory = actionContext
                    => new BadRequestObjectResult(actionContext.ModelState));
        }
    }
}
