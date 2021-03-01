using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentValidation.TestHelper;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using ReleaseBoard.Common.Contracts.Common.Models;
using MediatR;
using Moq;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Messages.Validators;
using ReleaseBoard.ReadModels;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Messages.Validators
{
    /// <summary>
    /// Тесты для валидатора <see cref="NewBuildEventValidator"/>.
    /// </summary>
    public class NewBuildEventValidatorTests
    {
        private readonly Mock<IMediator> mediatorMock = new(MockBehavior.Loose);
        private readonly NewBuildEventValidator validator;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public NewBuildEventValidatorTests()
        {
            validator = new NewBuildEventValidator(mediatorMock.Object);
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если сборка не существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_BuildIsNotExist_ShouldNotHaveError()
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel>());

            var result = validator.TestValidate(new NewBuildEvent { Build = FakeGenerator.BuildsDto.Generate() });

            result.ShouldNotHaveAnyValidationErrors();
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если указаны пустые Number и ReleaseNumber.
        /// </summary>
        [Fact]
        public void TestValidate_BuildIsNull_ShouldHaveValidationErrorFor()
        {
            var newBuildEvent = new NewBuildEvent { Build = new BuildDto() };

            validator.ShouldHaveValidationErrorFor(x => x.Build, null as BuildDto);
            validator.ShouldHaveValidationErrorFor(x => x.Build.Number, newBuildEvent);
            validator.ShouldHaveValidationErrorFor(x => x.Build.DistributionId, newBuildEvent);
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если указаны пустые Number и ReleaseNumber.
        /// </summary>
        [Fact]
        public void TestValidate_BuildIsValid_ShouldNotHaveValidationErrorFor()
        {
            var newBuildEvent = new NewBuildEvent { Build = FakeGenerator.BuildsDto.Generate() };

            validator.ShouldNotHaveValidationErrorFor(x => x.Build.CreateDate, newBuildEvent);
            validator.ShouldNotHaveValidationErrorFor(x => x.Build.Number, newBuildEvent);
            validator.ShouldNotHaveValidationErrorFor(x => x.Build.ReleaseNumber, newBuildEvent);
            validator.ShouldNotHaveValidationErrorFor(x => x.Build.Suffixes, newBuildEvent);
            validator.ShouldNotHaveValidationErrorFor(x => x.Build.DistributionId, newBuildEvent);
        }
        
        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если похожая сборка существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_SameBuildExist_ShouldHaveError()
        {
            var newBuild = FakeGenerator.BuildsDto.Generate(1).First();
            var existBuild = new BuildReadModel() { Number = newBuild.Number, Suffixes = newBuild.Suffixes.Reverse().ToList() };
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel> { existBuild });

            TestValidationResult<NewBuildEvent> result = validator.TestValidate(new NewBuildEvent { Build = newBuild });

            result.ShouldHaveAnyValidationError();
        }
    }
}
