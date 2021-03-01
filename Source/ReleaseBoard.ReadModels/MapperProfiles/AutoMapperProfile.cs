using System;
using AutoMapper;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.ReadModels.MapperProfiles
{
    /// <inheritdoc />
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Контструктор.
        /// </summary>
        public AutoMapperProfile()
        {
            // ReadModel maps.
            CreateMap<BuildMatchPattern, BuildMatchPatternReadModel>().ReverseMap();

            CreateMap<BuildCreated, BuildReadModel>()
                .ForMember(x => x.ReleaseNumber, x => x.MapFrom(s => s.ReleaseNumber.ToString()))
                .ForMember(x => x.Number, x => x.MapFrom(s => s.Number.ToString()));
           
            CreateMap<DistributionCreated, DistributionReadModel>();
            CreateMap<BuildsBinding, BuildBindingReadModel>();
            CreateMap<ProjectBinding, ProjectBindingReadModel>();
        }
    }
}
