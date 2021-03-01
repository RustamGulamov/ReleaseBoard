using System;
using ReleaseBoard.Domain.Builds;

namespace ReleaseBoard.Web.ApiModels.BuildModels
{
    /// <summary>
    /// Модель запроса на изменения состояния билда.
    /// </summary>
    public class ChangeBuildStateRequestModel
    {
        /// <summary>
        /// Идентификатор билда.
        /// </summary>
        public Guid BuildId { get; set; }

        /// <summary>
        /// Новое состояние билда.
        /// </summary>
        public LifeCycleState NewState { get; set; }

        /// <summary>
        /// Комментарий.
        /// </summary>
        public string Comment { get; set; }
    }
}
