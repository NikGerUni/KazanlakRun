namespace KazanlakRun.Data.Models
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        /// <summary>Две имена</summary>
        public string Names { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        // Foreign key to AidStation
        public int? AidStationId { get; set; }
        public AidStation AidStation { get; set; } = null!;

        // Many-to-many to Role via join entity
        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();
    }
}

