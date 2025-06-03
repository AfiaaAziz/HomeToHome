using System.ComponentModel.DataAnnotations;

namespace HomeToHome.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Feedback is required")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Worker Email is required")]
        [EmailAddress(ErrorMessage = "Invalid worker email format")]
        public string WorkerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service Request ID is required")]
        public int ServiceRequestId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}