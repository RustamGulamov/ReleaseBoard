using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.BuildSync.Responses;
using ReleaseBoard.Application.Interfaces;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.Services
{
    /// <inheritdoc />
    public class FakeBuildSyncService : IBuildSyncService
    {
        /// <inheritdoc />
        public Task<IResponseMessage> ChangeSuffix(string buildLocation, BuildSourceType sourceType, List<string> suffixes, List<string> currentSuffixes)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ParseBuildsResponse> ParseBuilds(string path, BuildSourceType sourceType, string pattern)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IResponseMessage> SearchBuilds(string path, BuildSourceType sourceType, IEnumerable<string> patternsToCheck)
        {
            var response = new ScanForBuildsListResponse()
            {
                Items = new List<ScanForBuildsResult>
                {
                    new ScanForBuildsResult
                    {
                        PatternMatch = Guid.NewGuid().ToString(), 
                        BuildsCount = 25
                    }
                }
            };
            return Task.FromResult<IResponseMessage>(response);
        }
    }
}
