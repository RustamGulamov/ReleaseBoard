using System;
using AutoMapper;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using ReleaseBoard.Common.Contracts.Common.Models;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Messages
{
    /// <summary>
    /// Профиль маппингов для событий из BuildSync.
    /// </summary>
    public class BuildEventMessagesMappingProfile : Profile
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public BuildEventMessagesMappingProfile()
        {
            CreateMap<BuildDto, CreateBuild>()
                .ForMember(x => x.ReleaseNumber, x => x.MapFrom(s => string.IsNullOrWhiteSpace(s.ReleaseNumber) ? null : new VersionNumber(s.ReleaseNumber)))
                .ForMember(x => x.Number, x => x.MapFrom(s => new VersionNumber(s.Number)))
                .ForMember(x => x.BuildDate, x => x.MapFrom(s => s.CreateDate));

            CreateMap<BuildDto, UpdateBuild>()
                .ForMember(x => x.ReleaseNumber, x => x.MapFrom(s => string.IsNullOrWhiteSpace(s.ReleaseNumber) ? null : new VersionNumber(s.ReleaseNumber)))
                .ForMember(x => x.Number, x => x.MapFrom(s => new VersionNumber(s.Number)));

            CreateMap<NewBuildEvent, CreateBuild>().ConstructUsing((e, c) => c.Mapper.Map<CreateBuild>(e.Build));

            CreateMap<UpdateBuildEvent, UpdateBuild>()
                .ForMember(x => x.ChangeDate, x => x.MapFrom(s => s.Date))
                .ForMember(x => x.ReleaseNumber, x => x.MapFrom(s => string.IsNullOrWhiteSpace(s.ReleaseNumber) ? null : new VersionNumber(s.ReleaseNumber)))
                .ForMember(x => x.Number, x => x.MapFrom(s => new VersionNumber(s.Number)));

            CreateMap<DeleteBuildEvent, DeleteBuild>();

            // Если существует location, то удалим сборку.
            CreateMap<UpdateBuildEvent, DeleteBuild>()
                .ForMember(x => x.DeleteDate, x => x.MapFrom(s => s.Date));
        }
    }
}
