namespace ToolTrackPro.Models.Models
{
    public class Borrow
    {
        public int Id { get; set; }
        public int ToolId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime EstimatedReturnDate { get; set; }
        public Borrow()
        {
            Id = 0;
            ToolId = 0;
            UserId = 0;
            BorrowDate = DateTime.Now;
            EstimatedReturnDate = DateTime.Now.AddDays(7);
        }
    }
}
