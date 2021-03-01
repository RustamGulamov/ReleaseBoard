using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.BuildSync.Responses;
using ReleaseBoard.Common.Contracts.Common;
using ReleaseBoard.Common.Contracts.Extensions;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Builds.Exceptions;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Builds.StateChangeChecker;
using ReleaseBoard.Domain.Core.Exceptions;
using ReleaseBoard.Domain.Repositories;

namespace ReleaseBoard.Application.CommandHandlers.Builds.ChangeState
{
    /// <inheritdoc />
    public class ChangeBuildStateHandler : UpdateBuildBaseHandler<ChangeBuildState>
    {
        private readonly IBuildStateChangeChecker changeChecker;
        private readonly IBuildLifeCycleStateMapper buildLifeCycleStateMapper;
        private readonly IBuildSyncService buildSyncService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="changeChecker"><see cref="IBuildStateChangeChecker"/>.</param>
        /// <param name="buildLifeCycleStateMapper"><see cref="IBuildLifeCycleStateMapper"/>.</param>
        /// <param name="buildSyncService"><see cref="IBuildSyncService"/>.</param>
        public ChangeBuildStateHandler(
            IAggregateRepository aggregateRepository,
            ILogger<ChangeBuildStateHandler> logger,
            IBuildStateChangeChecker changeChecker,
            IBuildLifeCycleStateMapper buildLifeCycleStateMapper,
            IBuildSyncService buildSyncService
        ) : base(aggregateRepository, logger)
        {
            this.changeChecker = changeChecker;
            this.buildLifeCycleStateMapper = buildLifeCycleStateMapper;
            this.buildSyncService = buildSyncService;
        }

        /// <inheritdoc />
        protected override async Task UpdateBuild(Build build, ChangeBuildState command)
        {
            StateChangeCheckResult result = await changeChecker.Check(build, command.NewState);
            
            if (!result.CanChange)
            {
                throw new ValidationException(new BuildTransferRuleCheckValidationError()
                {
                    ErrorName = "StateTranslateRuleViolation",
                    From = build.LifeCycleState,
                    To = command.NewState,
                    Rules = result.UnMatchRuleNames,
                });
            }

            List<string> newSuffixes = GetNewSuffixes(build.Suffixes.ToArray(), build.LifeCycleState, command.NewState).ToList();
            
            Logger.LogDebug($"Try change state of build {build}. NewState: {command.NewState}. NewSuffixes: {newSuffixes.JoinWith(",")}");
            
            IResponseMessage response = 
                await buildSyncService.ChangeSuffix(build.Location, build.SourceType, newSuffixes, build.Suffixes.ToList());

            switch (response)
            {
                case ChangeSuffixesResponse { IsSuccessful: true }:
                    build.UpdateLifeCycleState(command.NewState, DateTime.Now, command.Comment, command.User.Sid);
                    return;
                case ErrorResponseMessage errorResponseMessage:
                    HandleErrorResponse(errorResponseMessage, build, command.NewState);
                    return;
            }
        }
    }
}
