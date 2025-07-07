
namespace KazanlakRun.Data.Models
{
    public class Good
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Measure { get; set; }

        public double? Quantity { get; set; } = 0;

        public int? AidStationId { get; set; }

        public double  QuantityOneRunner { get; set; } = 0;
    }
}

