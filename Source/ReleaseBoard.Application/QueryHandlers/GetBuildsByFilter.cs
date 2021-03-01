using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReleaseBoard.Application.Factories;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.QueryHandlers
{
    /// <summary>
    /// Команда для сборок по фильтру.
    /// </summary>
    public class GetBuildsByFilter
    {
        /// <summary>
        /// Запрос.
        /// </summary>
        public class Query : IRequest<IList<BuildReadModel>>
        {
            /// <summary>
            /// Фильтр.
            /// </summary>
            public DistributionBuildsFilter Filter { get; set; }

            /// <summary>
            /// Делается ли запрос от лица администратора.
            /// </summary>
            public bool IsAdmin { get; set; }

            /// <summary>
            /// Сид автора запроса.
            /// </summary>
            public string UserSid { get; set; }
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        public class Handler : IRequestHandler<Query, IList<BuildReadModel>>
        {
            private readonly IReadOnlyRepository<BuildReadModel> buildRepository;
            private readonly IDistributionBuildsSpecificationFactory distributionBuildsSpecificationFactory;

            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="buildRepository"><see cref="IReadOnlyRepository{BuildReadModel}"/>.</param>
            /// <param name="distributionBuildsSpecificationFactory"><see cref="IDistributionBuildsSpecificationFactory"/>.</param>
            public Handler(
                IReadOnlyRepository<BuildReadModel> buildRepository,
                IDistributionBuildsSpecificationFactory distributionBuildsSpecificationFactory)
            {
                this.buildRepository = buildRepository;
                this.distributionBuildsSpecificationFactory = distributionBuildsSpecificationFactory;
            }

            /// <inheritdoc />
            public async Task<IList<BuildReadModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                Specification<BuildReadModel> specification = await distributionBuildsSpecificationFactory.CreateFromFilter(
                    request.Filter,
                    request.UserSid,
                    request.IsAdmin
                );

                return await buildRepository.QueryAll(specification.ToExpression(), cancellationToken);
            }
        }
    }
}
