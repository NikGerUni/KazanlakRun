
namespace KazanlakRun.Data.Models
{
    public class Good
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        /// <summary>Мярка (kg, l, pieces и т.н.)</summary>
        public string Measure { get; set; } = null!;

        /// <summary>Количество</summary>
        public int Quantity { get; set; }

        // Всеки Good е за точно един AidStation
        public int AidStationId { get; set; }
        public AidStation AidStation { get; set; } = null!;
    }
}

