using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class FavoriteModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductID { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}