namespace ToolTrackPro.Models.Models
{
    public class Return
    {
        public int Id { get; set; }
        public int BorrowId { get; set; }        
        public DateTime ReturnDate { get; set; }

        public Return()
        {
            Id = 0;
            BorrowId = 0;
            ReturnDate = DateTime.Now;
        }
    }
}
