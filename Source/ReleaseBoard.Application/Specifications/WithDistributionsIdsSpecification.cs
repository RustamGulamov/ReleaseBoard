using System;
using System.Linq;
using System.Linq.Expressions;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Specifications
{
    /// <summary>
    /// Фильтрует <see cref="BuildReadModel"/> по идентификатору дистрибутива.
    /// </summary>
    public class WithDistributionsIdsSpecification : Specification<BuildReadModel>
    {
        private readonly Guid[] distributionsIds;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="distributionsIds">Идентификатор дистрибутива.</param>
        public WithDistributionsIdsSpecification(params Guid[] distributionsIds)
        {
            this.distributionsIds = distributionsIds;
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            return x => distributionsIds.Contains(x.DistributionId);
        }
    }
}
