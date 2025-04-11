namespace ToolTrackPro.Models.Dtos
{
    public class ToolAssignmentDto
    {
        public int ToolId { get; set; }
        public string ToolName { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int BorrowId { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Boolean IsAvailable { get; set; }

        public ToolAssignmentDto()
        {
            ToolId = 0;
            UserId = 0;
            ToolName = string.Empty;            
            UserName = string.Empty;
            UserEmail = string.Empty;           
            IsAvailable = false;
        }
    }
}
