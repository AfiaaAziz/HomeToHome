namespace HomeToHome.Services
{
    public class AdminService
    {
        private readonly string _adminEmail = "hometohome@gmail.com";
        private readonly string _adminPasswordHash;

        public AdminService()
        {
            // Pre-hashed password for "hometohome123A" using BCrypt
            _adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("hometohome123A");
        }

        public async Task<bool> AuthenticateAdminAsync(string email, string password)
        {
            await Task.CompletedTask; // Simulate async operation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            if (email.Equals(_adminEmail, StringComparison.CurrentCultureIgnoreCase) && BCrypt.Net.BCrypt.Verify(password, _adminPasswordHash))
            {
                return true;
            }

            return false;
        }
    }
}