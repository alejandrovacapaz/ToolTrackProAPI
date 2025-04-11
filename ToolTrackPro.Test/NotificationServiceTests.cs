using Xunit;
using Moq;
using ToolTrackPro.Services.Imp;
using ToolTrackPro.Models.Dtos;
using ToolTrackPro.Models.Models;
using ToolTrackPro.Data.Repositories;

namespace ToolTrackPro.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public async Task SendOverdueEmailsAsync_ShouldQueueEmails_ForValidUsers()
        {
            // Arrange
            var mockToolRepo = new Mock<IToolRepository>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockEmailRepo = new Mock<IEmailQueueRepository>();

            var overdueTools = new List<OverdueToolDto>
            {
                new OverdueToolDto
                {
                    ToolId = 1,
                    ToolName = "Drill",
                    BorrowedByUserId = 101,
                    DaysOverdue = 3
                },
                new OverdueToolDto
                {
                    ToolId = 2,
                    ToolName = "Hammer",
                    BorrowedByUserId = 102,
                    DaysOverdue = 5
                }
            };

            mockToolRepo.Setup(r => r.GetOverdueToolsAsync()).ReturnsAsync(overdueTools);
            mockUserRepo.Setup(r => r.GetEmailAsync(101)).ReturnsAsync("user1@example.com");
            mockUserRepo.Setup(r => r.GetEmailAsync(102)).ReturnsAsync("user2@example.com");

            var service = new NotificationService(
                mockEmailRepo.Object,
                mockUserRepo.Object,
                mockToolRepo.Object
            );

            // Act
            await service.SendOverdueEmailsAsync();

            // Assert
            mockEmailRepo.Verify(r => r.InsertAsync(It.Is<EmailQueue>(e =>
                e.ToEmail == "user1@example.com" &&
                e.MailSubject.Contains("Drill") &&
                e.Body.Contains("3")
            )), Times.Once);

            mockEmailRepo.Verify(r => r.InsertAsync(It.Is<EmailQueue>(e =>
                e.ToEmail == "user2@example.com" &&
                e.MailSubject.Contains("Hammer") &&
                e.Body.Contains("5")
            )), Times.Once);

            mockEmailRepo.Verify(r => r.InsertAsync(It.IsAny<EmailQueue>()), Times.Exactly(2));
        }
    }
}

