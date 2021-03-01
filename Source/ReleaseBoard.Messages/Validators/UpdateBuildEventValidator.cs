using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using MediatR;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Messages.Validators
{
    /// <summary>
    /// Валидатор для класса <see cref="UpdateBuildEvent"/>.
    /// </summary>
    public class UpdateBuildEventValidator : BuildEventBaseValidator<UpdateBuildEvent>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildReadOnlyRepository"><see cref="IReadOnlyRepository{BuildReadModel}"/>.</param>
        /// <param name="mediator">Медиатр.</param>
        public UpdateBuildEventValidator(IReadOnlyRepository<BuildReadModel> buildReadOnlyRepository, IMediator mediator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(b => b.BuildId).NotEmpty();
            RuleFor(b => b.Location).NotEmpty();
            RuleFor(b => b.Suffixes).NotNull();
            RuleFor(b => b.Number).NotEmpty();

            RuleFor(e => e)
                .CustomAsync(async (@event, context, token) =>
                {
                    BuildReadModel build = await buildReadOnlyRepository.Query(x => x.Id == @event.BuildId, token);
                    if (build == null)
                    {
                        context.AddFailure("Build doesn't exist");
                        return;
                    }
                    
                    IEnumerable<BuildReadModel> existedBuilds = 
                        await mediator.Send(new GetSameBuilds.Query()
                        {
                            DistributionId = build.DistributionId,
                            BuildNumber = @event.Number,
                            Suffixes = @event.Suffixes,
                            SourceType = build.SourceType,
                        });

                    var otherBuilds = existedBuilds.Where(x => x.Id != @event.BuildId);
                    
                    if (otherBuilds.Any())
                    {
                        AddExistBuildsValidationFailure(otherBuilds, context);
                    }
                });
        }

        /// <inheritdoc />
        public override async Task<ValidationResult> ValidateAsync(
            ValidationContext<UpdateBuildEvent> context,
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
            return new(new[]
            {
                new ValidationFailure(nameof(UpdateBuildEvent), "UpdateBuildEvent model should be valid!")
            });
        }
    }
}
