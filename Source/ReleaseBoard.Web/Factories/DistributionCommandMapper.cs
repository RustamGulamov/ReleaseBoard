using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.TransferRules.Interfaces;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.DistributionModels;
using ReleaseBoard.Web.Providers;

namespace ReleaseBoard.Web.Factories
{
    /// <summary>
    /// Маппер модели <see cref="DistributionModel"/> в команду <see cref="CreateDistribution"/>.
    /// </summary>
    public class DistributionCommandMapper : IDistributionCommandMapper
    {
        private readonly IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternRepository;
        private readonly ILighthouseServiceApi lighthouseServiceApi;
        private readonly IEnumerable<IBuildStateTransferRule> transferRules;
        private readonly IAuthorizedUserProvider authorizedUserProvider;
        private readonly IMapper mapper;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildMatchPatternRepository"><see cref="IReadOnlyRepository{BuildMatchPatternReadModel}"/>.</param>
        /// <param name="lighthouseServiceApi"><see cref="ILighthouseServiceApi"/>.</param>
        /// <param name="transferRules"><see cref="IEnumerable{IBuildStateTransferRule}"/>.</param>
        /// <param name="authorizedUserProvider"><see cref="IAuthorizedUserProvider"/>.</param>
        /// <param name="mapper">Маппер.</param>
        public DistributionCommandMapper(
            IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternRepository,
            ILighthouseServiceApi lighthouseServiceApi,
            IEnumerable<IBuildStateTransferRule> transferRules,
            IAuthorizedUserProvider authorizedUserProvider,
            IMapper mapper)
        {
            this.buildMatchPatternRepository = buildMatchPatternRepository;
            this.lighthouseServiceApi = lighthouseServiceApi;
            this.transferRules = transferRules;
            this.authorizedUserProvider = authorizedUserProvider;
            this.mapper = mapper;
        }

        /// <summary>
        /// Преобразует ui модель в команду.
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <returns>Комманда.</returns>
        public async Task<UpdateDistribution> MapToUpdateCommand(DistributionModel model) =>
            new UpdateDistribution
            {
                Name = model.Name, 
                Id = model.Id,
                Owners = await CreateUsers(model.OwnersSids),
                ActionUser = mapper.Map<User>(authorizedUserProvider.Get()),
                BuildBindings = await MapToBuildBindings(model.BuildBindings),
                ProjectBindings = await MapToProjectBindings(model.ProjectBindings),
                AvailableLifeCycles = model.AvailableLifeCycles,
                LifeCycleStateRules = GetAllLifeCycleStateRules(model.AvailableLifeCycles)
            };

        /// <summary>
        /// Преобразует ui модель в команду.
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <returns>Комманда.</returns>
        public async Task<CreateDistribution> MapToCreateCommand(DistributionModel model) =>
            new CreateDistribution
            {
                Name = model.Name, 
                Owners = await CreateUsers(model.OwnersSids),
                ActionUser = mapper.Map<User>(authorizedUserProvider.Get()),
                AvailableLifeCycles = model.AvailableLifeCycles,
                LifeCycleStateRules = GetAllLifeCycleStateRules(model.AvailableLifeCycles),
                ProjectBindings = await MapToProjectBindings(model.ProjectBindings),
                BuildBindings = await MapToBuildBindings(model.BuildBindings)
            };
        
        private async Task<BuildsBinding[]> MapToBuildBindings(BuildBindingModel[] bindingModels)
        {
            var result = new List<BuildsBinding>();
            foreach (BuildBindingModel bindingModel in bindingModels)
            {
                BuildMatchPatternReadModel patternReadModel = await buildMatchPatternRepository.Query(x => x.Id == bindingModel.PatternId);
                var pattern = mapper.Map<BuildMatchPattern>(patternReadModel);
                result.Add(new BuildsBinding(bindingModel.Path, pattern, bindingModel.SourceType));
            }

            return result.ToArray();
        }

        private async Task<ProjectBinding[]> MapToProjectBindings(ProjectBindingModel[] bindings)
        {
            var result = new List<ProjectBinding>();
            foreach (ProjectBindingModel modelProjectBinding in bindings)
            {
                var projectDto = await lighthouseServiceApi.GetProjectById(modelProjectBinding.ProjectId);
                result.Add(
                    new ProjectBinding(modelProjectBinding.MaskProjectVersion,
                        new Project(projectDto.ExternalId, projectDto.ShortName, projectDto.Name)));
            }

            return result.ToArray();
        }

        private async Task<List<User>> CreateUsers(IList<string> usersSids)
        {
            var result = new List<User>();
            foreach (string sid in usersSids)
            {
                result.Add(await CreateUser(sid));
            }
            return result;
        }

        private async Task<User> CreateUser(string userSid)
        {
            var lighthouseUser = await lighthouseServiceApi.GetUserBySid(userSid);
            if (lighthouseUser == null)
            {
                throw new InvalidOperationException("User not found in lighthouse service");
            }

            return lighthouseUser;
        }

        private string[] GetAllLifeCycleStateRules(LifeCycleState[] availableLifeCycles)
        {
            return transferRules
                .Where(x => availableLifeCycles.Contains(x.DestinationState))
                .Select(x => x.Name)
                .ToArray();
        }
    }
}
