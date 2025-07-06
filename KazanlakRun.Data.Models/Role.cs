
namespace KazanlakRun.Data.Models
{
    public class Role
    {
        public int Id { get; set; }

            public string Name { get; set; } = null!;

        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();
    }
}

