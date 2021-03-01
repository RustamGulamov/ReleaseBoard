using System;
using System.Text.RegularExpressions;
using FluentValidation;
using ReleaseBoard.Web.ApiModels;

namespace ReleaseBoard.Web.Validators
{
    /// <inheritdoc />
    public class NewBuildMatchPatternValidator : AbstractValidator<NewBuildMatchPatternModel>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public NewBuildMatchPatternValidator()
        {
            RuleFor(b => b.Title).NotEmpty();
            RuleFor(b => b.Regexp)
                .NotEmpty()
                .Must(IsRegexpValid)
                .WithMessage("Not valid regular expression");
        }

        private bool IsRegexpValid(string regexp)
        {
            try
            {
                var _ = new Regex(regexp);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
