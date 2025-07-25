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
        public class VolunteersByAidStationPageViewModel
        {
            public AidStationVolunteersReportViewModel Station { get; set; } = null!;
            public int PageNumber { get; set; }
            public int TotalPages { get; set; }
            public string FilterText { get; set; } = string.Empty;
           public int PageSize { get; set; }
    }
    }


