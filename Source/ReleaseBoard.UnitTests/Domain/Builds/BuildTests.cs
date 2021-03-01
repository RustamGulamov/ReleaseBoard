using System;
using System.Collections.Generic;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Базовый класс для тестов билда.
    /// </summary>
    public abstract class BuildTests
    {
        /// <summary>
        /// Дата сборки для теста.
        /// </summary>
        protected readonly DateTime BuildDate = new(2020, 11, 12);
        
        /// <summary>
        /// Местоположение.
        /// </summary>
        protected readonly string Location = "/path/to";
        
        /// <summary>
        /// Номер сборки.
        /// </summary>
        protected readonly VersionNumber Number = new("1.3.2.3");
        
        /// <summary>
        /// Идентификатор дистрибутива.
        /// </summary>
        protected readonly Guid DistributionId = Guid.Parse("ac54a8ee-9601-449c-9b89-5c0f19d77d0b");
        
        /// <summary>
        /// Релизный номер сборки.
        /// </summary>
        protected readonly VersionNumber ReleaseNumber = new("1.3.2.3");

        /// <summary>
        /// Создает сборку.
        /// </summary>
        /// <param name="state">Состояние.</param>
        /// <param name="suffixes">Суффиксы.</param>
        /// <returns>Сборка.</returns>
        protected Build CreateBuild(LifeCycleState state = LifeCycleState.Build, List<string> suffixes = null) => 
            new(BuildDate, Number, ReleaseNumber, DistributionId, Location, BuildSourceType.Pdc, state, suffixes);
    }
}
