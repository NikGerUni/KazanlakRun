using System.Collections.Generic;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class VolunteerReport
    {
        public string Names { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }

    public class AidStationVolunteersReportViewModel
    {
        public string AidStationName { get; set; } = null!;
        public List<VolunteerReport> Volunteers { get; set; } = new();
    }
}

