namespace ToolTrackPro.Models.Models
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        
        public Tool()
        {
            Id = 0;
            Name = string.Empty;           
        }
    }
}
