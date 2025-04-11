using ToolTrackPro.Models.Models;

namespace ToolTrackPro.Data.Repositories
{
    public interface IReturnRepository
    {
        Task<bool> InsertAsync(Return returned);
    }
}
