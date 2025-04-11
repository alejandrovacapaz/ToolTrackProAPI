using ToolTrackPro.Models.Dtos;

namespace ToolTrackPro.Services
{
    public interface INotificationService
    {
        Task SendOverdueEmailsAsync();
    }
}
