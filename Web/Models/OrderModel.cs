namespace Web.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string OrderCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }
}
