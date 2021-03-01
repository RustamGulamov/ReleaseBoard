using System;
using System.Linq;
using Bogus;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.Common.Models;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels;
using LighthouseUser = Lighthouse.Contracts.Dtos.EmployeeDto;
using Project = ReleaseBoard.Domain.ValueObjects.Project;

namespace ReleaseBoard.UnitTests.DataGenerators
{
    /// <summary>
    /// Генератор пользователей.
    /// </summary>
    public static class FakeGenerator
    {
        /// <summary>
        /// Возвращает фейковые объекты OldBuildMatchPattern.
        /// </summary>
        public static readonly Faker<BuildMatchPattern> BuildMatchPatterns =
            new Faker<BuildMatchPattern>()
                .CustomInstantiator(x => new BuildMatchPattern(x.Random.Word()));

        /// <summary>
        /// Фейковые контракты сборок.
        /// </summary>
        public static readonly Faker<BuildDto> BuildsDto = new Faker<BuildDto>()
            .RuleFor(x => x.Location, f => f.Random.Word())
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.DistributionId, f => Guid.NewGuid())
            .RuleFor(x => x.CreateDate, f => f.Date.Past())
            .RuleFor(x => x.Number, f => $"1.2.3.4{f.Random.Number(0, 999)}")
            .RuleFor(x => x.ReleaseNumber, f => "1.2.3")
            .RuleFor(x => x.Suffixes, f => f.Random.WordsArray(3));

        /// <summary>
        /// Фейковые контракты сборок.
        /// </summary>
        public static readonly Faker<BuildReadModel> Build = new Faker<BuildReadModel>()
            .RuleFor(x => x.Location, f => f.Random.Word())
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.DistributionId, f => Guid.NewGuid())
            .RuleFor(x => x.BuildDate, f => f.Date.Past())
            .RuleFor(x => x.Number, f => $"1.2.3.4{f.Random.Number(0, 999)}")
            .RuleFor(x => x.ReleaseNumber, f => "1.2.3")
            .RuleFor(x => x.Suffixes, f => f.Random.WordsArray(3).ToList());
        
        /// <summary>
        /// Возвращает фейковые объекты OldBuildMatchPattern.
        /// </summary>
        public static readonly Faker<BuildsBinding> BuildsBindings =
            new Faker<BuildsBinding>()
                .CustomInstantiator(x =>
                    new BuildsBinding(x.Random.Word(), BuildMatchPatterns.Generate(1)[0], x.PickRandom<BuildSourceType>()));

        /// <summary>
        /// Фейковые юзеры.
        /// </summary>
        public static readonly Faker<LighthouseUser> LighthouseUsers =
            new Faker<LighthouseUser>()
                .RuleFor(x => x.Name, x => x.PickRandom(userNames))
                .RuleFor(x => x.Sid, x => Guid.NewGuid().ToString());

        /// <summary>
        /// Фейковые юзеры.
        /// </summary>
        public static readonly Faker<User> Users =
            new Faker<User>()
                .CustomInstantiator(x => new User(Guid.NewGuid().ToString(), x.PickRandom(userNames)));

        /// <summary>
        /// Возвращает фейковые объекты Projects.
        /// </summary>
        public static readonly Faker<Project> Projects =
            new Faker<Project>()
                .CustomInstantiator(x => new Project(Guid.NewGuid(), x.Random.Words(), x.Random.Words()));

        /// <summary>
        /// Возвращает фейковые объекты OldBuildMatchPattern.
        /// </summary>
        public static readonly Faker<ProjectBinding> ProjectBindings =
            new Faker<ProjectBinding>()
                .CustomInstantiator(x => new ProjectBinding(x.Random.Hash(), Projects.Generate(1)[0]));
        
        private static readonly string[] userNames = new[] { "Иванов Иван", "Петр Петрович", "Кабанов Святослав", "Люк Скайвокер", "Квентин Тартарино" };

        /// <summary>
        /// Взять случайный объект.
        /// </summary>
        /// <returns>Пользователь.</returns>
        public static LighthouseUser GetLighthouseUser() => LighthouseUsers.Generate(1).First();

        /// <summary>
        /// Генерация рандомной строки.
        /// </summary>
        /// <returns>Рандомная строка.</returns>
        public static string GetString() => new Faker<string>().CustomInstantiator(x => x.Random.Words());

        /// <summary>
        /// Взять случайный объект.
        /// </summary>
        /// <returns>Пользователь.</returns>
        public static User GetUser() => Users.Generate(1).First();

        /// <summary>
        /// Взять случайный объект.
        /// </summary>
        /// <returns>Пользователь.</returns>
        public static BuildsBinding GetBuildsBinding() => BuildsBindings.Generate(1).First();
    }
}
