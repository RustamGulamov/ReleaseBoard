using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using FluentValidation.TestHelper;
using Moq;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels;
using ReleaseBoard.UnitTests.DataGenerators;
using ReleaseBoard.Web.ApiModels.DistributionModels;
using ReleaseBoard.Web.Validators.ModelValidators;
using Xunit;

namespace ReleaseBoard.UnitTests
{
    /// <summary>
    /// Тесты для валидатора <see cref="DistributionModelValidator"/>.
    /// </summary>
    public class DistributionModelValidatorTests
    {
        private readonly Mock<IReadOnlyRepository<BuildReadModel>> buildsRepositoryMock = new Mock<IReadOnlyRepository<BuildReadModel>>();
        private readonly Mock<IReadOnlyRepository<DistributionReadModel>> distributionRepositoryMock = new Mock<IReadOnlyRepository<DistributionReadModel>>();
        private readonly Mock<ILighthouseServiceApi> lighthouseServiceMock = new Mock<ILighthouseServiceApi>();
        private readonly DistributionModelValidator validator;
        private readonly DistributionModel distributionModel;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public DistributionModelValidatorTests()
        {
            validator = new DistributionModelValidator(distributionRepositoryMock.Object, buildsRepositoryMock.Object, lighthouseServiceMock.Object);
            distributionModel = new DistributionModel
            {
                Id = Guid.NewGuid(),
                Name = FakeGenerator.GetString().ToLower(),
                OwnersSids = new List<string> { FakeGenerator.GetString() }
            };
        }

        /// <summary>
        /// Коллекции некорректных данных для CommonRules.
        /// </summary>
        public static IEnumerable<object[]> InvalidDataForCommonRulesDistributionModels => new[]
        {
            new object[] { new DistributionModel { Name = string.Empty }, nameof(DistributionModel.Name), "Create" },
            new object[] { new DistributionModel { Name = "a" }, nameof(DistributionModel.Name), "Create"  },
            new object[] { new DistributionModel { Name = string.Join(string.Empty, Enumerable.Repeat(1, 257).Select(x => "a")) }, nameof(DistributionModel.Name), "Create" },
            new object[] { new DistributionModel { OwnersSids = new List<string>(), Name = "test1" }, nameof(DistributionModel.OwnersSids), "Create"  },
            new object[] { new DistributionModel { OwnersSids = new List<string> { string.Empty }, Name = "test1" }, nameof(DistributionModel.OwnersSids), "Create"  },
            new object[]
            {
                new DistributionModel
                {
                    OwnersSids = new List<string> { "sid" },
                    Name = "test1",
                    BuildBindings = new BuildBindingModel[] {new BuildBindingModel(), new BuildBindingModel(), }
                },
                nameof(DistributionModel.BuildBindings), "Create"
            },
            new object[]
            {
                new DistributionModel
                {
                    OwnersSids = new List<string> { "sid" },
                    Name = "test1",
                    BuildBindings = new BuildBindingModel[] { new BuildBindingModel() },
                    ProjectBindings = new ProjectBindingModel[] { new ProjectBindingModel(), new ProjectBindingModel(), }
                },
                nameof(DistributionModel.ProjectBindings), "Create"
            },
            new object[]
            {
                new DistributionModel
                {
                    OwnersSids = new List<string> { "notValidOwner" },
                    Name = "test1",
                    BuildBindings = new BuildBindingModel[] { new BuildBindingModel() },
                    ProjectBindings = new ProjectBindingModel[] { new ProjectBindingModel(), }
                },
                nameof(DistributionModel.OwnersSids), "Create"
            },
        };

        /// <summary>
        /// Тест проверяет валидацию, если входные данные некорректные.
        /// </summary>
        /// <param name="distributionModel"><see cref="DistributionModel"/>.</param>
        /// <param name="propertyName">Имя свойство.</param>
        /// <param name="ruleSet">Имя правил.</param>
        [Theory]
        [MemberData(nameof(InvalidDataForCommonRulesDistributionModels))]
        public void TestValidate_InvalidCommonData_ShouldHaveError(DistributionModel distributionModel, string propertyName, string ruleSet)
        {
            lighthouseServiceMock.Setup(x => x.IsValidUsers(It.IsAny<string[]>())).ReturnsAsync(false);

            var result = validator.TestValidate(distributionModel, ruleSet);

            result.ShouldHaveValidationErrorFor(propertyName);
        }

        /// <summary>
        /// Тест проверяет валидацию обновления дистрибутива, если не существует идентификатор дистрибутива.
        /// </summary>
        [Fact]
        public void TestValidate_UpdateDistribution_DoesNotExistId_ShouldHaveError()
        {
            var result = validator.TestValidate(distributionModel, "Update");

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        /// <summary>
        /// Тест проверяет валидацию обновления дистрибутива, если удалено LifeCycleState, если сборка с таким статусом существует.
        /// </summary>
        [Fact]
        public void TestValidate_UpdateDistribution_ExistBuildWithDeletedLifeCycleState_ShouldHaveError()
        {
            distributionModel.AvailableLifeCycles = new LifeCycleState[] { LifeCycleState.Release };
            lighthouseServiceMock.Setup(x => x.IsValidUsers(It.IsAny<string[]>())).ReturnsAsync(true);
            buildsRepositoryMock
                .Setup(x => x.Any(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(true);
            distributionRepositoryMock
                .Setup(x => x.Any(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(true);
            distributionRepositoryMock
                .Setup(x => x.Query(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new DistributionReadModel
                {
                    Id = distributionModel.Id,
                    AvailableLifeCycles = new List<LifeCycleState>
                    {
                        LifeCycleState.Release,
                        LifeCycleState.Build
                    }
                });

            var result = validator.TestValidate(distributionModel, "Update");

            result.ShouldHaveValidationErrorFor(x => x.AvailableLifeCycles);
        }

        /// <summary>
        /// Тест проверяет валидацию обновления дистрибутива, если имя дистрибутива изменилось.
        /// </summary>
        [Fact]
        public void TestValidate_UpdateDistribution_NameIsChanged_ShouldHaveError()
        {
            lighthouseServiceMock.Setup(x => x.IsValidUsers(It.IsAny<string[]>())).ReturnsAsync(true);
            distributionRepositoryMock
                .Setup(x => x.Any(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync((Expression<Func<DistributionReadModel, bool>> predicate, CancellationToken token) => predicate.Compile()(
                    new DistributionReadModel { Id = distributionModel.Id, Name = distributionModel.Name }));
            distributionRepositoryMock
                .Setup(x => x.Query(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new DistributionReadModel
                {
                    Id = distributionModel.Id,
                    Name = FakeGenerator.GetString()
                });

            var result = validator.TestValidate(distributionModel, "Update");

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        /// <summary>
        /// Тест проверяет валидацию обновления дистрибутива, если все данные корректные.
        /// </summary>
        [Fact]
        public void TestValidate_UpdateDistribution_DataIsValid_ShouldHaveNotError()
        {
            lighthouseServiceMock.Setup(x => x.IsValidUsers(It.IsAny<string[]>())).ReturnsAsync(true);
            distributionRepositoryMock
                .Setup(x => x.Any(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(true);
            distributionRepositoryMock
                .Setup(x => x.Query(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(new DistributionReadModel
                {
                    Id = distributionModel.Id,
                    Name = distributionModel.Name,
                    Owners = new List<User> { FakeGenerator.GetUser() },
                    AvailableLifeCycles = distributionModel.AvailableLifeCycles.ToList(),
                });

            var result = validator.TestValidate(distributionModel, "Update");

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
