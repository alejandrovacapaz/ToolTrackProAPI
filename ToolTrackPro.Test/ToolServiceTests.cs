using Moq;
using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;
using ToolTrackPro.Services.Imp;
using ToolTrackPro.Data.Repositories;
using Xunit;

namespace ToolTrackPro.Tests
{
    public class ToolServiceTests
    {
        private readonly Mock<IToolRepository> mockToolRepository;
        private readonly Mock<IBorrowRepository> mockBorrowRepository;
        private readonly Mock<IReturnRepository> mockReturnRepository;
        private readonly ToolService toolService;

        public ToolServiceTests()
        {
            mockToolRepository = new Mock<IToolRepository>();
            mockBorrowRepository = new Mock<IBorrowRepository>();
            mockReturnRepository = new Mock<IReturnRepository>();

            toolService = new ToolService(
                mockToolRepository.Object,
                mockBorrowRepository.Object,
                mockReturnRepository.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnToolList()
        {
            // Arrange
            var tools = new List<ToolAssignmentDto>
            {
                new ToolAssignmentDto { ToolId = 1, ToolName = "Hammer", IsAvailable = true },
                new ToolAssignmentDto { ToolId = 2, ToolName = "Screwdriver", IsAvailable = false }
            };
            mockToolRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tools);

            // Act
            var result = await toolService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Hammer", result[0].ToolName);
        }

        [Fact]
        public async Task BorrowAsync_ShouldReturnFalse_WhenToolDoesNotExist()
        {
            // Arrange
            var borrow = new Borrow { ToolId = 1, UserId = 1 };
            mockToolRepository.Setup(repo => repo.ExistsAsync(borrow.ToolId)).ReturnsAsync(false);

            // Act
            var result = await toolService.BorrowAsync(borrow);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BorrowAsync_ShouldReturnTrue_WhenToolIsAvailable()
        {
            // Arrange
            var borrow = new Borrow { ToolId = 1, UserId = 1 };
            mockToolRepository.Setup(repo => repo.ExistsAsync(borrow.ToolId)).ReturnsAsync(true);
            mockToolRepository.Setup(repo => repo.IsAvailableAsync(borrow.ToolId)).ReturnsAsync(true);
            mockBorrowRepository.Setup(repo => repo.InsertAsync(borrow)).ReturnsAsync(true);

            // Act
            var result = await toolService.BorrowAsync(borrow);

            // Assert
            Assert.True(result);
        }
    }
}