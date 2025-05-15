using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Enter username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Enter email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = " Enter password")]
        public string Password { get; set; }
    }
}
