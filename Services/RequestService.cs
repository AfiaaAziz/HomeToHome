using HomeToHome.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

public class RequestService
{
    private readonly string _connectionString;

    public RequestService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
    }

    public async Task SubmitRequestAsync(ServiceRequest request)
    {
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = @"
            INSERT INTO ServiceRequests 
            (UserEmail, WorkerEmail, ServiceType, Description, PreferredDate, PreferredTime, City, FullAddress, RequestStatus)
            VALUES 
            (@UserEmail, @WorkerEmail, @ServiceType, @Description, @PreferredDate, @PreferredTime, @City, @FullAddress, @RequestStatus)";

        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@UserEmail", request.UserEmail);
        cmd.Parameters.AddWithValue("@WorkerEmail", request.WorkerEmail);
        cmd.Parameters.AddWithValue("@ServiceType", request.ServiceType);
        cmd.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PreferredDate", request.PreferredDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PreferredTime", request.PreferredTime ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@City", request.City ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@FullAddress", request.FullAddress ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@RequestStatus", request.RequestStatus ?? "Pending");

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<ServiceRequest>> GetAllRequestsAsync()
    {
        var requests = new List<ServiceRequest>();
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = "SELECT Id, UserEmail, WorkerEmail, ServiceType, Description, PreferredDate, PreferredTime, City, FullAddress, RequestStatus, CreatedAt FROM ServiceRequests";
        using SqlCommand cmd = new(query, conn);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            requests.Add(new ServiceRequest
            {
                Id = reader.GetInt32(0),
                UserEmail = reader.GetString(1),
                WorkerEmail = reader.GetString(2),
                ServiceType = reader.GetString(3), 
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                PreferredDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                PreferredTime = reader.IsDBNull(6) ? null : reader.GetString(6),
                City = reader.IsDBNull(7) ? null : reader.GetString(7),
                FullAddress = reader.IsDBNull(8) ? null : reader.GetString(8),
                RequestStatus = reader.GetString(9),
                CreatedAt = reader.GetDateTime(10)
            });
        }

        return requests;
    }

    public async Task<List<ServiceRequest>> GetRequestsByUserEmailAsync(string email)
    {
        List<ServiceRequest> requests = new();
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = @"
            SELECT Id, UserEmail, WorkerEmail, ServiceType, Description, PreferredDate, PreferredTime, City, FullAddress, RequestStatus, CreatedAt
            FROM ServiceRequests
            WHERE UserEmail = @Email";

        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Email", email);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            requests.Add(new ServiceRequest
            {
                Id = reader.GetInt32(0),
                UserEmail = reader.GetString(1),
                WorkerEmail = reader.GetString(2),
                ServiceType = reader.GetString(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                PreferredDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                PreferredTime = reader.IsDBNull(6) ? null : reader.GetString(6),
                City = reader.IsDBNull(7) ? null : reader.GetString(7),
                FullAddress = reader.IsDBNull(8) ? null : reader.GetString(8),
                RequestStatus = reader.GetString(9),
                CreatedAt = reader.GetDateTime(10)
            });
        }

        return requests;
    }

    public async Task UpdateRequestAsync(ServiceRequest request)
    {
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = @"
            UPDATE ServiceRequests 
            SET UserEmail = @UserEmail, 
                WorkerEmail = @WorkerEmail, 
                ServiceType = @ServiceType, 
                Description = @Description, 
                PreferredDate = @PreferredDate, 
                PreferredTime = @PreferredTime, 
                City = @City, 
                FullAddress = @FullAddress, 
                RequestStatus = @RequestStatus
            WHERE Id = @Id";

        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Id", request.Id);
        cmd.Parameters.AddWithValue("@UserEmail", request.UserEmail);
        cmd.Parameters.AddWithValue("@WorkerEmail", request.WorkerEmail);
        cmd.Parameters.AddWithValue("@ServiceType", request.ServiceType);
        cmd.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PreferredDate", request.PreferredDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PreferredTime", request.PreferredTime ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@City", request.City ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@FullAddress", request.FullAddress ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@RequestStatus", request.RequestStatus);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<ServiceRequest>> GetRequestsByWorkerEmailAsync(string email)
    {
        List<ServiceRequest> requests = new();
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = @"
            SELECT Id, UserEmail, WorkerEmail, ServiceType, Description, PreferredDate, PreferredTime, City, FullAddress, RequestStatus, CreatedAt
            FROM ServiceRequests
            WHERE WorkerEmail = @Email";

        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Email", email);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            requests.Add(new ServiceRequest
            {
                Id = reader.GetInt32(0),
                UserEmail = reader.GetString(1),
                WorkerEmail = reader.GetString(2),
                ServiceType = reader.GetString(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                PreferredDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                PreferredTime = reader.IsDBNull(6) ? null : reader.GetString(6),
                City = reader.IsDBNull(7) ? null : reader.GetString(7),
                FullAddress = reader.IsDBNull(8) ? null : reader.GetString(8),
                RequestStatus = reader.GetString(9),
                CreatedAt = reader.GetDateTime(10)
            });
        }

        return requests;
    }
}