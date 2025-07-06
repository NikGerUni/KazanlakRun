namespace KazanlakRun.Data.Models
{
    public class VolunteerRole
    {
        public int VolunteerId { get; set; }
        public Volunteer Volunteer { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}

