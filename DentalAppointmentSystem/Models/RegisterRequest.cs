namespace YourProjectNamespace.Models
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; } = "Customer";  // Default value
        public string Mobile { get; set; }
        public string Image { get; set; }  // If image is stored as a string (URL or base64)
        public bool IsActive { get; set; } = true;  // Default 
    }
}