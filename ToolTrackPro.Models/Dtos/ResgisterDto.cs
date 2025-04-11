namespace ToolTrackPro.Models.Dtos
{
    public class ResgisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public ResgisterDto()
        {
            this.Name = string.Empty;
            this.Email = string.Empty;
            this.Password = string.Empty;
        }
    }
}
