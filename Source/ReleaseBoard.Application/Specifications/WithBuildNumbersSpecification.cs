using System;
using System.Linq;
using System.Linq.Expressions;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Specifications
{
    /// <summary>
    /// Фильтрует <see cref="BuildReadModel"/> по номерам билдов.
    /// </summary>
    public class WithBuildNumbersSpecification : Specification<BuildReadModel>
    {
        private readonly string[] buildNumbers;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildNumbers">Cписок номеров сборки.</param>
        public WithBuildNumbersSpecification(string[] buildNumbers)
        {
            this.buildNumbers = buildNumbers ?? throw new ArgumentNullException(nameof(buildNumbers));
        }

        /// <inheritdoc />
        public override Expression<Func<BuildReadModel, bool>> ToExpression()
        {
            return x => buildNumbers.Contains(x.Number);
        }
    }
}
