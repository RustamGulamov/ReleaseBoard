using System;
using System.Linq.Expressions;
using ReleaseBoard.Application.Models;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Specifications
{
    /// <summary>
    /// Фильтрует <see cref="BuildReadModel"/> по вхождению даты создания сборки в указанный период.
    /// </summary>
    public class WithCreationDateRangeSpecification : Specification<BuildReadModel>
    {
        private readonly DateRange creationDateRange;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="creationDateRange">Период создания сборки.</param>
        public WithCreationDateRangeSpecification(DateRange creationDateRange)
        {
            this.creationDateRange = creationDateRange ?? throw new ArgumentNullException(nameof(creationDateRange));
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            return x => x.BuildDate >= creationDateRange.StartDate.Date
                && x.BuildDate < creationDateRange.EndDate.Date.AddDays(1);
        }
    }
}
