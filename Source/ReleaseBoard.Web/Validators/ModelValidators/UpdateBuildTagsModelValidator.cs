using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.BuildModels;

namespace ReleaseBoard.Web.Validators.ModelValidators
{
    /// <inheritdoc />
    public class UpdateBuildTagsModelValidator : AbstractValidator<UpdateBuildTagsModel>
    {
        private readonly IReadOnlyRepository<BuildReadModel> buildsRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildsRepository"><see cref="IReadOnlyRepository{BuildReadModel}"/>.</param>
        public UpdateBuildTagsModelValidator(IReadOnlyRepository<BuildReadModel> buildsRepository)
        {
            this.buildsRepository = buildsRepository;
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.BuildId)
                .MustAsync(Exist)
                .WithMessage(m => $"Сборки {m.BuildId} не существует.");

            RuleFor(x => x.Tags)
                .Must(HasNotEmptyTag)
                .WithMessage("Теги не должны быть пустыми.");

            RuleFor(x => x.Tags)
                .Must(EveryTagUnique)
                .WithMessage("Теги должны быть уникальными.");

            RuleForEach(x => x.Tags)
                .MaximumLength(100)
                .WithMessage("Длина тега не может быть больше 100 символов.");

        }

        private async Task<bool> Exist(Guid buildId, CancellationToken token) =>
            await buildsRepository.Any(x => x.Id == buildId, token);

        private bool HasNotEmptyTag(string[] tags) => !tags.Any(string.IsNullOrWhiteSpace);

        private bool EveryTagUnique(string[] tags) =>
            tags
                .Select(x => x.ToLower().Trim(' '))
                .Distinct()
                .Count() == tags.Length;
    }
}
