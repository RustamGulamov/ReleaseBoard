using System;
using System.Linq.Expressions;
using System.Threading;
using FluentValidation.TestHelper;
using Moq;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Messages.Validators;
using ReleaseBoard.ReadModels;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Messages.Validators
{
    /// <summary>
    /// Тесты для валидатора.
    /// </summary>
    public class DeleteBuildEventValidatorTests
    {
        private readonly Mock<IReadOnlyRepository<BuildReadModel>> buildReadOnlyRepository = new();
        private readonly DeleteBuildEventValidator validator;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public DeleteBuildEventValidatorTests()
        {
            validator = new DeleteBuildEventValidator(buildReadOnlyRepository.Object);
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если сборка существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_BuildIsExist_ShouldNotHaveError()
        {
            (BuildReadModel build, DeleteBuildEvent deleteBuildEvent) = Generate();
            buildReadOnlyRepository
                .Setup(x => x.Any(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(true);

            var result = validator.TestValidate(deleteBuildEvent);

            result.ShouldNotHaveAnyValidationErrors();
        }

        /// <summary>
        /// Тест проверяет валидатора <see cref="NewBuildEventValidator"/>, если сборка не существует в базе.
        /// </summary>
        [Fact]
        public void TestValidate_BuildIsNotExist_ShouldHaveError()
        {
            (BuildReadModel _, DeleteBuildEvent deleteBuildEvent) = Generate();
            buildReadOnlyRepository
                .Setup(x => x.Any(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(false);

            var result = validator.TestValidate(deleteBuildEvent);

            result.ShouldHaveAnyValidationError();
        }

        private (BuildReadModel build, DeleteBuildEvent deleteBuildEvent) Generate()
        {
            var build = FakeGenerator.Build.Generate();
            var deleteBuildEvent = new DeleteBuildEvent()
            {
                BuildId = build.Id,
                DeleteDate = DateTime.Now
            };

            return (build, deleteBuildEvent);
        }
    }
}
