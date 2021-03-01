using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using ReleaseBoard.Common.Contracts.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Messages;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions.EventHandlers
{
    /// <summary>
    /// Тесты для класса <see cref="BuildBindingsEventHandler"/>.
    /// </summary>
    public class BuildBindingsEventHandlerTests
    {
        private readonly Guid distributionId = Guid.NewGuid();
        private readonly BuildBindingsEventHandler buildBindingsEventHandler;
        private readonly Mock<IMediator> mediatorMock = new();
        private readonly Mock<IJobStorageService> jobStorageMock = new();
        private readonly Mock<IReadOnlyRepository<BuildReadModel>> buildRepositoryMock = new();
        private List<object> results = new();

        /// <summary>
        /// Конструктор.
        /// </summary>
        public BuildBindingsEventHandlerTests()
        {
            jobStorageMock.Setup(x => x.GetJobResult<IEnumerable<BuildDto>>(It.IsAny<string>()))
                .Returns(new List<BuildDto>()
                {
                    new() { DistributionId = Guid.Empty, Location = "some_location", Number = "1.0.2.3", ReleaseNumber = "1.0.2.3" },
                    new() { DistributionId = Guid.Empty, Location = "some_location1", Number = "1.0.2.5", ReleaseNumber = "1.0.2.5" }
                });

            mediatorMock
                .Setup(x => x.Send(It.IsAny<ICommand>(), It.IsAny<CancellationToken>()))
                .Callback<object, CancellationToken>((o, ct) =>
                    {
                        results.Add(o);
                    }
                )
                .Returns(Task.FromResult(new Unit()));
            
            buildRepositoryMock
                .Setup(x => x.Query(It.IsAny<Expression<Func<BuildReadModel, bool>>>(), CancellationToken.None))
                .Returns(() => Task.FromResult<BuildReadModel>(null));
            
            buildBindingsEventHandler = new BuildBindingsEventHandler(
                new Mock<ILogger<BuildBindingsEventHandler>>().Object,
                new Mock<IBackgroundJobClient>().Object,
                jobStorageMock.Object,
                mediatorMock.Object,
                new Mapper(new MapperConfiguration(x => x.AddProfile(new BuildEventMessagesMappingProfile()))),
                buildRepositoryMock.Object
            );
        }

        /// <summary>
        /// Тест проверяет отправление комманд, если сбока не существует в базе.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task SendCommands_BuildsDoNotExists_SendCreateBuildCommands()
        {
            string parseJobId = "parseId";
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel>());

            await buildBindingsEventHandler.SendCommandsForParsedBuilds(parseJobId, distributionId);

            Assert.Equal(2, results.Count);
            Assert.IsType<CreateBuild>(results.First());
            Assert.Equal(distributionId, (results.First() as CreateBuild).DistributionId);
        }

        /// <summary>
        /// Тест проверяет отправление комманды MarkAsTrackedBuild, если сбока существует в базе и статус UnTracked.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task SendCommands_BuildsExistsAndUnTracked_SendMarkTrackedBuildCommands()
        {
            string parseJobId = "parseId";
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel>()
                {
                    new()
                    {
                        DistributionId = Guid.Empty,
                        Id = Guid.NewGuid(),
                        IsUnTracked = true,
                    }
                });

            await buildBindingsEventHandler.SendCommandsForParsedBuilds(parseJobId, distributionId);

            Assert.IsType<MarkAsTrackedBuild>(results[0]);
            Assert.IsType<UpdateBuild>(results[1]);
            Assert.Equal(distributionId, (results[0] as MarkAsTrackedBuild).DistributionId);
        }

        /// <summary>
        /// Тест проверяет, что команды не отправляются, если сборки существуют и статус Tracked.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task SendCommands_BuildsExistsAndTracked_IgnoreBuilds()
        {
            string parseJobId = "parseId";
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetSameBuilds.Query>(), CancellationToken.None))
                .ReturnsAsync(new List<BuildReadModel>()
                {
                    new()
                    {
                        DistributionId = distributionId,
                        Id = Guid.NewGuid(),
                        IsUnTracked = false,
                    }
                });

            await buildBindingsEventHandler.SendCommandsForParsedBuilds(parseJobId, distributionId);

            Assert.Empty(results);
        }

        /// <summary>
        /// Тест проверяет, что команды не отправляются, если GetJobResult возращает null.
        /// </summary>
        [Fact]
        public void SendCommands_GetJobResultReturnedNull_NotCommands()
        {
            jobStorageMock.Setup(x => x.GetJobResult<IEnumerable<BuildDto>>(It.IsAny<string>()))
                .Returns((IEnumerable<BuildDto>)null);

            Assert.Empty(results);
        }
    }
}
