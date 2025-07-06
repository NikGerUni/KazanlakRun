
namespace KazanlakRun.Data.Models
{
    public class Good
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Measure { get; set; } = null!;

        public double Quantity { get; set; }

        public double QuantityOneRunner { get; set; }
    }
}

