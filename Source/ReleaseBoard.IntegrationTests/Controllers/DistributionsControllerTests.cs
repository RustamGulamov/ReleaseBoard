using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using Hangfire.Common;
using Hangfire.States;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Infrastructure.Common.Extensions;
using Moq;
using ReleaseBoard.IntegrationTests.AutoFixture;
using ReleaseBoard.IntegrationTests.ProjectTestServer;
using ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase;
using ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants;
using ReleaseBoard.IntegrationTests.ProjectTestServer.Extensions;
using ReleaseBoard.IntegrationTests.ProjectTestServer.Services;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.DistributionModels;
using ReleaseBoard.Web.Controllers;
using Xunit;

namespace ReleaseBoard.IntegrationTests.Controllers
{
    /// <summary>
    /// Интеграционные тесты для <see cref="DistributionsController"/>.
    /// </summary>
    public class DistributionsControllerTests : IClassFixture<ApplicationTestServerFactory>
    {
        private const string DistributionsControllerPath = "/api/Distributions";
        private readonly IFixture fixture;
        private readonly HttpClient client;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="factory"><see cref="ApplicationTestServerFactory"/>.</param>
        public DistributionsControllerTests(ApplicationTestServerFactory factory)
        {
            fixture = AutoFixtureFactory.Create();
            client = factory.Server.CreateAuthClient();
            SetupBackgroundJobClientMock();
        }

        /// <summary>
        /// Список некорректных дистрибутивов при создании.
        /// </summary>
        public static IEnumerable<object[]> IncorrectDistributionsToCreate { get; } = new[]
        {
            new object[] { null },
            // Тестовый случай: не указан SID владельца дистрибутива.
            new object[] { new DistributionModel { Name = DistributionsControllerPath } }, 
            // Тестовый случай: не найден пользователь с указанным SID.
            new object[] { new DistributionModel { Name = DistributionsControllerPath, OwnersSids = new List<string> { FakeUserSids.UknownUser1 } } }, 
            // Тестовый случай: пользователь с указанным SID не активен.
            new object[] { new DistributionModel { Name = DistributionsControllerPath, OwnersSids = new List<string> { FakeUserSids.Shagbalov } } }, 
            // Тестовый случай: дистрибутив с указанным именем уже существует.
            new object[] { new DistributionModel { Name = nameof(FakeDistributionsIds.VipnetCsp), OwnersSids = new List<string> { FakeUserSids.Yakovlev } } },
             //Тестовый случай: несколько привязок с одинаковыми полями: Path, PatternId, SourceType.
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = nameof(client), PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                        new BuildBindingModel { Path = nameof(client).ToLower(), PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = "C:\\", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = $"{nameof(client)}////", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = $"{nameof(client)}\\\\", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = ".", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = "..\\vipnet", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
        };

        /// <summary>
        /// Список некорректных дистрибутивов при обновлении.
        /// </summary>
        public static IEnumerable<object[]> IncorrectDistributionsToUpdate { get; } = new[]
        {
            new object[] { null },
            // Тестовый случай: не указан идентификатор дистрибутива.
            new object[] { new DistributionModel { Name = DistributionsControllerPath } }, 
            // Тестовый случай: не указан SID владельца дистрибутива.
            new object[] { new DistributionModel { Name = DistributionsControllerPath, Id = FakeDistributionsIds.VipnetCsp } }, 
            // Тестовый случай: не найден пользователь с указанным SID.
            new object[] { new DistributionModel { Name = DistributionsControllerPath, OwnersSids = new List<string> { FakeUserSids.UknownUser1 }, Id = FakeDistributionsIds.VipnetCsp } }, 
            // Тестовый случай: пользователь с указанным SID не активен.
            new object[] { new DistributionModel { Name = DistributionsControllerPath, OwnersSids = new List<string> { FakeUserSids.Shagbalov }, Id = FakeDistributionsIds.VipnetCsp } },
            // Тестовый случай: имя дистрибутива изменено, но оно соответствует имени другого дистрибутива.
            new object[] { new DistributionModel { Name = nameof(FakeDistributionsIds.VipnetPkiClient), OwnersSids = new List<string> { FakeUserSids.Yakovlev }, Id = FakeDistributionsIds.VipnetCsp } },
            // Тестовый случай: имя дистрибутива изменено, но оно соответствует имени другого дистрибутива, только с отличием регистра.
            new object[] { new DistributionModel { Name = nameof(FakeDistributionsIds.VipnetPkiClient).ToUpper(), OwnersSids = new List<string> { FakeUserSids.Yakovlev }, Id = FakeDistributionsIds.VipnetCsp } },
            //Тестовый случай: несколько привязок с одинаковыми полями: Path, PatternId, SourceType.
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    Id = FakeDistributionsIds.VipnetCsp,
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = nameof(client), PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                        new BuildBindingModel { Path = nameof(client).ToLower(), PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    Id = FakeDistributionsIds.VipnetCsp,
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = "C:\\", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    Id = FakeDistributionsIds.VipnetCsp,
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = $"{nameof(client)}////", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    Id = FakeDistributionsIds.VipnetCsp,
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = $"{nameof(client)}\\\\", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    Id = FakeDistributionsIds.VipnetCsp,
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = ".", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
            new object[]
            {
                new DistributionModel
                {
                    Name = DistributionsControllerPath,
                    OwnersSids = new List<string> { FakeUserSids.Yakovlev },
                    Id = FakeDistributionsIds.VipnetCsp,
                    BuildBindings = new BuildBindingModel[]
                    {
                        new BuildBindingModel { Path = "..\\vipnet", PatternId = Guid.Empty, SourceType = BuildSourceType.Pdc },
                    }
                }
            },
        };

        /// <summary>
        /// Проверяет создание дистрибутива без привязки к сборкам.
        /// Ожидается само создание дистрибутива, сканирование сборок не запускается.
        /// </summary>
        /// <param name="bindingsCount">Количество привязок сборке.</param>
        /// <returns><see cref="Task"/>.</returns>
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task Create_DistributionWithBuildBinding_DistributionCreatedTasksIdsNotEmpty(int bindingsCount)
        {
            var distribution = new DistributionModel
            {
                Name = fixture.Create<string>(),
                OwnersSids = new List<string> { FakeUserSids.Kurindin },
                BuildBindings = 
                    fixture.CreateMany<BuildBindingModel>(bindingsCount)
                           .Select(x => new BuildBindingModel { Path = x.Path, PatternId = x.PatternId, SourceType = BuildSourceType.Pdc })
                           .ToArray()
            };

            await client.PostAsJsonAsync<object>(DistributionsControllerPath, distribution);

            ValidateDistributionsEquality(distribution);
        }

        /// <summary>
        /// Проверяет валидаторы дистрибутива (при создании) - ожидается 400 код ошибки.
        /// </summary>
        /// <param name="distribution">Некорректный дистрибутив.</param>
        /// <returns><see cref="Task"/>.</returns>
        [Theory]
        [MemberData(nameof(IncorrectDistributionsToCreate))]
        public async Task Create_IncorrectDistributionModel_BadRequest(DistributionModel distribution)
        {
            var result = 
                await client.PostAsync(DistributionsControllerPath, ConvertToStringContent(distribution));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var createdDistribution = GetCreatedDistribution(distribution);
            Assert.Null(createdDistribution);
        }

        /// <summary>
        /// Проверяет валидаторы дистрибутива (при обновлении) - ожидается 400 код ошибки.
        /// </summary>
        /// <param name="distribution">Некорректный дистрибутив.</param>
        /// <returns><see cref="Task"/>.</returns>
        [Theory]
        [MemberData(nameof(IncorrectDistributionsToUpdate))]
        public async Task Update_IncorrectDistributionModel_BadRequest(DistributionModel distribution)
        {
            var result = 
                await client.PutAsync(DistributionsControllerPath, ConvertToStringContent(distribution));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        private DistributionReadModel GetCreatedDistribution(DistributionModel distribution)
        {
            return FakeDataBase
                .Distributions
                .FirstOrDefault(x => 
                    x.Name == distribution?.Name && 
                    x.Owners.Select(x => x.Sid).OrderBy(x => x).Equals(distribution.OwnersSids.OrderBy(x => x)));
        }

        private void SetupBackgroundJobClientMock()
        {
            MockedServices.BackgroundJobClientMock
                .Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Returns(() => Guid.NewGuid().ToString());
        }

        private void ValidateDistributionsEquality(DistributionModel distributionModel)
        {
            var createdDistribution = GetCreatedDistribution(distributionModel);
            Assert.NotNull(createdDistribution);
            Assert.Equal(distributionModel.OwnersSids, createdDistribution.Owners.Select(x => x.Sid));
            Assert.Equal(distributionModel.BuildBindings.Length, createdDistribution.BuildBindings.Count);
            Assert.Equal(distributionModel.ProjectBindings.Length, createdDistribution.ProjectBindings.Count);
            Assert.NotEqual(distributionModel.Id, createdDistribution.Id);
        }

        private StringContent ConvertToStringContent(DistributionModel distribution)
        {
            return new StringContent(JsonSerializer.Serialize(distribution), Encoding.UTF8);
        }
    }
}
