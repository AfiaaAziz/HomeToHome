using System.ComponentModel.DataAnnotations;

namespace HomeToHome.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User Email is required")]
        [EmailAddress(ErrorMessage = "Invalid user email format")]
        [MaxLength(100, ErrorMessage = "User Email cannot exceed 100 characters")]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Worker Email is required")]
        [EmailAddress(ErrorMessage = "Invalid worker email format")]
        [MaxLength(100, ErrorMessage = "Worker Email cannot exceed 100 characters")]
        public string WorkerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service Type is required")]
        [MaxLength(50, ErrorMessage = "Service Type cannot exceed 50 characters")]
        public string ServiceType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preferred Date is required")]
        [CustomValidation(typeof(ServiceRequest), nameof(ValidatePreferredDate))]
        public DateTime? PreferredDate { get; set; }

        [Required(ErrorMessage = "Preferred Time is required")]
        [RegularExpression(@"^(1[0-2]|0?[1-9]):[0-5][0-9] (AM|PM)$", ErrorMessage = "Preferred Time must be in format HH:MM AM/PM (e.g., 2:00 PM)")]
        [MaxLength(8, ErrorMessage = "Preferred Time cannot exceed 8 characters")]
        public string? PreferredTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string? City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Address is required")]
        [MaxLength(250, ErrorMessage = "Full Address cannot exceed 250 characters")]
        public string? FullAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Request Status is required")]
        [RegularExpression(@"^(Pending|Accepted|Rejected)$", ErrorMessage = "Request Status must be Pending, Accepted, or Rejected")]
        public string? RequestStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public static ValidationResult? ValidatePreferredDate(DateTime? preferredDate, ValidationContext _)
        {
            if (!preferredDate.HasValue)
                return ValidationResult.Success;

            if (preferredDate.Value.Date < DateTime.Today)
                return new ValidationResult("Preferred Date cannot be in the past");

            return ValidationResult.Success;
        }
    }
}