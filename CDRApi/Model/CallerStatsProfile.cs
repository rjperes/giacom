using AutoMapper;
using CDRServices;

namespace CDRApi.Model
{
    public class CallerStatsProfile : Profile
    {
        public CallerStatsProfile()
        {
            CreateMap<CallerStats, CallerStatsDto>()
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(x => x.EndTime))
                .ReverseMap();
        }
    }
}
