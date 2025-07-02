
namespace KazanlakRun.Data.Models
{
    public class AidStation
    {
        public int Id { get; set; }

        /// <summary>Кратко име (A1, A2…)</summary>
        public string ShortName { get; set; } = null!;

        /// <summary>Пълно име на пункта</summary>
        public string Name { get; set; } = null!;

        // One-to-many: този пункт обслужва множество volunteers
        public ICollection<Volunteer> Volunteers { get; set; }
            = new List<Volunteer>();

        // Many-to-many with Distance
        public ICollection<AidStationDistance> AidStationDistances { get; set; }
            = new List<AidStationDistance>();

        // One-to-many: този пункт има много Goods
        public ICollection<Good> Goods { get; set; }
            = new List<Good>();
    }
}

