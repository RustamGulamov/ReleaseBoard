using System;
using Hangfire;
using Moq;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.Services
{
    /// <summary>
    /// Класс с замоканными сервисами.
    /// </summary>
    public static class MockedServices
    {
        /// <summary>
        /// Mock сервиса <see cref="IBackgroundJobClient"/>.
        /// </summary>
        public static readonly Mock<IBackgroundJobClient> BackgroundJobClientMock = new Mock<IBackgroundJobClient>();
    }
}
