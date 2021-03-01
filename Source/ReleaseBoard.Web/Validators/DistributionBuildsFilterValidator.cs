using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Web.Validators
{
    /// <summary>
    /// Валидатор фильтра сборок по дистрибутиву.
    /// </summary>
    public class DistributionBuildsFilterValidator : AbstractValidator<DistributionBuildsFilter>
    {
        private readonly IReadOnlyRepository<DistributionReadModel> distributionsRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="distributionsRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
        public DistributionBuildsFilterValidator(IReadOnlyRepository<DistributionReadModel> distributionsRepository)
        {
            this.distributionsRepository = distributionsRepository;
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x).NotNull();
            RuleFor(x => x.DistributionId)
                .MustAsync(DistributionExist)
                .WithMessage("Дистрибутив не обнаружен");

            RuleFor(x => x.CreationDateRange)
                .Must(x => x != null && x.StartDate.Date <= DateTime.Today && x.StartDate.Date <= x.EndDate.Date)
                .WithMessage("Не корректное начало периода даты");
        }

        private async Task<bool> DistributionExist(Guid distributionId, CancellationToken token) =>
           await distributionsRepository.Any(x => x.Id == distributionId, token);
    }
}
