using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Messages.Validators
{
    /// <summary>
    /// Валидатор для класса <see cref="DeleteBuildEvent"/>.
    /// </summary>
    public class DeleteBuildEventValidator : AbstractValidator<DeleteBuildEvent>
    {
        private readonly IReadOnlyRepository<BuildReadModel> buildReadOnlyRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildReadOnlyRepository"><see cref="IReadOnlyRepository{BuildReadModel}"/>.</param>
        public DeleteBuildEventValidator(IReadOnlyRepository<BuildReadModel> buildReadOnlyRepository)
        {
            this.buildReadOnlyRepository = buildReadOnlyRepository;
            this.CascadeMode = CascadeMode.Stop;

            RuleFor(b => b.BuildId)
                .MustAsync(BeExist)
                .WithMessage(b => $"Build { b.BuildId } doesn't exist.");
        }

        /// <inheritdoc />
        public override async Task<ValidationResult> ValidateAsync(
            ValidationContext<DeleteBuildEvent> context,
            CancellationToken cancellation = default)
        {
            return context?.InstanceToValidate == null ? GetFailure() : await base.ValidateAsync(context, cancellation);
        }

        /// <summary>
        /// Возвращает негативный результат проверки модели запроса.
        /// </summary>
        /// <returns>Негативный результат проверки.</returns>
        private ValidationResult GetFailure()
        {
            return new ValidationResult(new[]
            {
                new ValidationFailure(nameof(UpdateBuildEvent), "DeleteBuildEvent model should be valid!")
            });
        }

        private async Task<bool> BeExist(Guid buildId, CancellationToken token)
        {
            return await buildReadOnlyRepository.Any(x => x.Id == buildId, token);
        }
    }
}
