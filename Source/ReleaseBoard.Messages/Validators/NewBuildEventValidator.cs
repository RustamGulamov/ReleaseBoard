using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using MediatR;
using ReleaseBoard.Application;
using ReleaseBoard.Application.QueryHandlers;

namespace ReleaseBoard.Messages.Validators
{
    /// <summary>
    /// Валидатор для класса <see cref="NewBuildEvent"/>.
    /// </summary>
    public class NewBuildEventValidator : BuildEventBaseValidator<NewBuildEvent>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        public NewBuildEventValidator(IMediator mediator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(b => b.Build)
                .NotNull()
                .WithMessage("Build is null");
            
            RuleFor(b => b.Build)
                .SetValidator(new NewBuildValidator())
                .CustomAsync(async (build, context, _) =>
                {
                    var existedBuilds = 
                        await mediator.Send(new GetSameBuilds.Query()
                        {
                            DistributionId = build.DistributionId,
                            BuildNumber = build.Number,
                            Suffixes = build.Suffixes,
                            SourceType = build.SourceType
                        });
            
                    if (existedBuilds.Any())
                    {
                        AddExistBuildsValidationFailure(existedBuilds, context);
                    }
                });
        }

        /// <inheritdoc />
        public override async Task<ValidationResult> ValidateAsync(
            ValidationContext<NewBuildEvent> context,
            CancellationToken cancellation = default)
        {
            return context?.InstanceToValidate == null ? GetFailure() : await base.ValidateAsync(context, cancellation);
        }

        /// <summary>
        /// Возвращает негативный результат проверки модели запроса.
        /// </summary>
        /// <returns>Негативный результат проверки.</returns>
        private ValidationResult GetFailure() =>
            new(new[]
            {
                new ValidationFailure(nameof(NewBuildEvent), "NewBuildEvent model should be valid!")
            });
    }
}
