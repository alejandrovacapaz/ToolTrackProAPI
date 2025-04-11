namespace ToolTrackPro.Models.Models
{
    public class EmailQueue
    {
        public int Id { get; set; }
        public string ToEmail { get; set; }
        public string MailSubject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Sent { get; set; }
        
        public EmailQueue()
        {
            ToEmail = string.Empty;
            MailSubject = string.Empty;
            Body = string.Empty;
            CreatedAt = DateTime.Now;
            Sent = false;
        }
    }
}
