using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.DistributionModels;

namespace ReleaseBoard.Web.Validators.ModelValidators
{
    /// <inheritdoc />
    public partial class DistributionModelValidator : AbstractValidator<DistributionModel>
    {
        private const string DistributionNameShouldBeUniqueMessage = "Название дистрибутива должно быть уникальным";

        private readonly IReadOnlyRepository<DistributionReadModel> distributionsRepository;
        private readonly IReadOnlyRepository<BuildReadModel> buildsRepository;
        private readonly ILighthouseServiceApi lighthouseService;
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="distributionsRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
        /// <param name="buildsRepository"><see cref="IReadOnlyRepository{BuildBindingReadModel}"/>.</param>
        /// <param name="lighthouseService"><see cref="ILighthouseServiceApi"/>.</param>
        public DistributionModelValidator(
            IReadOnlyRepository<DistributionReadModel> distributionsRepository,
            IReadOnlyRepository<BuildReadModel> buildsRepository,
            ILighthouseServiceApi lighthouseService)
        {
            this.distributionsRepository = distributionsRepository;
            this.buildsRepository = buildsRepository;
            this.lighthouseService = lighthouseService;

            RuleSet("Create", CreateRuleSet);

            RuleSet("Update", UpdateRuleSet);
        }
        
        private void CommonRuleSet()
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Название не должно быть больше пустым.")
                .MinimumLength(3).WithMessage("Название должно быть больше 3 символов")
                .MaximumLength(256).WithMessage("Название должно быть меньше 256 символов");

            RuleFor(d => d.BuildBindings)
                .Must(BindingsShouldBeUnique).WithMessage("Привязки сборки совпадают");

            RuleFor(d => d.ProjectBindings)
                .Must(BindingsShouldBeUnique).WithMessage("Привязки к проектам совпадают");

            RuleFor(d => d.AvailableLifeCycles)
                .Must(ContainsBuildAndReleaseLifeCycles).WithMessage("Состояния 'Собрана' и 'Релиз' обязательны.");

            RuleFor(d => d.OwnersSids)
                .NotEmpty().WithMessage("Не указаны ответственные за дистрибутив")
                .Must(IsUnique).WithMessage("Список ответственных должен быть уникальным")
                .MustAsync(IsValidUsers).WithMessage("Все пользователи должны существовать и быть активными");
        }

        private async Task<bool> IsValidUsers(IList<string> ownersSids, CancellationToken token) => 
            await lighthouseService.IsValidUsers(ownersSids.ToArray());

        private async Task<bool> IsUniqueDistributionName(string distributionName, CancellationToken token)
        {
            return !await distributionsRepository.Any(x => x.Name == distributionName, token);
        }

        private bool ContainsBuildAndReleaseLifeCycles(LifeCycleState[] lifeCycleStates)
        {
            return lifeCycleStates.Contains(LifeCycleState.Build) && lifeCycleStates.Contains(LifeCycleState.Release);
        }

        private async Task<bool> BindingsPathsIsUnique(BuildBindingModel[] buildBindings, Guid distributionId = default, CancellationToken cancellationToken = default)
        {
            IEnumerable<string> bindingsPaths = buildBindings.Select(x => x.Path);
            return !await distributionsRepository.Any(d =>
                d.Id != distributionId && d.BuildBindings.Any(b => bindingsPaths.Contains(b.Path)),
                cancellationToken
            );
        }

        private bool BindingsShouldBeUnique<T>(T[] bindings) => 
            bindings.Length == bindings.Distinct().Count();

        private bool IsUnique(IList<string> ownersSids) =>
            ownersSids.Select(x => x.ToUpper()).Distinct().Count() == ownersSids.Count;
    }
}
