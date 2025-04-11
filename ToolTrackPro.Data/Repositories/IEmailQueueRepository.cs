using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories
{
    public interface IEmailQueueRepository
    {
        Task<bool> InsertAsync(EmailQueue emailQueue);        
    }
}
