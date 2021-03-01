using System;
using System.Collections.Generic;
using System.Linq;
using Lighthouse.Contracts.Enums;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants;
using ReleaseBoard.ReadModels;
using LighthouseUser = Lighthouse.Contracts.Dtos.EmployeeDto;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase
{
    /// <summary>
    /// Фейковая база данных.
    /// </summary>
    public static class FakeDataBase
    {
        /// <summary>
        /// Список пользователей.
        /// </summary>
        public static readonly List<LighthouseUser> Users = new(){};

        /// <summary>
        /// Список фейковых дистрибутивов.
        /// </summary>
        public static readonly List<DistributionReadModel> Distributions = new()
        {
            new() { Name = nameof(FakeDistributionsIds.VipnetCsp), Id = FakeDistributionsIds.VipnetCsp, Owners = new List<User> { GetUserBySid(FakeUserSids.Kurindin) } },
            new() { Name = nameof(FakeDistributionsIds.VipnetClient), Id = FakeDistributionsIds.VipnetClient, Owners = new List<User> { GetUserBySid(FakeUserSids.Milich) } },
            new() { Name = nameof(FakeDistributionsIds.VipnetPkiClient), Id = FakeDistributionsIds.VipnetPkiClient, Owners = new List<User> { GetUserBySid(FakeUserSids.Medvedev) } },
            new() { Name = nameof(FakeDistributionsIds.Empty), Id = FakeDistributionsIds.Empty }
        };

        private static LighthouseUser GetLighthouseUserBySid(string sid) => Users.FirstOrDefault(x => x.Sid == sid);

        private static User GetUserBySid(string sid)
        {
            var user = Users.FirstOrDefault(x => x.Sid == sid);
            return user == null ? null : new User(user.Sid, user.Name);
        }
    }
}
