using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Messages;
using ReleaseBoard.Messages.EventMessageHandlers;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;
using static Moq.It;

namespace ReleaseBoard.UnitTests.Messages.EventHandlers
{
    /// <summary>
    /// Тесты для <see cref="NewBuildEventHandler"/>.
    /// </summary>
    public class NewBuildEventHandlerTests
    {
        private readonly Mock<ILogger<NewBuildEventHandler>> fakeLogger = new();
        private readonly Mock<IMediator> mediatorMock = new(MockBehavior.Loose);
        private readonly IEventMessageHandler<NewBuildEvent> newBuildHandler;
        private readonly Mock<IValidator<NewBuildEvent>> validator = new();
        private object result;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public NewBuildEventHandlerTests()
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Callback<object, CancellationToken>((o, ct) =>
                    {
                        result = o;
                    }
                )
                .ReturnsAsync(new object());

            newBuildHandler = new NewBuildEventHandler(
                mediatorMock.Object,
                new Mapper(new MapperConfiguration(x => x.AddProfile(new BuildEventMessagesMappingProfile()))),
                validator.Object,
                fakeLogger.Object);
        }


        /// <summary>
        /// Тест проверяет, что отправляется ли команда CreateBuild.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handle_ValidEventMessages_SendCreateBuildCommand()
        {
            validator
                .Setup(x => x.ValidateAsync(It.IsAny<NewBuildEvent>(), IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ValidationResult()));
            var build = FakeGenerator.BuildsDto.Generate();

            await newBuildHandler.Handle(new NewBuildEvent { Build = build });

            Assert.IsType<CreateBuild>(result);
            var createBuild = result as CreateBuild;
            Assert.Equal(build.DistributionId, createBuild.DistributionId);
            Assert.Equal(build.Location, createBuild.Location);
            Assert.Equal(build.Number, createBuild.Number.ToString());
        }

        /// <summary>
        /// Тест проверяет, что отправляется ли команда CreateBuild, если валидация не прошла.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handle_InvalidEvent_NotSendCommand()
        {
            validator
                .Setup(x => x.ValidateAsync(It.IsAny<NewBuildEvent>(), IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>() {new("some property", "some message")}));
            var build = FakeGenerator.BuildsDto.Generate();

            await newBuildHandler.Handle(new NewBuildEvent { Build = build });

            Assert.Null(result);
        }
    }
}
