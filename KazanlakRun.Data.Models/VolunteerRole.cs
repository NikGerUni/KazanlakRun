// VolunteerRole.cs
namespace KazanlakRun.Data.Models
{
    /// <summary>Join entity за many-to-many Volunteer ↔ Role</summary>
    public class VolunteerRole
    {
        public int VolunteerId { get; set; }
        public Volunteer Volunteer { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}

