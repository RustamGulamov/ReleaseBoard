using System;

namespace ReleaseBoard.Web.ApiModels.BuildModels
{
    /// <summary>
    /// Модель ответа запроса <see cref="GenerateNewBuildPathsRequestModel"/>.
    /// </summary>
    public class GenerateNewBuildPathsResponseModel
    {
        /// <summary>
        /// <see cref="BuildSourceType"/>.
        /// </summary>
        public BuildSourceType BuildSourceType { get; set; }

        /// <summary>
        /// Сгенерирование пути.
        /// </summary>
        public string[] BuildPaths { get; set; }
    }
}
