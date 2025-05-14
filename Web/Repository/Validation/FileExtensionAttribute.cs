using System.ComponentModel.DataAnnotations;

namespace Web.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file )
            {
                var ext = Path.GetExtension(file.FileName);
                string[] exts = { ".jpg", ".png", ".jpeg" };

                bool result = exts.Any(x => ext.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Please upload a valid image file (.jpg, .png, .jpeg)");
                }
            }
            return ValidationResult.Success;
        }
    }
}
