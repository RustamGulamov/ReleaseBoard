using System;

namespace ReleaseBoard.Web.ApiModels.BuildModels
{
    /// <summary>
    /// Модель запроса генерации новых путей сборки.
    /// </summary>
    public class GenerateNewBuildPathsRequestModel
    {
        /// <summary>
        /// Идентификатор дистрибутива.
        /// </summary>
        public Guid DistributionId { get; set; }

        /// <summary>
        /// Предполагаемая версия номер сборки.
        /// </summary>
        public string Number { get; set; }
    }
}
