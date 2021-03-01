using System;

namespace ReleaseBoard.Web.ApiModels.BuildModels
{
    /// <summary>
    /// Модель обновления тегов сборки.
    /// </summary>
    public class UpdateBuildTagsModel
    {
        /// <summary>
        /// Идентификатор сборки.
        /// </summary>
        public Guid BuildId { get; set; }

        /// <summary>
        /// Теги.
        /// </summary>
        public string[] Tags { get; set; } = { };
    }
}
