using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReleaseBoard.Domain.Specifications
{
    /// <summary>
    /// Класс с расширениями для <see cref="Expression"/>.
    /// Используется для комбинирования двух и более выражений
    /// в цепочку фильтров с применением логических операторов И/ИЛИ.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Логический оператор "И" для объединения двух выражений фильтрации.
        /// </summary>
        /// <param name="left">Выражение слева.</param>
        /// <param name="right">Выражение справа.</param>
        /// <typeparam name="T">Тип фильтруемых объектов.</typeparam>
        /// <returns>Цепочка из двух фильтров с логическим "И" между ними.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            ParameterExpression leftParameter = left.Parameters[0];

            SubstitutionExpressionVisitor visitor = new SubstitutionExpressionVisitor();
            visitor.AddExpression(right.Parameters[0], leftParameter);

            Expression body = Expression.AndAlso(left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, leftParameter);
        }

        /// <summary>
        /// Логический оператор "ИЛИ" для объединения двух выражений фильтрации.
        /// </summary>
        /// <param name="left">Выражение слева.</param>
        /// <param name="right">Выражение справа.</param>
        /// <typeparam name="T">Тип фильтруемых объектов.</typeparam>
        /// <returns>Цепочка из двух фильтров с логическим "ИЛИ" между ними.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            ParameterExpression leftParameter = left.Parameters[0];

            SubstitutionExpressionVisitor visitor = new SubstitutionExpressionVisitor();
            visitor.AddExpression(right.Parameters[0], leftParameter);

            Expression body = Expression.OrElse(left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, leftParameter);
        }

        /// <summary>
        /// Вспомогательный класс <see cref="ExpressionVisitor"/> для подстановки деревьев выражений.
        /// </summary>
        private class SubstitutionExpressionVisitor : ExpressionVisitor
        {
            private readonly Dictionary<Expression, Expression> substitutionExpressions = new Dictionary<Expression, Expression>();

            /// <summary>
            /// Сохраняет выражение в словаре для последующей подстановки (см. VisitParameter).
            /// </summary>
            /// <param name="key"><see cref="Expression"/>.</param>
            /// <param name="value"><see cref="Expression"/>.</param>
            public void AddExpression(Expression key, Expression value)
            {
                substitutionExpressions.Add(key, value);
            }

            /// <inheritdoc />
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return substitutionExpressions.TryGetValue(node, out Expression newValue)
                    ? newValue
                    : node;
            }
        }
    }
}
