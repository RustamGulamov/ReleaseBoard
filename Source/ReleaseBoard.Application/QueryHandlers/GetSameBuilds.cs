using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using MediatR;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.QueryHandlers
{
    /// <summary>
    /// Запрос для получения список похожих сборок по номеру, суффиксу, дистрибутиву.
    /// </summary>
    public static class GetSameBuilds
    {
        /// <summary>
        /// Запрос.
        /// </summary>
        public class Query : IRequest<IEnumerable<BuildReadModel>>
        {
            /// <summary>
            /// Идентификатор дистриубтива.
            /// </summary>
            public Guid DistributionId { get; set; }

            /// <summary>
            /// Номер сборки.
            /// </summary>
            public string BuildNumber { get; set; } = string.Empty;

            /// <summary>
            /// Суффиксы.
            /// </summary>
            public IEnumerable<string> Suffixes { get; set; } = new List<string>();
            
            /// <summary>
            /// Тип хранилища.
            /// </summary>
            public BuildSourceType SourceType { get; set; }
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        public class Handler : IRequestHandler<Query, IEnumerable<BuildReadModel>>
        {
            private readonly IBuildLifeCycleStateMapper stateMapper;
            private readonly IRepository<BuildReadModel> buildRepository;
            
            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="stateMapper"></param>
            /// <param name="buildRepository"></param>
            public Handler(IBuildLifeCycleStateMapper stateMapper, IRepository<BuildReadModel> buildRepository)
            {
                this.stateMapper = stateMapper;
                this.buildRepository = buildRepository;
            }
            
            /// <inheritdoc />
            public async Task<IEnumerable<BuildReadModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                LifeCycleState state = stateMapper.MapFromSuffixes(request.Suffixes);
                
                IList<BuildReadModel> builds =
                    await buildRepository
                        .QueryAll(x => 
                            x.DistributionId == request.DistributionId && 
                            x.Number == request.BuildNumber &&
                            x.SourceType == request.SourceType &&
                            x.LifeCycleState == state, cancellationToken);
                
                HashSet<string> setSuffixes = request.Suffixes.ToHashSet(StringComparer.OrdinalIgnoreCase);
                return builds.Where(b => setSuffixes.SetEquals(b.Suffixes));
            }
        }
    }
}
