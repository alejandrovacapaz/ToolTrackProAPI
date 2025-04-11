namespace ToolTrackPro.Models.Dtos
{
    public class OverdueToolDto
    {
        public int ToolId { get; set; }
        public string ToolName { get; set; }
        public int BorrowedByUserId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public int DaysOverdue { get; set; }
        
        public OverdueToolDto()
        {
            ToolId = 0;
            ToolName = string.Empty;
            BorrowedByUserId = 0;
            BorrowedAt = DateTime.Now;
            DaysOverdue = 0;
        }
    }
}
