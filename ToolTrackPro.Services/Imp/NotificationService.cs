using ToolTrackPro.Data.Repositories;
using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Services.Imp
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailQueueRepository emailQueueRepository;
        private readonly IUserRepository userRepository;
        private readonly IToolRepository toolRepository;

        public NotificationService(IEmailQueueRepository emailQueueRepository, IUserRepository userRepository, IToolRepository toolRepository)
        {
            this.emailQueueRepository = emailQueueRepository;
            this.userRepository = userRepository;
            this.toolRepository = toolRepository;
        }        

        public async Task SendOverdueEmailsAsync()
        {
            var overdueTools = await GetOverdueToolsAsync();
            try
            {
                foreach (var tool in overdueTools)
                {
                    string email = await GetUserEmailAsync(tool.BorrowedByUserId);
                    if (string.IsNullOrEmpty(email)) continue;

                    await QueueEmailAsync(email, $"Overdue Tool: {tool.ToolName}", $"The tool '{tool.ToolName}' is overdue by {tool.DaysOverdue} day(s). Please return it ASAP.");
                } 
            }
            catch (Exception ex)
            {
                throw new Exception("Overdue tool notifications were not send, try again", ex);
            }              
        }

        private async Task<string> GetUserEmailAsync(int userId)
        {
            return await this.userRepository.GetEmailAsync(userId);
        }

        //TODO configure sttp to send emails, meanwhile save it in the EmailQueue table
        private async Task QueueEmailAsync(string to, string subject, string body)
        {
            await this.emailQueueRepository.InsertAsync(new EmailQueue
            {
                ToEmail = to,
                MailSubject = subject,
                Body = body
            });
        }

        private async Task<List<OverdueToolDto>> GetOverdueToolsAsync()
        {
            return await this.toolRepository.GetOverdueToolsAsync();
        }
    }
}
