using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories
{
    public interface IToolRepository
    {
        Task<List<ToolAssignmentDto>> GetAllAsync();
        Task<bool> ExistsAsync(int toolId);
        Task<bool> IsAvailableAsync(int toolId);
        Task<List<OverdueToolDto>> GetOverdueToolsAsync();
        Task<bool> InsertAsync(Tool tool);
    }
}
