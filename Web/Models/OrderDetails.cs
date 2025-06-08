using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string OrderCode { get; set; }
        [ForeignKey("ProductID")]
        public OrderModel Order { get; set; }
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
    }
}
