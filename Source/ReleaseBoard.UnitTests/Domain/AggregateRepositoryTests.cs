using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Exceptions;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.Infrastructure.Data.MongoDb.EventStore;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain
{
    /// <summary>
    /// Тесты для класса <see cref="AggregateRepository"/>.
    /// </summary>
    public class AggregateRepositoryTests
    {
        private readonly Guid id = Guid.Parse("96ed1bb5-c338-417b-848a-9a97a9feb867");
        private readonly IAggregateRepository aggregateRepository;
        private readonly MemoryChangesetRepository memoryChangesetRepository = new();

        /// <summary>
        /// Конструктор.
        /// </summary>
        public AggregateRepositoryTests()
        {
            var eventStore = new EventStore(memoryChangesetRepository, new Mock<IMediator>().Object);
            aggregateRepository = new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object);

            memoryChangesetRepository.EventsStorage.Add(new Changeset()
            {
                Version = 1,
                Name = $"{nameof(Build)}-{id}",
                Data = new List<Event>()
                {
                    BuildCreated
                }
            });
        }

        private BuildCreated BuildCreated => new BuildCreated(
            id,
            DateTime.Now,
            Guid.Parse("96ed1bb5-c338-417b-848a-9a97a9feb868"),
            "some_path",
            new VersionNumber("1.0.0.1"),
            null,
            LifeCycleState.Build,
            BuildSourceType.Pdc,
            new string[] {},
            new Metadata()
            {
                AggregateId = id
            }
        );

        /// <summary>
        /// Тест проверяет загрузку агрегата.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task LoadAggregateById_CreateBuild_LoadedAggregateById()
        {
            Build build = await aggregateRepository.LoadById<Build>(id);

            Assert.Equal(id, build.Id);
            Assert.Equal(BuildCreated.Location, build.Location);
            Assert.Equal(BuildCreated.Number, build.Number);
            Assert.Equal(BuildCreated.SourceType, build.SourceType);
        }

        /// <summary>
        /// Тест проверяет изменился ли агрегат.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task UpdateAggregateById_UpdateBuild_Success()
        {
            await aggregateRepository
                .UpdateById<Build>(id,
                    b =>
                    {
                        b.Update("new_path",  new VersionNumber("1.0.1"), new VersionNumber("1.0.1.2"),new List<string>(), DateTime.MaxValue);
                        return Task.CompletedTask;
                    } );


            Build build = await aggregateRepository.LoadById<Build>(id);
            Assert.Equal(id, build.Id);
            Assert.Equal("new_path", build.Location);
        }

        /// <summary>
        /// Тест проверяет генерацию исключения при отсутствии агрегата сборки.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task LoadAggregateById_BuildDoesnotExist__DomainException()
        {
            await Assert.ThrowsAsync<DomainException>(() => aggregateRepository.LoadById<Build>(Guid.NewGuid()));
        }
    }
}
