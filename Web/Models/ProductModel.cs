using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web.Repository.Validation;

namespace Web.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Yêu cầu chọn 1 thương hiệu")]
        public int BrandID { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Yêu cầu chọn 1 danh mục")]
        public int CategoryID { get; set; }
        // Navigation properties

        [Required(ErrorMessage ="Yêu cầu nhập lượng hàng tồn kho")]
        [Range(0,int.MaxValue,ErrorMessage ="Nhập số lượng lớn hơn 0.")]
        public int StockQuantity { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public string Image { get; set; } 
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
