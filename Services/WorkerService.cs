using Microsoft.Data.SqlClient;
using HomeToHome.Models;


namespace HomeToHome.Services
{
    public class WorkerService
    {
        private readonly string _connectionString;

        public WorkerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Worker?> AuthenticateWorker(string email, string password)
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM Workers WHERE Email = @Email";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string storedHash = reader["Password"].ToString();
                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                {
                    return new Worker
                    {
                        FirstName = reader["FirstName"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                }
            }

            return null;
        }

        public async Task<bool> RegisterWorker(Worker worker)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"
    INSERT INTO Workers 
    (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender, City, FullAddress, Designation, 
     Experience, PreferredWorkingHours, CNIC, Password, Skills)
    VALUES 
    (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Gender, @City, @FullAddress, @Designation,
     @Experience, @PreferredWorkingHours, @CNIC, @Password, @Skills);";

                using SqlCommand cmd = new SqlCommand(query, conn);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(worker.Password);

                cmd.Parameters.AddWithValue("@FirstName", worker.FirstName);
                cmd.Parameters.AddWithValue("@LastName", worker.LastName);
                cmd.Parameters.AddWithValue("@Email", worker.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", worker.PhoneNumber);
                cmd.Parameters.AddWithValue("@DateOfBirth", worker.DateOfBirth.Value);
                cmd.Parameters.AddWithValue("@Gender", worker.Gender);
                cmd.Parameters.AddWithValue("@City", worker.City);
                cmd.Parameters.AddWithValue("@FullAddress", worker.FullAddress);
                cmd.Parameters.AddWithValue("@Designation", worker.Designation);
                cmd.Parameters.AddWithValue("@Experience", (object?)worker.Experience ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PreferredWorkingHours", worker.PreferredWorkingHours);
                cmd.Parameters.AddWithValue("@CNIC", worker.CNIC);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Skills", string.Join(", ", worker.Skills));

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving worker: " + ex.Message);
                return false;
            }
        }

        public async Task<List<Worker>> SearchWorkersBySkill(string skill)
        {
            List<Worker> workers = new();

            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            string query = @"
            SELECT FirstName, LastName, City, Designation, Experience, Skills, Email
            FROM Workers
            WHERE Skills LIKE @Skill";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Skill", "%" + skill + "%");

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Worker worker = new()
                {
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    City = reader["City"].ToString(),
                    Designation = reader["Designation"].ToString(),
                    Experience = reader["Experience"] as int?,
                    Skills = reader["Skills"].ToString()?.Split(",").Select(s => s.Trim()).ToList(),
                    Email = reader["Email"].ToString()
                };
                workers.Add(worker);
            }

            return workers;
        }

        public async Task<Worker?> GetWorkerByEmailAsync(string email)
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            string query = @"
            SELECT FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender, City, FullAddress, 
                   Designation, Experience, PreferredWorkingHours, CNIC, Password, Skills
            FROM Workers
            WHERE Email = @Email";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Worker
                {
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                    Gender = reader["Gender"].ToString(),
                    City = reader["City"].ToString(),
                    FullAddress = reader["FullAddress"].ToString(),
                    Designation = reader["Designation"].ToString(),
                    Experience = reader.IsDBNull(reader.GetOrdinal("Experience")) ? null : reader.GetInt32(reader.GetOrdinal("Experience")),
                    PreferredWorkingHours = reader["PreferredWorkingHours"].ToString(),
                    CNIC = reader["CNIC"].ToString(),
                    Password = reader["Password"].ToString(),
                    Skills = reader["Skills"].ToString()?.Split(",").Select(s => s.Trim()).ToList() ?? new List<string>()
                };
            }

            return null;
        }

        public async Task<bool> UpdateWorkerAsync(Worker worker)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                string query = @"
            UPDATE Workers 
            SET FirstName = @FirstName, 
                LastName = @LastName, 
                PhoneNumber = @PhoneNumber, 
                DateOfBirth = @DateOfBirth, 
                Gender = @Gender, 
                City = @City, 
                FullAddress = @FullAddress, 
                Designation = @Designation, 
                Experience = @Experience, 
                PreferredWorkingHours = @PreferredWorkingHours, 
                CNIC = @CNIC, 
                Password = @Password, 
                Skills = @Skills
            WHERE Email = @Email";

                using SqlCommand cmd = new(query, conn);
                string hashedPassword = worker.Password.Length > 50 ? worker.Password : BCrypt.Net.BCrypt.HashPassword(worker.Password);

                cmd.Parameters.AddWithValue("@FirstName", worker.FirstName);
                cmd.Parameters.AddWithValue("@LastName", worker.LastName);
                cmd.Parameters.AddWithValue("@Email", worker.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", worker.PhoneNumber);
                cmd.Parameters.AddWithValue("@DateOfBirth", worker.DateOfBirth ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Gender", worker.Gender);
                cmd.Parameters.AddWithValue("@City", worker.City);
                cmd.Parameters.AddWithValue("@FullAddress", worker.FullAddress);
                cmd.Parameters.AddWithValue("@Designation", worker.Designation);
                cmd.Parameters.AddWithValue("@Experience", (object?)worker.Experience ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PreferredWorkingHours", worker.PreferredWorkingHours);
                cmd.Parameters.AddWithValue("@CNIC", worker.CNIC);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Skills", string.Join(", ", worker.Skills));

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating worker: " + ex.Message);
                return false;
            }
        }

        public async Task<List<Worker>> GetAllWorkersAsync()
        {
            List<Worker> workers = new();
            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                string query = @"
                SELECT FirstName, LastName, Email, PhoneNumber, DateOfBirth, Gender, City, FullAddress, 
                       Designation, Experience, PreferredWorkingHours, CNIC, Skills
                FROM Workers";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    workers.Add(new Worker
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        Gender = reader["Gender"].ToString(),
                        City = reader["City"].ToString(),
                        FullAddress = reader["FullAddress"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        Experience = reader.IsDBNull(reader.GetOrdinal("Experience")) ? null : reader.GetInt32(reader.GetOrdinal("Experience")),
                        PreferredWorkingHours = reader["PreferredWorkingHours"].ToString(),
                        CNIC = reader["CNIC"].ToString(),
                        Skills = reader["Skills"].ToString()?.Split(",").Select(s => s.Trim()).ToList() ?? new List<string>()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving workers: {ex.Message}");
                throw;
            }
            return workers;
        }
    }
}