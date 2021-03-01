using System;
using System.Linq;
using System.Linq.Expressions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Specifications
{
    /// <summary>
    /// Фильтрует <see cref="BuildReadModel"/> по состоянию сборки <see cref="LifeCycleState"/>.
    /// </summary>
    public class WithLifeCycleStateSpecification : Specification<BuildReadModel>
    {
        private readonly LifeCycleState[] lifeCycleStates;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="lifeCycleStates">Массив доступных состояний сборки для фильтрации.</param>
        public WithLifeCycleStateSpecification(LifeCycleState[] lifeCycleStates)
        {
            this.lifeCycleStates = lifeCycleStates;
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            return x => lifeCycleStates.Contains(x.LifeCycleState);
        }
    }
}
