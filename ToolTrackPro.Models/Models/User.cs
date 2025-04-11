namespace ToolTrackPro.Models.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public User()
        {
            Id = 0;
            Name = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }
    }    
}
