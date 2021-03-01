using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.DistributionModels;

namespace ReleaseBoard.Web.Validators.ModelValidators
{
    /// <summary>
    /// Валидатор. Правила для обновления дистрибутива.
    /// </summary>
    public partial class DistributionModelValidator
    {
        private void UpdateRuleSet()
        {
            RuleFor(d => d.Id)
                .MustAsync(DistributionExist)
                .WithMessage(x => $"Дистрибутив c идентификатором {x.Id} не существует")
                .DependentRules(CommonRuleSet);

            RuleFor(model => model)
                .CustomAsync(async (model, context, token) =>
                {
                    DistributionReadModel distribution = await distributionsRepository.Query(x => x.Id == model.Id, token);

                    if (distribution == null)
                    {
                        return;
                    }

                    if (DistributionNameChanged(distribution, model) && !await IsUniqueDistributionName(model.Name, token))
                    {
                        context.AddFailure(nameof(model.Name), DistributionNameShouldBeUniqueMessage);
                    }

                    IEnumerable<LifeCycleState> deletedLifeCycleStates = distribution.AvailableLifeCycles.Except(model.AvailableLifeCycles);
                    if (await ExistBuildByDeletedLifeCycleState(distribution, deletedLifeCycleStates))
                    {
                        context.AddFailure(nameof(model.AvailableLifeCycles), $"Существует сборка с удаленным состоянием {deletedLifeCycleStates.First():G}");
                    }
                });

            When(x => x.BuildBindings.Any(),
                () =>
                {
                    RuleFor(x => x.BuildBindings)
                        .MustAsync((distribution, buildBindings, cancellationToken) => BindingsPathsIsUnique(buildBindings, distribution.Id, cancellationToken))
                        .WithMessage("Один из путей в привязках используется в другом дистрибутиве.");
                });

        }
        
        private async Task<bool> DistributionExist(Guid distributionId, CancellationToken token)
        {
            return await distributionsRepository.Any(x => x.Id == distributionId, token);
        }

        private async Task<bool> ExistBuildByDeletedLifeCycleState(DistributionReadModel distribution, IEnumerable<LifeCycleState> deletedLifeCycleState)
        {
            return await buildsRepository.Any(x => x.DistributionId == distribution.Id && deletedLifeCycleState.Contains(x.LifeCycleState));
        }
        
        private bool DistributionNameChanged(DistributionReadModel existDistribution, DistributionModel updatedDistribution)
        {
            return !string.Equals(updatedDistribution.Name, existDistribution.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
