using AutoMapper;
using CDRServices;

namespace CDRApi.Model
{
    public class CallStatsProfile : Profile
    {
        public CallStatsProfile()
        {
            CreateMap<CallStats, CallStatsDto>()
                .ReverseMap();
        }
    }
}
