using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using FluentValidation.TestHelper;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using MediatR;
using Moq;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Messages.Validators;
using ReleaseBoard.ReadModels;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Messages.Validators
{
    /// <summary>
    /// Тесты для <see cref="UpdateBuildEventValidator"/>.
    /// </summary>
    public class UpdateBuildEventValidatorTests
    {
        private readonly Mock<IReadOnlyRepository<BuildReadModel>> buildReadOnlyRepository = new();
        private readonly Mock<IMediator> mediatorMock = new(MockBehavior.Loose);
        private readonly UpdateBuildEventValidator validator;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public UpdateBuildEventValidatorTests()
        {
            validator = new UpdateBuildEventValidator(buildReadOnlyRepository.Object, mediatorMock.Object);
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если сборка существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_BuildIsExist_ShouldNotHaveError()
        {
            var existedBuild = FakeGenerator.Build.Generate(1).First();
            UpdateBuildEvent updateBuildEvent = new()
            {
                BuildId = existedBuild.Id, 
                Location = existedBuild.Location, Number = existedBuild.Number, 
                Suffixes = existedBuild.Suffixes.Select(x => x.ToLower()).ToList()
            };
            buildReadOnlyRepository
                .Setup(x => x.Query(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(existedBuild);
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel> { existedBuild });
            
            TestValidationResult<UpdateBuildEvent> result = validator.TestValidate(updateBuildEvent);

            result.ShouldNotHaveAnyValidationErrors();
        }
        
        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если сборка не существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_BuildDoesNotExist_ShouldHaveError()
        {
            UpdateBuildEvent updateBuildEvent = new()
            {
                BuildId = Guid.NewGuid(), 
                Location = "some_location", Number = "1.1.1", 
                Suffixes = new List<string>() { "R", "alpha" }
            };
            buildReadOnlyRepository
                .Setup(x => x.Query(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync((BuildReadModel)null);

            TestValidationResult<UpdateBuildEvent> result = validator.TestValidate(updateBuildEvent);

            result.ShouldHaveAnyValidationError();
        }
        
        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если похожая(суффиксы с нижним регистром) сборка существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_SameBuildWithToLowerSuffixesIsExist_ShouldHaveError()
        {
            var existedBuild = FakeGenerator.Build.Generate(1).First();
            UpdateBuildEvent updateOtherBuildEvent = new()
            {
                BuildId = Guid.NewGuid(), 
                Location = "some_other_location", 
                Number = existedBuild.Number, 
                Suffixes = existedBuild.Suffixes.Select(x => x.ToLower()).ToList()
            };
            buildReadOnlyRepository
                .Setup(x => x.Query(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(existedBuild);
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel> { existedBuild });

            TestValidationResult<UpdateBuildEvent> result = validator.TestValidate(updateOtherBuildEvent);

            result.ShouldHaveAnyValidationError();
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="UpdateBuildEventValidator"/>, если указаны неверные NewPath и NewSuffixes.
        /// </summary>
        [Fact]
        public void TestValidate_UpdateBuildEventIsNotValid_ShouldHaveValidationErrorFor()
        {
            UpdateBuildEvent updateBuildEvent = new UpdateBuildEvent();

            validator.ShouldHaveValidationErrorFor(x => x.BuildId, updateBuildEvent);
        }
    }
}
