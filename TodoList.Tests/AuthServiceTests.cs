using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Application.DTOs;
using TodoList.Infrastructure.Services;
using Xunit;

namespace TodoList.Tests
{
    public class AuthServiceTests
    {
        private Mock<UserManager<IdentityUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<IConfiguration> GetMockConfiguration()
        {
            var jwtSettings = new Dictionary<string, string?> {
               {"Jwt:Key", "TestSuperSecretKey12345678901234567890"},
               {"Jwt:Issuer", "http://test.issuer.com"},
               {"Jwt:Audience", "http://test.audience.com"}
           };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(jwtSettings)
                .Build();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c.GetSection("Jwt")["Key"]).Returns(configuration["Jwt:Key"]);
            mockConfiguration.Setup(c => c.GetSection("Jwt")["Issuer"]).Returns(configuration["Jwt:Issuer"]);
            mockConfiguration.Setup(c => c.GetSection("Jwt")["Audience"]).Returns(configuration["Jwt:Audience"]);
            mockConfiguration.Setup(c => c["Jwt:Key"]).Returns(configuration["Jwt:Key"]);
            mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns(configuration["Jwt:Issuer"]);
            mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns(configuration["Jwt:Audience"]);

            return mockConfiguration;
        }


        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenUserExists()
        {
            var mockUserManager = GetMockUserManager();
            var mockConfiguration = GetMockConfiguration();
            var registerDto = new UserRegisterDto { UserName = "existinguser", Password = "Password123", ConfirmPassword = "Password123" };
            var existingUser = new IdentityUser { UserName = "existinguser" };

            mockUserManager.Setup(um => um.FindByNameAsync(registerDto.UserName))
                           .ReturnsAsync(existingUser);

            var service = new AuthService(mockUserManager.Object, mockConfiguration.Object);

            var result = await service.RegisterAsync(registerDto);

            Assert.False(result.Succeeded);
            Assert.Contains("Username already exists.", result.Errors);
            mockUserManager.Verify(um => um.FindByNameAsync(registerDto.UserName), Times.Once);
            mockUserManager.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldFail_WhenUserNotFound()
        {
            var mockUserManager = GetMockUserManager();
            var mockConfiguration = GetMockConfiguration();
            var loginDto = new UserLoginDto { UserName = "nonexistent", Password = "Password123" };

            mockUserManager.Setup(um => um.FindByNameAsync(loginDto.UserName))
                           .ReturnsAsync((IdentityUser?)null);

            var service = new AuthService(mockUserManager.Object, mockConfiguration.Object);

            var result = await service.LoginAsync(loginDto);

            Assert.False(result.Succeeded);
            Assert.Contains("Invalid username or password.", result.Errors);
            mockUserManager.Verify(um => um.FindByNameAsync(loginDto.UserName), Times.Once);
            mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}