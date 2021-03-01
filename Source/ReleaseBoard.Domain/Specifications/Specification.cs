using System;
using System.Linq.Expressions;

namespace ReleaseBoard.Domain.Specifications
{
    /// <summary>
    /// Шаблон спецификация.
    /// </summary>
    /// <typeparam name="T">Объект для проверки.</typeparam>
    public abstract class Specification<T>
    {
        /// <summary>
        /// Выражение проверки удовлетворения спецификации.
        /// </summary>
        /// <returns>Выражение проверки.</returns>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Метод для проверки соответствия объекта спецификации.
        /// </summary>
        /// <param name="entity">Проверяемая сущность.</param>
        /// <returns>Подходит ли объект под спецификацию.</returns>
        public bool IsSatisfiedBy(T entity)
        {
            return ToExpression()
                .Compile()
                .Invoke(entity);
        }
    }
}
