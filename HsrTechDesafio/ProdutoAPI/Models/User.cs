namespace ProdutoAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Enum para os papéis
    }

    // Enum para definir os papéis permitidos
    public enum UserRole
    {
        Admin,
        User
    }
}
