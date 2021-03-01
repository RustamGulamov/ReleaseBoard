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
    public class WithTagsSpecification : Specification<BuildReadModel>
    {
        private readonly string[] tags;
        private readonly SelectCondition selectCondition;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="tags">Список тегов для фильтрации сборок.</param>
        /// <param name="selectCondition">Логическое условие фильтрации тегов.</param>
        public WithTagsSpecification(string[] tags, SelectCondition selectCondition)
        {
            this.tags = tags ?? throw new ArgumentNullException(nameof(tags));
            this.selectCondition = selectCondition;
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            switch (selectCondition)
            {
                case SelectCondition.And:
                    return x => tags.All(t => x.Tags.Contains(t));
                case SelectCondition.Or:
                    return x => tags.Any(t => x.Tags.Contains(t));
                default:
                    throw new InvalidOperationException($"Unrecognized condition: {selectCondition}");
            }
        }
    }
}
