using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Messages;
using ReleaseBoard.Messages.EventMessageHandlers;
using ReleaseBoard.Messages.Validators;
using ReleaseBoard.ReadModels;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Messages.EventHandlers
{
    /// <summary>
    /// Тесты для <see cref="UpdateBuildEventHandler"/>.
    /// </summary>
    public class UpdateBuildEventHandlerTests
    {
        private readonly Mock<ILogger<UpdateBuildEventHandler>> fakeLogger = new();
        private readonly Mock<IMediator> mediatorMock = new(MockBehavior.Loose);
        private readonly IEventMessageHandler<UpdateBuildEvent> updateBuildHandler;
        private readonly Mock<IValidator<UpdateBuildEvent>> validator = new();
        private readonly Mock<IReadOnlyRepository<BuildReadModel>> buildRepositoryMock = new();
        private object result;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public UpdateBuildEventHandlerTests()
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Callback<object, CancellationToken>((o, ct) =>
                    {
                        result = o;
                    }
                )
                .ReturnsAsync(new object());
            
            updateBuildHandler = new UpdateBuildEventHandler(
                mediatorMock.Object,
                new Mapper(new MapperConfiguration(x => x.AddProfile(new BuildEventMessagesMappingProfile()))),
                validator.Object,
                fakeLogger.Object);
        }


        /// <summary>
        /// Тест проверяет, что отправляется ли команда UpdateBuild.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handle_ValidEventMessages_SendUpdateBuildCommand()
        {
            validator
                .Setup(x => x.ValidateAsync(It.IsAny<UpdateBuildEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ValidationResult()));
            UpdateBuildEvent updateBuildEvent = GenerateUpdateBuildEvent();

            await updateBuildHandler.Handle(updateBuildEvent);

            Assert.IsType<UpdateBuild>(result);
            var updateBuild = result as UpdateBuild;
            Assert.Equal(updateBuildEvent.BuildId, updateBuild.BuildId);
            Assert.Equal(updateBuildEvent.Date, updateBuild.ChangeDate);
            Assert.Equal(updateBuildEvent.Location, updateBuild.Location);
            Assert.Equal(updateBuildEvent.Suffixes, updateBuild.Suffixes);
        }
        
        /// <summary>
        /// Тест проверяет, отправление событии DeleteEvent, если существует другая сборка с таким номером и суффиксом.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handle_ExistSameBuild_SendDeleteEvent()
        {
            validator
                .Setup(x => x.ValidateAsync(It.IsAny<UpdateBuildEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult() { Errors =
                {
                    new ValidationFailure("a", "error")
                    {
                        ErrorCode = ValidatorErrorCodes.ExistSameBuild
                    }
                }});
            UpdateBuildEvent updateBuildEvent = GenerateUpdateBuildEvent();
            
            await updateBuildHandler.Handle(updateBuildEvent);

            Assert.IsType<DeleteBuild>(result);
            var deleteEvent = result as DeleteBuild;
            Assert.Equal(updateBuildEvent.BuildId, deleteEvent.BuildId);
        }

        private UpdateBuildEvent GenerateUpdateBuildEvent()
        {
            var build = FakeGenerator.Build.Generate();
            var updateEvent = new UpdateBuildEvent()
            {
                BuildId = build.Id,
                Date = DateTime.Now,
                Location = build.Location + "_RC",
                Number = "1.2.3.4",
                ReleaseNumber = "1.2.3",
                Suffixes = build.Suffixes.Append("_RC").ToList()
            };

            return updateEvent;
        }
    }
}
