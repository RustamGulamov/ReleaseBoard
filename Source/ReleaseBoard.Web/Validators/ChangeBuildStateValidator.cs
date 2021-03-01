using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.BuildModels;

namespace ReleaseBoard.Web.Validators
{
    /// <inheritdoc />
    public class ChangeBuildStateValidator : AbstractValidator<ChangeBuildStateRequestModel>
    {
        private readonly IReadOnlyRepository<BuildReadModel> buildsRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildsRepository"><see cref="IReadOnlyRepository{BuildReadModel}"/>.</param>
        public ChangeBuildStateValidator(IReadOnlyRepository<BuildReadModel> buildsRepository)
        {
            this.buildsRepository = buildsRepository;
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x).NotNull();
            RuleFor(x => x.BuildId).NotEmpty()
                .MustAsync(BeExist)
                .WithMessage(b => $"Build {b.BuildId} must be exist.");
            
            RuleFor(x => x.NewState)
                .MustAsync((m, _, t) => BeNotEqualToCurrentState(m, t))
                .WithMessage(b => $"Build {b.BuildId} current state is {b.NewState}.");
        }

        private async Task<bool> BeExist(Guid buildId, CancellationToken token) =>
            await buildsRepository.Any(x => x.Id == buildId, token);

        private async Task<bool> BeNotEqualToCurrentState(ChangeBuildStateRequestModel model, CancellationToken token)
        {
            var build = await buildsRepository.Query(x => x.Id == model.BuildId, token);
            return build.LifeCycleState != model.NewState;
        }
    }
}
