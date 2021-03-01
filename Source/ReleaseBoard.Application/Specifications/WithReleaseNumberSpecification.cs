using System;
using System.Linq.Expressions;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Specifications
{
    /// <summary>
    /// Фильтрует <see cref="BuildReadModel"/> по номеру версии релиза.
    /// </summary>
    public class WithReleaseNumberSpecification : Specification<BuildReadModel>
    {
        private readonly string releaseNumber;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="releaseNumber">Номер версии релиза, например, 4.2.2.</param>
        public WithReleaseNumberSpecification(string releaseNumber)
        {
            if (string.IsNullOrWhiteSpace(releaseNumber))
            {
                throw new ArgumentNullException(nameof(releaseNumber));
            }

            this.releaseNumber = releaseNumber;
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            return x => x.ReleaseNumber == releaseNumber;
        }
    }
}
