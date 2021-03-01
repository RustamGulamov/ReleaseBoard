using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Builds;

namespace ReleaseBoard.Web.ApiModels.DistributionModels
{
    /// <summary>
    /// Модель дистрибутива.
    /// Используется для api.
    /// </summary>
    public class DistributionModel
    {
        /// <summary>
        /// Идентификатор дистрибутива.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название дистрибутива.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификаторы ответственных за дистрибутив.
        /// </summary>
        public IList<string> OwnersSids { get; set; } = new List<string>();

        /// <summary>
        /// Список доступных состояний сборок у дистрибутива.
        /// </summary>
        public LifeCycleState[] AvailableLifeCycles { get; set; } =
        {
            LifeCycleState.Build, 
            LifeCycleState.ReleaseCandidate, 
            LifeCycleState.Release
        };

        /// <summary>
        /// Привязки к сборкам.
        /// </summary>
        public BuildBindingModel[] BuildBindings { get; set; } = { };

        /// <summary>
        /// Привязки к проектам.
        /// </summary>
        public ProjectBindingModel[] ProjectBindings { get; set; } = { };
    }
}
