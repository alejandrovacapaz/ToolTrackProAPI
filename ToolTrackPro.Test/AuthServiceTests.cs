using Moq;
using Xunit;
using ToolTrackPro.Models.Models;
using ToolTrackPro.Data.Repositories;
using ToolTrackPro.Services.Imp;

namespace ToolTrackPro.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly AuthService authService;

        public AuthServiceTests()
        {
            mockUserRepository = new Mock<IUserRepository>();
            authService = new AuthService(mockUserRepository.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnTrue_WhenRepositorySucceeds()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashexamplepassword123"
            };

            mockUserRepository.Setup(repo => repo.RegisterAsync(user)).ReturnsAsync(true);

            // Act
            var result = await authService.RegisterAsync(user);

            // Assert
            Assert.True(result);
            mockUserRepository.Verify(repo => repo.RegisterAsync(user), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = "hashexamplepassword123"
            };

            var expectedUser = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com"
            };

            mockUserRepository.Setup(repo => repo.LoginAsync(user)).ReturnsAsync(expectedUser);

            // Act
            var result = await authService.LoginAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            var user = new User { Email = "unknown@example.com", PasswordHash = "hashexamplepassword123" };
            mockUserRepository.Setup(repo => repo.LoginAsync(user)).ReturnsAsync((User?)null);

            // Act
            var result = await authService.LoginAsync(user);

            // Assert
            Assert.Null(result);
        }
    }
}

