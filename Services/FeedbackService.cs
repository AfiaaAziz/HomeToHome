using Microsoft.Data.SqlClient;
using HomeToHome.Models;

namespace HomeToHome.Services
{
    public class FeedbackService
    {
        private readonly string _connectionString;

        public FeedbackService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        public async Task SaveFeedbackAsync(Feedback feedback)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO Feedbacks (Name, Email, Message, WorkerEmail, ServiceRequestId, Rating, SubmittedAt) 
                             VALUES (@Name, @Email, @Message, @WorkerEmail, @ServiceRequestId, @Rating, @SubmittedAt)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", feedback.Name);
            command.Parameters.AddWithValue("@Email", feedback.Email);
            command.Parameters.AddWithValue("@Message", feedback.Message);
            command.Parameters.AddWithValue("@WorkerEmail", feedback.WorkerEmail);
            command.Parameters.AddWithValue("@ServiceRequestId", feedback.ServiceRequestId);
            command.Parameters.AddWithValue("@Rating", feedback.Rating);
            command.Parameters.AddWithValue("@SubmittedAt", feedback.SubmittedAt);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Feedback>> GetFeedbackByWorkerEmailAsync(string workerEmail)
        {
            var feedbackList = new List<Feedback>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"SELECT Id, Name, Email, Message, WorkerEmail, ServiceRequestId, Rating, SubmittedAt 
                             FROM Feedbacks 
                             WHERE WorkerEmail = @WorkerEmail 
                             ORDER BY SubmittedAt DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WorkerEmail", workerEmail);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    feedbackList.Add(new Feedback
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Message = reader.GetString(3),
                        WorkerEmail = reader.GetString(4),
                        ServiceRequestId = reader.GetInt32(5),
                        Rating = reader.GetInt32(6),
                        SubmittedAt = reader.GetDateTime(7)
                    });
                }
            }
            return feedbackList;
        }
    }
}