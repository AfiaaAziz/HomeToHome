using System;
using System.ComponentModel.DataAnnotations;

namespace HomeToHome.Models
{
    public class User
    {
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public string? FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public string? LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string? Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Phone must be 10-15 digits")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        [CustomValidation(typeof(User), nameof(ValidateAge))]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50)]
        public string? City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Address is required")]
        [MaxLength(250)]
        public string? FullAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNIC is required")]
        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "CNIC must be 13 digits")]
        public string? CNIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and symbol")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public static ValidationResult? ValidateAge(DateTime? dob, ValidationContext _)
        {
            if (dob.HasValue && DateTime.Today.Year - dob.Value.Year < 18)
            {
                return new ValidationResult("Must be at least 18 years old");
            }
            return ValidationResult.Success;
        }
    }
}
