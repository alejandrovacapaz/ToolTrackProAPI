using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Services
{
    public interface IToolService
    {
        Task<List<ToolAssignmentDto>> GetAllAsync();
        Task<bool> BorrowAsync(Borrow borrow);
        Task<bool> ReturnAsync(Return returned);
        Task<bool> AddAsync(Tool tool);
    }
}
