using Microsoft.Data.SqlClient;
using HomeToHome.Models;
using BCrypt.Net; // Add this for BCrypt

namespace HomeToHome.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Users WHERE LOWER(Email) = LOWER(@Email)";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@Email", email?.Trim() ?? string.Empty);

                using SqlDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    string storedHash = reader["Password"].ToString();
                    // Verify the provided password against the stored hash
                    if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        var user = new User
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            City = reader["City"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            Gender = reader["Gender"].ToString(),
                            FullAddress = reader["FullAddress"].ToString(),
                            CNIC = reader["CNIC"].ToString()
                        };
                        return user;
                    }
                }
            }
            return null;
        }

        public async Task RegisterUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }


            // Hash the password before storing
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password?.Trim());

            using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            string query = @"
                INSERT INTO Users 
                (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender, City, FullAddress, CNIC, Password)
                VALUES 
                (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Gender, @City, @FullAddress, @CNIC, @Password)";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@FirstName", user.FirstName?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@LastName", user.LastName?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@Email", user.Email?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Gender", user.Gender?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@City", user.City?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@FullAddress", user.FullAddress?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@CNIC", user.CNIC?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@Password", passwordHash); // Store hashed password

            await command.ExecuteNonQueryAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            string query = @"
            SELECT FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender, City, FullAddress, CNIC
            FROM Users
            WHERE LOWER(Email) = LOWER(@Email)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Email", email?.Trim() ?? string.Empty);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                    Gender = reader["Gender"].ToString(),
                    City = reader["City"].ToString(),
                    FullAddress = reader["FullAddress"].ToString(),
                    CNIC = reader["CNIC"].ToString()
                };
                return user;
            }

            return null;
        }
    }
}