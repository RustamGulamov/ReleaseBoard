using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Builds
{
    /// <summary>
    /// Модель снепшота билда.
    /// </summary>
    public class BuildSnapshot : AggregateSnapshot
    {
        /// <summary>
        /// Cостояние артефактов (файлов сборки).
        /// </summary>
        public ArtifactState ArtifactState { get; set; }

        /// <summary>
        /// Дата сборки.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата обновления сборки.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Идентификатор дистрибутива.
        /// </summary>
        public Guid DistributionId { get; set; }

        /// <summary>
        /// Помечена ли сборка как "удаленная".
        /// </summary>
        public bool IsUnTracked { get; set; }

        /// <summary>
        /// Cостояние сборки в жизненном цикле выпуска релиза.
        /// </summary>
        public LifeCycleState LifeCycleState { get; set; }

        /// <summary>
        /// Относительный путь в хранилище сборок.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Номер сборки.
        /// </summary>
        public VersionNumber Number { get; set; }

        /// <summary>
        /// Основной номер версии релиза.
        /// Например при парсинге билда HW100\4.3.2\4.3_(2.3298), то ReleaseNumber=4.3.2.
        /// </summary>
        public VersionNumber ReleaseNumber { get; set; }

        /// <summary>
        /// Суффиксы.
        /// </summary>
        public IList<string> Suffixes { get; set; } = new List<string>();

        /// <summary>
        /// Теги.
        /// </summary>
        public IList<string> Tags { get; set; } = new List<string>();
    }
}
