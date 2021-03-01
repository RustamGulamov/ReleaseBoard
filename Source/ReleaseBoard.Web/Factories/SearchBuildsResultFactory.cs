using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.SearchBuildsModels;

namespace ReleaseBoard.Web.Factories
{
    /// <summary>
    /// Фабрика для создания <see cref="SearchBuildsResultModel"/>.
    /// </summary>
    public class SearchBuildsResultFactory
    {
        /// <summary>
        /// Создает экземпляр <see cref="List{SearchBuildsRequestModel}"/>.
        /// </summary>
        /// <param name="response">Ответ на запрос на сканирование билдов.</param>
        /// <param name="patterns">Список билдов.</param>
        /// <returns>Список результатов сканирования.</returns>
        public static List<SearchBuildsResultModel> Create(ScanForBuildsListResponse response, IList<BuildMatchPatternReadModel> patterns)
        {
            return response
                .Items
                .Select(s =>
                {
                    BuildMatchPatternReadModel pattern = patterns.FirstOrDefault(x => x?.Regexp?.Equals(s.PatternMatch, StringComparison.OrdinalIgnoreCase) ?? false);
                    return new SearchBuildsResultModel
                    {
                        BuildsCount = s.BuildsCount,
                        PatternMatchId = pattern?.Id.ToString()
                    };
                })
                .ToList();
        }
    }
}
