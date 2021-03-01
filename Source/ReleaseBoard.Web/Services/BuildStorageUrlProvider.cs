using System;
using System.IO;
using System.Threading.Tasks;
using ReleaseBoard.Web.Helpers;
using ReleaseBoard.Web.Services.Interfaces;
using ReleaseBoard.Web.Settings;

namespace ReleaseBoard.Web.Services
{
    /// <inheritdoc />
    public class BuildStorageUrlProvider : IBuildStorageUrlProvider
    {
        private readonly BuildStorageUrlSettings settings;

        /// <summary>
        /// .ctor.
        /// </summary>
        /// <param name="settings"><see cref="BuildStorageUrlSettings"/>.</param>
        public BuildStorageUrlProvider(BuildStorageUrlSettings settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc />
        public Task<string> GetOpenUrl(BuildSourceType buildSourceType, string buildLocation)
        {
            string path = PathEncoder.GetEncoded(buildLocation);

            string result = settings.BaseUrlsToOpenBuilds.TryGetValue(buildSourceType, out string baseUrl)
                ? Path.Combine(baseUrl, path)
                : throw new NotSupportedException($"Не удалось найти базовый путь для {buildSourceType:G}");

            return Task.FromResult(result);
        }
    }
}
