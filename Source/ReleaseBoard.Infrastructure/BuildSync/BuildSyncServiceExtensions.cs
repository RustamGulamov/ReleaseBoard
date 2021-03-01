using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.BuildSync.Responses;
using ReleaseBoard.Common.Contracts.Common.Models;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Infrastructure.BuildSync
{
    /// <summary>
    /// Расширения для сервиса синхронизации сборок.
    /// </summary>
    public static class BuildSyncServiceExtensions
    {
        /// <summary>
        /// Ищет сборки по биндингам.
        /// </summary>
        /// <param name="buildSyncService">Сервис синхронизации.</param>
        /// <param name="bindings">Биндинги.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public static async Task<IEnumerable<BuildDto>> ParseBuilds(
            this IBuildSyncService buildSyncService, 
            IEnumerable<BuildsBinding> bindings)
        {
            List<Task<ParseBuildsResponse>> tasks =
                bindings
                .Select(b => buildSyncService.ParseBuilds(b.Path, b.SourceType, b.Pattern.Regexp))
                .ToList();

            ParseBuildsResponse[] messages = await Task.WhenAll(tasks);
            
            return messages.SelectMany(s => s.Builds);
        }
    }
}
