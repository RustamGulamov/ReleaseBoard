using System;
using Microsoft.Extensions.DependencyInjection;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Builds.StateChangeChecker;
using ReleaseBoard.Domain.Builds.TransferRules;
using ReleaseBoard.Domain.Builds.TransferRules.Interfaces;
using ReleaseBoard.Domain.Builds.TransferRules.Repository;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.Services;

namespace ReleaseBoard.Domain
{
    /// <summary>
    /// Extension методы <see cref="IServiceCollection"/> для Монго.
    /// </summary>
    public static class StartupServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует конфиги.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        public static void RegisterDomain(this IServiceCollection services)
        {
            services.AddScoped<IBuildLifeCycleStateMapper, BuildLifeCycleStateMapper>();
            services.AddScoped<IAggregateRepository, AggregateRepository>();

            services.AddScoped<IEventStore, EventStore.EventStore>();
            services.AddSingleton<ICollectionsComparer, CollectionsComparer>();
            services.AddSingleton<ITransferRuleRepository, TransferRuleRepository>();

            services.AddSingleton<IBuildStateTransferRule, ReleaseStateTransferRule>();
            services.AddSingleton<IBuildStateTransferRule, ReleaseCandidateStateTransferRule>();
            services.AddSingleton<IBuildStateTransferRule, CertifiedStateTransferRule>();
            
            services.AddScoped<IBuildStateChangeChecker, BuildStateChangeChecker>();
        }
    }
}
