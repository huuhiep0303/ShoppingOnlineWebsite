using System.ComponentModel.DataAnnotations;

namespace Web.Models.ViewModels
{
    public class LoginViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Enter username")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = " Enter password")]
        public string Password { get; set; }
        public string ReturnURL { get; set; }
    }
}
