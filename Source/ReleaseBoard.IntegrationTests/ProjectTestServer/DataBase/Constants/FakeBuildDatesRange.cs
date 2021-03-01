using System;
using ReleaseBoard.Application.Models;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants
{
    /// <summary>
    /// Представляет фейковый период дат сборки билдов.
    /// </summary>
    public class FakeBuildDatesRange
    {
        /// <summary>
        /// Дата начала периода сборки.
        /// </summary>
        public static readonly DateTime StartDate = new DateTime(2018, 1, 1);

        /// <summary>
        /// Дата окончания периода сборки.
        /// </summary>
        public static readonly DateTime EndDate = StartDate.AddYears(1);

        /// <summary>
        /// Период в течении которого созданы все фейковые сборки.
        /// </summary>
        public static readonly DateRange BuildsCreationDateRange = new DateRange { StartDate = StartDate, EndDate = EndDate };
    }
}
