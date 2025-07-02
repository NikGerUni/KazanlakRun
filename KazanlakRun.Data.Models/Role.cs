// Role.cs
namespace KazanlakRun.Data.Models
{
    public class Role
    {
        public int Id { get; set; }

        /// <summary>Име на ролята (doctor, security и т.н.)</summary>
        public string Name { get; set; } = null!;

        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();
    }
}

