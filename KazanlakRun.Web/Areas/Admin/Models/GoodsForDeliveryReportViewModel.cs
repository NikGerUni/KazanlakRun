namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class GoodsForDeliveryReportViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Measure { get; set; } = string.Empty;
        public decimal NeededQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal ForDelivery => NeededQuantity - Quantity;
    }
}
