using AutoMapper;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.User.Models;

namespace KazanlakRun.Web.MappingProfiles
{
    public class VolunteerProfile : Profile
    {
        public VolunteerProfile()
        {
            CreateMap<VolunteerInputModel, Volunteer>()
                ;

            CreateMap<Volunteer, VolunteerInputModel>();
        }
    }
}

