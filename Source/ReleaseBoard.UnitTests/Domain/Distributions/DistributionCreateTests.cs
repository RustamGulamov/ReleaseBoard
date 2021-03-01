using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.Distributions.Events.LifeCycleStates;
using ReleaseBoard.Domain.Distributions.Events.Owners;
using ReleaseBoard.Domain.Distributions.Exceptions;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions
{
    /// <summary>
    /// Тесты для создания дистрибутива.
    /// </summary>
    public class DistributionCreateTests : DistributionTests
    {
        /// <summary>
        /// Некорректные данные для создания дистрибутива.
        /// </summary>
        public static IEnumerable<object[]> InvalidArgsDistribution => new[]
        {
            new object[] { null, Owners, AvailableLifeCycles, LifeCycleStateRules },
            new object[] { "    ", Owners, AvailableLifeCycles, LifeCycleStateRules },
            new object[] { DistributionName, null, AvailableLifeCycles, LifeCycleStateRules },
            new object[] { DistributionName, Owners, null, LifeCycleStateRules },
            new object[] { DistributionName, Owners, AvailableLifeCycles, null },
            new object[] { DistributionName, Enumerable.Empty<User>(), AvailableLifeCycles, LifeCycleStateRules },
            new object[] { DistributionName, Owners, Enumerable.Empty<LifeCycleState>(), LifeCycleStateRules },
        };
        
        /// <summary>
        /// Тестирует создание объекта.
        /// </summary>
        [Fact]
        public void Create_WithoutBindings_Success()
        {
            Distribution distribution = CreateDistribution();

            AssertDistribution(distribution, Owners, AvailableLifeCycles, LifeCycleStateRules);
            Assert.IsType<DistributionCreated>(GetUncommitedChanges(distribution)[0]);
            Assert.True(distribution.GetUncommitedChanges().Count == 1);
        }

        /// <summary>
        /// Тестирует создание дистрибутива с добавлением BuildBindings.
        /// </summary>
        [Fact]
        public void Create_WithBindings_Success()
        {
            var buildsBindings = FakeGenerator.BuildsBindings.Generate(2);
            Distribution distribution = CreateDistribution();

            distribution.AddBuildBindings(buildsBindings, FakeGenerator.GetUser().Sid);

            AssertDistribution(distribution, Owners, AvailableLifeCycles, LifeCycleStateRules);
            Assert.IsType<DistributionCreated>(GetUncommitedChanges(distribution)[0]);
            Assert.IsType<BuildBindingsAdded>(GetUncommitedChanges(distribution)[1]);
            AssertDistributionBuildBinding(distribution, GetUncommitedChanges(distribution)[1], buildsBindings);
            Assert.True(distribution.GetUncommitedChanges().Count == 2);
        }

        /// <summary>
        /// Тестирует восстановление объекта.
        /// </summary>
        [Fact]
        public void RestoreFromEvents_Success()
        {
            Guid distributionId = Guid.NewGuid();
            Metadata metadata = new Metadata() { AggregateId = distributionId };
            List<User> owners = FakeGenerator.Users.Generate(4);
            LifeCycleState[] lifeCycleStates = Enum.GetValues(typeof(LifeCycleState)).Cast<LifeCycleState>().ToArray();
            string[] lifeCycleStateRules = lifeCycleStates.Select(x => x.ToString()).ToArray();
            BuildsBinding buildsBinding1 = FakeGenerator.GetBuildsBinding();
            BuildsBinding buildsBinding2 = FakeGenerator.GetBuildsBinding();
            BuildsBinding buildsBinding3 = FakeGenerator.GetBuildsBinding();
            Guid[] newBuilds1 = { Guid.NewGuid(), Guid.NewGuid() };
            Guid[] newBuilds2 = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var fakeBuildBinding = new Dictionary<BuildsBinding, Guid[]>()
            {
                [buildsBinding1] = new Guid[] { Guid.NewGuid(), Guid.NewGuid() },
                [buildsBinding2] = new Guid[] {},
                [buildsBinding3] = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() },
            };
            
            var distribution = new Distribution(new List<Event>
            {
                new DistributionCreated(distributionId, DistributionName, new User[] { owners[0] }, new LifeCycleState[] { lifeCycleStates[0] }, Array.Empty<string>(), metadata),
                
                new DistributionOwnersAdded(new User[] { owners[1], owners[2] }, metadata),
                new DistributionOwnersRemoved(new User[] { owners[0], owners[2] }, metadata),
                new DistributionOwnersAdded(new User[] { owners[0], owners[2], owners[3] }, metadata),
                
                new DistributionAvailableLifeCyclesAdded(new LifeCycleState[] { lifeCycleStates[1], lifeCycleStates[2] }, metadata),
                new DistributionAvailableLifeCyclesRemoved(new LifeCycleState[] { lifeCycleStates[2], lifeCycleStates[1], lifeCycleStates[0] }, metadata),
                new DistributionAvailableLifeCyclesAdded(lifeCycleStates, metadata),
                
                new DistributionLifeCycleStateRulesAdded(new string[] { lifeCycleStateRules[1], lifeCycleStateRules[2] }, metadata),
                new DistributionLifeCycleStateRulesRemoved(new string[] { lifeCycleStateRules[2] }, metadata),
                new DistributionLifeCycleStateRulesRemoved(new string[] { lifeCycleStateRules[1] }, metadata),
                new DistributionLifeCycleStateRulesAdded(lifeCycleStateRules, metadata),
                
                new BuildBindingsAdded(fakeBuildBinding, metadata),
                new BuildBindingsRemoved(fakeBuildBinding.Take(1).ToDictionary(x => x.Key, x=> x.Value), metadata),
                new BuildsToBuildsBindingAdded(buildsBinding2, newBuilds1, metadata),
                new BuildFromBuildsBindingRemoved(buildsBinding3, fakeBuildBinding[buildsBinding3].Take(3).ToArray(), metadata),
                new BuildsToBuildsBindingAdded(buildsBinding2, newBuilds2, metadata),
                new BuildBindingsAdded(fakeBuildBinding.Take(1).ToDictionary(x => x.Key, x=> x.Value), metadata),
                new BuildsToBuildsBindingAdded(buildsBinding1, newBuilds1, metadata),
            });
        
        
            AssertDistribution(distribution, owners, lifeCycleStates, lifeCycleStateRules);
            Assert.Equal(new Dictionary<BuildsBinding, List<Guid>>()
            {
                [buildsBinding1] = fakeBuildBinding.Take(1).First().Value.Union(newBuilds1).ToList(),
                [buildsBinding2] = newBuilds1.Union(newBuilds2).ToList(),
                [buildsBinding3] = new() { fakeBuildBinding[buildsBinding3].Last() },
            }, distribution.BindingToBuilds);
            Assert.Empty(distribution.GetUncommitedChanges());
        }
        
        
        /// <summary>
        /// Тест проверяет исключение CreateDistributionException, если создать объект с некорректными данными.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="owners">Ответственные.</param>
        /// <param name="availableLifeCycles">Список доступных состояний сборок у дистрибутива.</param>
        /// <param name="lifeCycleStateRules">Имена правил перехода по состояниям сборки.</param>
        [Theory]
        [MemberData(nameof(InvalidArgsDistribution))]
        public void Throws_InvalidData_CreateDistributionException(string name, IEnumerable<User> owners, IEnumerable<LifeCycleState> availableLifeCycles, IEnumerable<string> lifeCycleStateRules) => 
            Assert.Throws<CreateDistributionException>(() => new Distribution(name, owners, availableLifeCycles, lifeCycleStateRules));

        private void AssertDistribution(Distribution distribution, IEnumerable<User> owners, IEnumerable<LifeCycleState> availableLifeCycles, IEnumerable<string> ruleNames)
        {
            Assert.Equal(DistributionName, distribution.Name);
            Assert.Equal(owners.OrderBy(x => x.Sid), distribution.Owners.OrderBy(x => x.Sid));
            Assert.Equal(availableLifeCycles.OrderBy(x => x), distribution.AvailableLifeCycles.OrderBy(x => x));
            Assert.Equal(ruleNames.OrderBy(x => x), distribution.LifeCycleStateRules.OrderBy(x => x));
        }

        private void AssertDistributionBuildBinding(Distribution distribution, Event @event, List<BuildsBinding> exceptedBuildBindings)
        {
            var buildBindingsAdded = @event as BuildBindingsAdded;

            Assert.Equal(distribution.Id, buildBindingsAdded.Metadata.AggregateId);
            Assert.Equal(exceptedBuildBindings, buildBindingsAdded.BindingToBuilds.Select(x => x.Key));
            Assert.True(buildBindingsAdded.BindingToBuilds.Select(x => x.Value).SelectMany(x => x).IsEmpty());
        }
    }
}
