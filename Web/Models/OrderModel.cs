using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public enum OrderStatus
    {
        Success = 0,
        Pending = 1,
        Ok = 2
    }
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        
        public string CustomerName { get; set; }
        
        public string OrderCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderStatus Status { get; set; }
    }
}
