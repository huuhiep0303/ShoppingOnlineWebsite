using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public string CustomerEmail { get; set; }
        [Range(1,5)]
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}