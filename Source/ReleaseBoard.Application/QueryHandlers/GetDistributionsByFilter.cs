using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.QueryHandlers
{
    /// <summary>
    /// Запрос для получения список distributions по фильтру.
    /// </summary>
    public static class GetDistributionsByFilter
    {
        /// <summary>
        /// Запрос.
        /// </summary>
        public class Query : IRequest<IList<DistributionReadModel>>
        {
            /// <summary>
            /// Фильтр.
            /// </summary>
            public DistributionsFilter Filter { get; set; }

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
        public class Handler : IRequestHandler<Query, IList<DistributionReadModel>>
        {
            private readonly IReadOnlyRepository<DistributionReadModel> distributionRepository;

            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="distributionRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
            public Handler(
                IReadOnlyRepository<DistributionReadModel> distributionRepository)
            {
                this.distributionRepository = distributionRepository;
            }

            /// <inheritdoc />
            public async Task<IList<DistributionReadModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                IList<DistributionReadModel> distributions = await GetByFilter(request.Filter);

                return distributions
                    .Where(x => request.IsAdmin || x.Owners.Any(o => o.Sid == request.UserSid))
                    .ToList();
            }

            private async Task<IList<DistributionReadModel>> GetByFilter(DistributionsFilter filter)
            {
                if (filter.SelectedDistributions.IsEmpty())
                {
                    return await distributionRepository.GetAll();
                }
                
                return await distributionRepository.QueryAll(x => filter.SelectedDistributions.Contains(x.Id));
            }
        }
    }
}
