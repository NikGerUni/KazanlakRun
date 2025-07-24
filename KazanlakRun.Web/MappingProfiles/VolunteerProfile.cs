using AutoMapper;
using KazanlakRun.Data.Models;
using KazanlakRun.Areas.User.Models;

namespace KazanlakRun.Web.MappingProfiles
{
    public class VolunteerProfile : Profile
    {
        public VolunteerProfile()
        {
            // от InputModel към ентити (за Create/Update)
            CreateMap<VolunteerInputModel, Volunteer>()
                // ако имената на свойствата не съвпадат, примерно:
                // .ForMember(dest => dest.AidStationId, opt => opt.MapFrom(src => src.StationId))
                ;

            // от ентити към InputModel (за Edit GET)
            CreateMap<Volunteer, VolunteerInputModel>();
        }
    }
}

