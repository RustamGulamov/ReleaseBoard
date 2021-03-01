using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;

namespace ReleaseBoard.UnitTests.Domain.Distributions
{
    /// <summary>
    /// Базовый метод для тестов дистрибутивов.
    /// </summary>
    public abstract class DistributionTests
    {
        /// <summary>
        /// Имя дистрибутива.
        /// </summary>
        protected static readonly string DistributionName = "Мой дистрибутив";

        /// <summary>
        /// Пользователь.
        /// </summary>
        protected static readonly User[] Owners = FakeGenerator.Users.Generate(2).ToArray();

        /// <summary>
        /// Список доступных состояний сборок у дистрибутива.
        /// </summary>
        protected static readonly LifeCycleState[] AvailableLifeCycles = { LifeCycleState.Build , LifeCycleState.Release };
        
        /// <summary>
        /// Имена правил перехода по состояниям сборки.
        /// </summary>
        protected static readonly string[] LifeCycleStateRules = { LifeCycleState.Build.ToString(), LifeCycleState.Release.ToString() };

        /// <summary>
        /// Создает дистрибутив.
        /// </summary>
        /// <returns>Дистрибутив.</returns>
        protected Distribution CreateDistribution() => new(DistributionName, Owners, AvailableLifeCycles, LifeCycleStateRules);

        /// <summary>
        /// Получить события.
        /// </summary>
        /// <param name="distribution"><see cref="Distribution"/>.</param>
        /// <returns><see cref="Event"/>.</returns>
        protected List<Event> GetUncommitedChanges(Distribution distribution)
        {
            return new(distribution.GetUncommitedChanges());
        }
    }
}
