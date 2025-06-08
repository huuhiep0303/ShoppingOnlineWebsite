using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Inventory
    {
        [Key]
        public int inventoryId { get; set; }

        [Required]
        public int productId { get; set; }
        public ProductModel Product { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Chưa nhập.")]
        public int Quantity { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Quá dài.")]
        public string Unit {  get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bắt buộc nhập.")]
        public decimal ReorderLevel { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }
        public DateTime lastUpdate { get; set; }
        public Inventory()
        {
            lastUpdate = DateTime.Now;
        }
        public Inventory(int ProductId, int initialQuantity, int ReorderQuantity, DateTime date)
        {
            productId = ProductId;
            Quantity = initialQuantity;
            ReorderLevel = ReorderQuantity;
            ExpiryDate = date;
            lastUpdate = DateTime.Now;
        }

        //Khi hàng dưới 50 thì cần nhập hàng lại
        public bool NeedRestock() => Quantity <= ReorderLevel - 50;
    }
}
