namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class VolunteerListItem
    {
        public int Id { get; set; }
        public string Names { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public List<string> RoleNames { get; set; } = new();
    }
}