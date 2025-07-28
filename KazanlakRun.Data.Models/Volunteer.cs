namespace KazanlakRun.Data.Models
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        public string Names { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public int? AidStationId { get; set; }
        public AidStation AidStation { get; set; } = null!;

        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();

    }
}


