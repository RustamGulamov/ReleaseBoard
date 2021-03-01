using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReleaseBoard.Domain.Specifications
{
    /// <summary>
    /// Представляет спецификацию "И" - применяется для комбинирования других спецификаций.
    /// </summary>
    /// <typeparam name="T">Тип проверяемого объекта.</typeparam>
    public class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T>[] specifications;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="specifications">Список спецификаций для объединения посредством логического "И".</param>
        public AndSpecification(params Specification<T>[] specifications)
        {
            this.specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));

            if (!specifications.Any())
            {
                throw new ArgumentNullException(nameof(specifications));
            }
        }

        /// <inheritdoc />
        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> result = specifications[0].ToExpression();
            foreach (Specification<T> specification in specifications.Skip(1))
            {
                result = result.And(specification.ToExpression());
            }

            return result;
        }
    }
}
