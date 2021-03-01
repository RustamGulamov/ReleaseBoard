using System;
using FluentValidation;
using ReleaseBoard.Web.ApiModels.SearchBuildsModels;

namespace ReleaseBoard.Web.Validators
{
    /// <inheritdoc />
    public class SearchBuildsRequestModelValidator : AbstractValidator<SearchBuildsRequestModel>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public SearchBuildsRequestModelValidator()
        {
            RuleFor(b => b.SourceType).IsInEnum();
        }
    }
}
