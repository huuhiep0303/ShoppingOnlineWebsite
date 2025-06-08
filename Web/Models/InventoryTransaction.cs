using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    namespace Entity_class
    {
        public enum ActionEnum
        {
            Import = 0,
            Reduce = 1,
            Return = 2,
            Sale = 3
        } 
        public class InventoryTransaction
        {
            [Key]
            public int TransactionId { get; set; }//TransactionId INT PRIMARY KEY IDENTITY(1,1), -- Tự tăng

            [Required]
            public int ProductId { get; set; }
            public ProductModel Product { get; set; }

            [Required]
            public ActionEnum ActionType { get; set; }

            [Required]
            [Range(0, int.MaxValue, ErrorMessage = "Số lượng ít nhaas5 là 0.")]
            public int QuantityChanged { get; set; }

            [DataType(DataType.DateTime)]
            public DateTime ChangedTime { get; set; } = DateTime.Now; // ngày nhập hàng hoặc xuất j đó

            [StringLength(200)]
            public string note { get; set; }// ghi chú kèm theo

            public InventoryTransaction()
            {
                ChangedTime = DateTime.Now;
            }
            public InventoryTransaction(int productId, ActionEnum actionType, int quantityChanged, string note = "") : this()
            {
                ProductId = productId;
                ActionType = actionType;
                QuantityChanged = quantityChanged;
                ChangedTime = DateTime.Now;
                this.note = note;
            }
        }
    }
}
