using System;
using System.Linq;
using FluentValidation;

namespace ReleaseBoard.Web.Validators.ModelValidators
{
    /// <summary>
    /// Валидатор. Правила для создания дистрибутива.
    /// </summary>
    public partial class DistributionModelValidator
    {
        private void CreateRuleSet()
        {
            CommonRuleSet();

            RuleFor(d => d.Name)
                .MustAsync(IsUniqueDistributionName)
                .WithMessage(DistributionNameShouldBeUniqueMessage);


            When(x => x.BuildBindings.Any(),
                () =>
                {
                    RuleFor(x => x.BuildBindings)
                        .MustAsync((bindings, cancellationToken) => BindingsPathsIsUnique(bindings, cancellationToken: cancellationToken))
                        .WithMessage("Один из путей в привязках используется в другом дистрибутиве.");
                });
        }
    }
}
