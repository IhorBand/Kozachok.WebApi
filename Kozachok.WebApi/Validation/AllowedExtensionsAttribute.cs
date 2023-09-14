using System.ComponentModel.DataAnnotations;

namespace Kozachok.WebApi.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile file) return ValidationResult.Success;

            var extension = Path.GetExtension(file.FileName);

            return !extensions.Contains(extension.ToLower()) ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return $"Your image's file type is not valid.";
        }
    }
}
