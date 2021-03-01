using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.Distributions.Events.Owners;
using ReleaseBoard.Domain.Distributions.Events.ProjectBindings;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions
{
    /// <summary>
    /// Тесты на обновление дистрибутива.
    /// </summary>
    public class DistributionUpdateTests: DistributionTests
    {
        /// <summary>
        /// Тестирует создание объекта.
        /// </summary>
        [Fact]
        public void AddMultiplyBuildBinding_AddedOnlyOneElement()
        {
            var buildBindings = FakeGenerator.BuildsBindings.Generate(3);
            Distribution distribution = CreateDistribution();

            distribution.AddBuildBindings(buildBindings, null);
            distribution.AddBuildBindings(buildBindings, null);

            Assert.Equal(DistributionName, distribution.Name);
            Assert.NotEmpty(distribution.Owners);
            Assert.NotEmpty(distribution.AvailableLifeCycles);
            Assert.NotEmpty(distribution.LifeCycleStateRules);
            Assert.Equal(buildBindings, distribution.BindingToBuilds.Keys);
            Assert.IsType<DistributionCreated>(GetUncommitedChanges(distribution)[0]);
            Assert.IsType<BuildBindingsAdded>(GetUncommitedChanges(distribution)[1]);
            Assert.Equal(2, distribution.GetUncommitedChanges().Count());
        }

        /// <summary>
        /// Тестирует добавление объекта привязки проектов.
        /// </summary>
        [Fact]
        public void AddMultiplyProjectBinding_AddedOnlyOneElement()
        {
            Distribution distribution = CreateDistribution();
            Project project = new Project(Guid.NewGuid(), "Rxxxxxxxx", "Rxxxxxxxx");
            string versionMask = "1.4.**";

            distribution.AddProjectBinding(new ProjectBinding(versionMask, project), null);
            distribution.AddProjectBinding(new ProjectBinding(versionMask, project), null);

            Assert.Equal(DistributionName, distribution.Name);
            Assert.NotEmpty(distribution.Owners);
            Assert.NotEmpty(distribution.AvailableLifeCycles);
            Assert.NotEmpty(distribution.LifeCycleStateRules);
            Assert.IsType<DistributionCreated>(distribution.GetUncommitedChanges().First());
            Assert.IsType<ProjectBindingAdded>(distribution.GetUncommitedChanges().Last());
            Assert.Equal(2, distribution.GetUncommitedChanges().Count);
            Assert.Single(distribution.ProjectBindings);
            Assert.Equal(project.Name, distribution.ProjectBindings[0].Project.Name);
            Assert.Equal(versionMask, distribution.ProjectBindings[0].MaskProjectVersion);
        }

        /// <summary>
        /// Тестирует обновление свойств дистрибутива.
        /// </summary>
        [Fact]
        public void UpdateName_PropertiesChanged()
        {
            Distribution distribution = CreateDistribution();
            string newName = FakeGenerator.GetString();

            distribution.UpdateName(newName);

            Assert.Equal(newName, distribution.Name);
            Assert.IsType<DistributionNameUpdated>(distribution.GetUncommitedChanges().Last());
        }

        /// <summary>
        /// Тестирует обновление свойств дистрибутива.
        /// </summary>
        [Fact]
        public void AddOwners_PropertiesChanged()
        {
            Distribution distribution = CreateDistribution();
            User newOwner = FakeGenerator.GetUser();
            Assert.DoesNotContain(newOwner, distribution.Owners);

            distribution.AddOwners(new List<User> { newOwner });

            Assert.Contains(newOwner, distribution.Owners);
            Assert.IsType<DistributionOwnersAdded>(distribution.GetUncommitedChanges().Last());
        }

        /// <summary>
        /// Тестирует обновление свойств дистрибутива.
        /// </summary>
        [Fact]
        public void RemoveOwners_PropertiesChanged()
        {
            Distribution distribution = CreateDistribution();
            var ownerToDelete = distribution.Owners[0];
            
            distribution.RemoveOwners(new List<User>() { ownerToDelete});

            Assert.DoesNotContain(ownerToDelete, distribution.Owners);
            Assert.IsType<DistributionOwnersRemoved>(distribution.GetUncommitedChanges().Last());
        }
    }
}
