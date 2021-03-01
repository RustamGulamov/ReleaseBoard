using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using ReleaseBoard.Application.Models;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.IntegrationTests.ProjectTestServer;
using ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants;
using ReleaseBoard.Web.Controllers;
using Xunit;

namespace ReleaseBoard.IntegrationTests.Controllers
{
    /// <summary>
    /// Интеграционные тесты для <see cref="BuildsController"/>.
    /// </summary>
    public class BuildsControllerTests : IClassFixture<ApplicationTestServerFactory>
    {
        private const string BuildsControllerPath = "/api/Builds";
        private readonly TestServer server;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="factory"><see cref="ApplicationTestServerFactory"/>.</param>
        public BuildsControllerTests(ApplicationTestServerFactory factory)
        {
            server = factory.Server;
        }

        /// <summary>
        /// Список фильтров, под которые не подпадет ни одна фейковая сборка.
        /// </summary>
        public static IEnumerable<object[]> FiltersNotCorrespondingToAnyBuilds { get; } = new[]
        {
            // Тестовый случай: заданный в фильтре диапазон дат не соответствует ни одной сборке.
            new object[] { new DistributionBuildsFilter { DistributionId = FakeDistributionsIds.VipnetCsp, CreationDateRange = new DateRange() } },
            // Тестовый случай: указанный дистрибутив не содержит ни одной сборки.
            new object[] { new DistributionBuildsFilter { DistributionId = FakeDistributionsIds.Empty, CreationDateRange = FakeBuildDatesRange.BuildsCreationDateRange } },
            // Тестовый случай: ни одна сборка не содержит указанный тег.
            new object[]
            {
                new DistributionBuildsFilter
                {
                    DistributionId = FakeDistributionsIds.VipnetClient,
                    CreationDateRange = FakeBuildDatesRange.BuildsCreationDateRange,
                    Tags = new string[] { "AAAA" }
                }
            },
            // Тестовый случай: ни одна сборка не содержит указанный суффикс.
            new object[]
            {
                new DistributionBuildsFilter
                {
                    DistributionId = FakeDistributionsIds.VipnetClient,
                    CreationDateRange = FakeBuildDatesRange.BuildsCreationDateRange,
                    Suffixes = new string[] { "AAAA" }
                }
            },
        };
    }
}
