using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;
using ToolTrackPro.Data.Repositories;

namespace ToolTrackPro.Services.Imp
{
    public class ToolService : IToolService
    {        
        private readonly IToolRepository toolRepository;
        private readonly IBorrowRepository borrowRepository;
        private readonly IReturnRepository returnRepository;

        public ToolService(IToolRepository toolRepository, IBorrowRepository borrowRepository, IReturnRepository returnRepository)
        {   
            this.toolRepository = toolRepository;
            this.borrowRepository = borrowRepository;
            this.returnRepository = returnRepository;
        }

        public async Task<List<ToolAssignmentDto>> GetAllAsync()
        {
            return await this.toolRepository.GetAllAsync();
        }

        public async Task<bool> BorrowAsync(Borrow borrow)
        {
            if(!await this.toolRepository.ExistsAsync(borrow.ToolId) || !await this.toolRepository.IsAvailableAsync(borrow.ToolId))
                return false;
            return await this.borrowRepository.InsertAsync(borrow);
        }

        public async Task<bool> ReturnAsync(Return returned)
        {
            return await this.returnRepository.InsertAsync(returned);
        }

        async Task<bool> IToolService.AddAsync(Tool tool)
        {
            return await this.toolRepository.InsertAsync(tool);
        }
    }
}
