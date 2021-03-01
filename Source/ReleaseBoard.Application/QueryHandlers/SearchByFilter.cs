using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Application.Models;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.QueryHandlers
{
    /// <summary>
    /// Запрос для поиска по сборкам и дистрибутивам.
    /// </summary>
    public static class SearchByFilter
    {
        /// <summary>
        /// Запрос.
        /// </summary>
        public class Query : IRequest<IEnumerable<SearchResult<object>>>
        {
            /// <summary>
            /// Строка поиска.
            /// </summary>
            public string SearchText { get; set; }

            /// <summary>
            /// Является ли админом.
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
        public class Handler : IRequestHandler<Query, IEnumerable<SearchResult<object>>>
        {
            private readonly ISearchRepository<BuildReadModel> buildSearchRepository;
            private readonly IReadOnlyRepository<DistributionReadModel> distributionsRepository;

            /// <summary>
            /// ctor.
            /// </summary>
            /// <param name="buildSearchRepository"><see cref="ISearchRepository{BuildReadModel}"/>.</param>
            /// <param name="distributionsRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
            public Handler(
                ISearchRepository<BuildReadModel> buildSearchRepository,
                IReadOnlyRepository<DistributionReadModel> distributionsRepository)
            {
                this.buildSearchRepository = buildSearchRepository;
                this.distributionsRepository = distributionsRepository;
            }

            /// <inheritdoc />
            public async Task<IEnumerable<SearchResult<object>>> Handle(Query request, CancellationToken cancellationToken)
            {
                IList<DistributionReadModel> distributions = await distributionsRepository.QueryAll(
                    d => request.IsAdmin || d.Owners.Any(o => o.Sid == request.UserSid), 
                    cancellationToken
                );

                var uniqueDistIds = distributions.Select(x => x.Id).Distinct();
                Expression<Func<BuildReadModel, bool>> filter = model => uniqueDistIds.Contains(model.DistributionId);
                
                return new SearchResult<object>[]
                {
                    await SearchBuild(request.SearchText, request.IsAdmin ? null : filter),
                    SearchDistributions(request.SearchText, distributions)
                };
            }

            private SearchResult<object> SearchDistributions(string filter, IList<DistributionReadModel> distributions) 
            {
                var resultSearchByName = distributions.Where(x => x.Name.Contains(filter, StringComparison.OrdinalIgnoreCase));

                return new SearchResult<object>()
                {
                    Type = SearchResultType.Distributions,
                    Result = resultSearchByName
                };
            }

            private async Task<SearchResult<object>> SearchBuild(string searchText, Expression<Func<BuildReadModel, bool>> filter = null)
            {
                IList<BuildReadModel> builds = await buildSearchRepository.TextSearch(
                    searchText, 
                    filter, 
                    x => x.Number, 
                    x => x.Suffixes, 
                    x=> x.Tags);

                return new SearchResult<object>()
                {
                    Type = SearchResultType.Builds,
                    Result = builds
                };
            }
        }
    }
}
