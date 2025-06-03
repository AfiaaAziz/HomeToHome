using HomeToHome.Models;
using Microsoft.Data.SqlClient;

namespace HomeToHome.Services
{
    public class ContactService
    {
        private readonly string _connectionString;

        public ContactService(string connectionString) => _connectionString = connectionString;

        public async Task SaveContactAsync(Contact contact)
        {
            try
            {
                var query = @"INSERT INTO Contacts (Name, Email, Message) 
                     VALUES (@Name, @Email, @Message)";

                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@Name", contact.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", contact.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Message", contact.Message ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contact: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Contact>> GetAllContactsAsync()
        {
            var contacts = new List<Contact>();
            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync();
                var query = @"SELECT Id, Name, Email, Message, SubmittedAt FROM Contacts ORDER BY SubmittedAt DESC";

                using SqlCommand cmd = new(query, conn);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    contacts.Add(new Contact
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Message = reader.GetString(3),
                        SubmittedAt = reader.GetDateTime(4)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving contacts: {ex.Message}");
                throw;
            }
            return contacts;
        }
    }
}