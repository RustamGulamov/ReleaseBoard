using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using Moq;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.StateChangeChecker;
using ReleaseBoard.Domain.Builds.TransferRules;
using ReleaseBoard.Domain.Builds.TransferRules.Interfaces;
using ReleaseBoard.Domain.Builds.TransferRules.Repository;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Тесты для <see cref="BuildStateChangeChecker"/>.
    /// </summary>
    public class BuildStateChangeCheckerTests
    {
        private readonly BuildStateChangeChecker buildStateChangeChecker;
        private readonly List<IBuildStateTransferRule> rules = new List<IBuildStateTransferRule>()
        {
            new ReleaseStateTransferRule(),
            new ReleaseCandidateStateTransferRule(),
            new CertifiedStateTransferRule()
        };
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        public BuildStateChangeCheckerTests()
        {
            var transferRuleRepositoryMock = new Mock<ITransferRuleRepository>();
            var aggregateRepositoryMock = new Mock<IAggregateRepository>();

            aggregateRepositoryMock.Setup(x => x.LoadById<Distribution>(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Distribution("name", FakeGenerator.Users.Generate(2), new LifeCycleState[] { LifeCycleState.Build }, new string[] { "some_rule" })));

            transferRuleRepositoryMock
                .Setup(x => x.Get(It.IsAny<List<string>>()))
                .Returns(rules);

            buildStateChangeChecker = new BuildStateChangeChecker(transferRuleRepositoryMock.Object, aggregateRepositoryMock.Object);
        }

        /// <summary>
        /// Список номеров билдов спецификации, сборки
        /// и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> BuildStateTransferRuleCurrentStateAndNewState => new[]
        {
            new object[]
            {
                string.Empty,
                LifeCycleState.Build,
                LifeCycleState.ReleaseCandidate,
                true
            },
            new object[]
            {
                "_RC",
                LifeCycleState.ReleaseCandidate,
                LifeCycleState.Release,
                true
            },
            new object[]
            {
                "_R",
                LifeCycleState.Release,
                LifeCycleState.ReleaseCandidate,
                true
            },
            new object[]
            {
                "_R",
                LifeCycleState.Release,
                LifeCycleState.Certified,
                true
            },
            new object[]
            {
                string.Empty,
                LifeCycleState.Build,
                LifeCycleState.Release,
                true
            },
            new object[]
            {
                "_R",
                LifeCycleState.Release,
                LifeCycleState.Build,
                false
            },
            new object[]
            {
                "_RC",
                LifeCycleState.ReleaseCandidate,
                LifeCycleState.Certified,
                false
            },
        };

        /// <summary>
        /// Тест проверят правил перехода.
        /// </summary>
        /// <param name="suffix">Суффиксы.</param>
        /// <param name="currentState">Текущее состояние.</param>
        /// <param name="newState">Новое состояние.</param>
        /// <param name="expectedCanChange">Ожидаемый результат.</param>
        /// <returns><see cref="Task"/>.</returns>
        [Theory]
        [MemberData(nameof(BuildStateTransferRuleCurrentStateAndNewState))]
        public async Task Check_BuildCurrentAndNewState_CanChange(string suffix, LifeCycleState currentState, LifeCycleState newState, bool expectedCanChange)
        {
            Build build = CreateBuild(currentState, suffix);

            StateChangeCheckResult result = await buildStateChangeChecker.Check(build, newState);

            Assert.Equal(expectedCanChange, result.CanChange);
        }


        private Build CreateBuild(LifeCycleState state, string suffix = "")
        {
            return new Build(
                DateTime.Now,
                new VersionNumber("4.3.2.1"),
                new VersionNumber("4.3.2"),
                Guid.NewGuid(),
                "Vipnet_Clinet\\4.3.2\\4.3.2_(4.3.2.1)" + suffix,
                BuildSourceType.Pdc,
                state,
                new List<string>()
                );
        }
    }
}
