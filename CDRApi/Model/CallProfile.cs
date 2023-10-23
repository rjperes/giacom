using AutoMapper;
using CDRModel;
using System.Globalization;

namespace CDRApi.Model
{
    public class CallProfile : Profile
    {
        public CallProfile()
        {
            CreateMap<CallDto, Call>()
                .ForMember(dest => dest.CallerId, opt => opt.MapFrom(x => x.Caller_Id))
                .ForMember(dest => dest.CallDate, opt => opt.MapFrom(x => DateTime.ParseExact(x.Call_Date!, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(x => (CallType)x.Type))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(x => TimeSpan.ParseExact(x.End_Time!, "hh\\:mm\\:ss", CultureInfo.InvariantCulture)))
                .ReverseMap();
        }
    }
}
