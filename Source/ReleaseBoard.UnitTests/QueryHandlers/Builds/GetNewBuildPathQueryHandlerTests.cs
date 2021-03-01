using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds
{
    /// <summary>
    /// Тесты для класса <see cref="GetNewBuildPaths"/>.
    /// </summary>
    public class GetNewBuildPathQueryHandlerTests
    {
        private static readonly string pdcBuildBindingFakePath = "VipnetPDC";
        private static readonly string artifactoryBuildBindingFakePath = "VipnetArtifactory";

        private static readonly string defaultPattern = @"^(?<major>\d+\.\d+\.\d+)\\(?<number>\d+(\.\d+){1,2}_\(\d+\.\d+\))[\w]*$";
        private static readonly string simpleNumberPattern = @"^(?<number>\d+(\.\d+){1,2}_\(\d+\.\d+\))[\w]*$";
        private static readonly string withDotsForPdc = @"^(?<major>(\d+\.){1,2}\d+)\\(?<number>\d+(\.\d+){2,3})[\w]*$";
        private static readonly string withDotsForArtifactory = @"^(?<major>(\d+\.){1,2}\d+)/(?<number>\d+(\.\d+){2,3})[\w]*$";

        private readonly GetNewBuildPaths.Handler handler;
        private readonly Mock<IReadOnlyRepository<DistributionReadModel>> distributionRepositoryMock = new Mock<IReadOnlyRepository<DistributionReadModel>>();

        private readonly DistributionReadModel distribution = new DistributionReadModel()
        {
            Id = Guid.NewGuid(),
            Owners = new List<User> { FakeGenerator.GetUser() },
            Name = "Some name",
        };

        /// <summary>
        /// Конструктор.
        /// </summary>
        public GetNewBuildPathQueryHandlerTests()
        {
            distributionRepositoryMock
                .Setup(x => x.Query(It.IsAny<Expression<Func<DistributionReadModel, bool>>>(), CancellationToken.None))
                .ReturnsAsync(distribution);

            handler = new GetNewBuildPaths.Handler(distributionRepositoryMock.Object, new Mock<ILogger<GetNewBuildPaths.Handler>>().Object);
        }

        /// <summary>
        /// Коллекции для проверки генератора для вычисления пути.
        /// </summary>
        public static IEnumerable<object[]> VersionAndExpectedRelativePaths => new[]
        {
            new object[]
            {
                "1.2.3.4444",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(defaultPattern, BuildSourceType.Pdc),
                },
                new Dictionary<BuildSourceType, string[]>()
                {
                    { BuildSourceType.Pdc,  new [] { $"{pdcBuildBindingFakePath}\\1.2.3\\1.2_(3.4444)" } }
                },
            },
            new object[]
            {
                "1.2.3.4",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(simpleNumberPattern, BuildSourceType.Pdc),
                },
                new Dictionary<BuildSourceType, string[]>()
                {
                    { BuildSourceType.Pdc,  new [] { $"{pdcBuildBindingFakePath}\\1.2_(3.4)" } }
                },
            },
            new object[]
            {
                "1.2.3.4",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(withDotsForPdc, BuildSourceType.Pdc),
                },
                new Dictionary<BuildSourceType, string[]>()
                {
                    {
                        BuildSourceType.Pdc,  new []
                        {
                            $"{pdcBuildBindingFakePath}\\1.2\\1.2.3.4",
                            $"{pdcBuildBindingFakePath}\\1.2.3\\1.2.3.4"
                        }
                    }
                },
            },
            new object[]
            {
                "1.2.3",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(defaultPattern, BuildSourceType.Pdc),
                },
                new Dictionary<BuildSourceType, string[]>(),
            },
            new object[]
            {
                "1.2.3",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(simpleNumberPattern, BuildSourceType.Pdc),
                },
                new Dictionary<BuildSourceType, string[]>(),
            },
            new object[]
            {
                "1.2.3",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(withDotsForPdc, BuildSourceType.Pdc),
                },
                new Dictionary<BuildSourceType, string[]>()
                {
                    {
                        BuildSourceType.Pdc,  new []
                        {
                            $"{pdcBuildBindingFakePath}\\1.2\\1.2.3"
                        }
                    }
                },
            },
            new object[]
            {
                "1.2.3.4",
                new List<BuildBindingReadModel>()
                {
                    GetBuildBindings(defaultPattern, BuildSourceType.Pdc),
                    GetBuildBindings(simpleNumberPattern, BuildSourceType.Pdc),
                    GetBuildBindings(withDotsForPdc, BuildSourceType.Pdc),
                    GetBuildBindings(withDotsForArtifactory, BuildSourceType.Artifactory)

                },
                new Dictionary<BuildSourceType, string[]>()
                {
                    {
                        BuildSourceType.Pdc,  new []
                        {
                            $"{pdcBuildBindingFakePath}\\1.2.3\\1.2_(3.4)",
                            $"{pdcBuildBindingFakePath}\\1.2_(3.4)",
                            $"{pdcBuildBindingFakePath}\\1.2\\1.2.3.4",
                            $"{pdcBuildBindingFakePath}\\1.2.3\\1.2.3.4",
                        }
                    },
                    {
                        BuildSourceType.Artifactory,  new []
                        {
                            $"{artifactoryBuildBindingFakePath}/1.2/1.2.3.4",
                            $"{artifactoryBuildBindingFakePath}/1.2.3/1.2.3.4"
                        }
                    }
                },
            },
        };

        /// <summary>
        /// Тест для проверки результата обработчика запроса GetNewPath.
        /// </summary>
        /// <param name="version">Версия.</param>
        /// <param name="buildBindings">Привязка к сборкам.</param>
        /// <param name="exceptedResult">Ожидаемый результат.</param>
        /// <returns><see cref="Task"/>.</returns>
        [Theory]
        [MemberData(nameof(VersionAndExpectedRelativePaths))]
        public async Task GetNewPath_HasBuildBindings_ExpectedResult(string version, List<BuildBindingReadModel> buildBindings, Dictionary<BuildSourceType, string[]> exceptedResult)
        {
            distribution.BuildBindings = buildBindings;

            Dictionary<BuildSourceType, string[]> result = await handler.Handle(CreateQuery(distribution.Id, version), CancellationToken.None);

            Assert.Equal(exceptedResult, result);
        }

        private static BuildBindingReadModel GetBuildBindings(string pattern, BuildSourceType buildSource)
        {
            return new BuildBindingReadModel()
            {
                Pattern = new BuildMatchPatternReadModel()
                {
                    Regexp = pattern
                },
                Path = buildSource == BuildSourceType.Pdc ? pdcBuildBindingFakePath : artifactoryBuildBindingFakePath,
                SourceType = buildSource
            };
        }

        private GetNewBuildPaths.Query CreateQuery(Guid id, string versionNumber)
        {
            return new GetNewBuildPaths.Query
            {
                DistributionId = id,
                VersionNumber = new VersionNumber(versionNumber)
            };
        }
    }
}
