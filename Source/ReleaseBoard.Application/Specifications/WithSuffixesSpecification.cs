using System;
using System.Linq;
using System.Linq.Expressions;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Specifications
{
    /// <summary>
    /// Фильтрует <see cref="BuildReadModel"/> по тегам и суффиксам.
    /// </summary>
    public class WithSuffixesSpecification : Specification<BuildReadModel>
    {
        private readonly string[] suffixes;
        private readonly SelectCondition selectCondition;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="suffixes">Список суффиксов для фильтрации сборок.</param>
        /// <param name="selectCondition">Логическое условие фильтрации суффиксов.</param>
        public WithSuffixesSpecification(string[] suffixes, SelectCondition selectCondition)
        {
            this.suffixes = suffixes ?? throw new ArgumentNullException(nameof(suffixes));
            this.selectCondition = selectCondition;
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            switch (selectCondition)
            {
                case SelectCondition.And:
                    return x => suffixes.All(t => x.Suffixes.Contains(t));
                case SelectCondition.Or:
                    return x => suffixes.Any(t => x.Suffixes.Contains(t));
                default:
                    throw new InvalidOperationException($"Unrecognized condition: {selectCondition}");
            }
        }
    }
}
