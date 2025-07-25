using Devsu.Application.Dtos.Core;

namespace Devsu.Test.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _repositoryMock = new();
            _mapperMock = new();
            _loggerMock = new();
            _userService = new(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenUserAlreadyExists()
        {
            // Arrange
            var input = new CreateUser { Identification = "123456", ClientId = "client123" };
            _repositoryMock
                .Setup(r => r.ExistAsync(x => x.Identification == input.Identification || x.ClientId == input.ClientId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.CreateAsync(input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Cliente con identificación o clientId ya existe", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var input = new CreateUser { Identification = "123456", ClientId = "client123" };
            var user = new User { Id = Guid.NewGuid() };

            _repositoryMock
                .Setup(r => r.ExistAsync(x => x.Identification == input.Identification || x.ClientId == input.ClientId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<User>(input))
                .Returns(user);

            _repositoryMock
                .Setup(r => r.CreateAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.CreateAsync(input, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(user.Id, result.Data);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var input = new EditUser { Identification = "123456", ClientId = "client123" };

            _repositoryMock
                .Setup(r => r.GetOneAsync(x => x.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateAsync(userId, input, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsNotFound);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenUserIsUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var input = new EditUser
            {
                Identification = "654321",
                ClientId = "client456",
                Name = "Updated Name",
                Gender = "Male",
                Phone = "123456789",
                Age = 30,
                Address = "Updated Address",
                Password = "newpassword"
            };

            var existingUser = new User
            {
                Id = userId,
                Identification = "123456",
                ClientId = "client123",
                Name = "Old Name",
                Gender = "Female",
                Phone = "987654321",
                Age = 25,
                Address = "Old Address",
                Password = "oldpassword"
            };

            _repositoryMock
                .Setup(r => r.GetOneAsync(x => x.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            _repositoryMock
                .Setup(r => r.ExistAsync(x => x.ClientId == input.ClientId && x.Id != userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(r => r.ExistAsync(x => x.Identification == input.Identification && x.Id != userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _userService.UpdateAsync(userId, input, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Message);
        }
        
        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetOneAsync(x => x.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.RemoveAsync(userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsNotFound);
            Assert.Equal("entity not found or already removed", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenUserIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId };

            _repositoryMock
                .Setup(r => r.GetOneAsync(x => x.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            _repositoryMock
                .Setup(r => r.SoftRemoveAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.RemoveAsync(userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Message);
        }
        
        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetPaginatedListAsync(It.IsAny<Paginate>(),default,It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaginationResult<User>());

            // Act
            var result = await _userService.GetAllAsync(new (),CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Data.Results);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "User1" },
                new User { Id = Guid.NewGuid(), Name = "User2" }
            };

            _repositoryMock
                .Setup(r => r.GetPaginatedListAsync(It.IsAny<Paginate>(),default,It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaginationResult<User>()
                {
                    Results = users
                });

            _mapperMock
                .Setup(m => m.Map<List<GetUser>>(users))
                .Returns(users.Select(u => new GetUser { Id = u.Id, Name = u.Name }).ToList());

            // Act
            var result = await _userService.GetAllAsync(new(),CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Results.Count);
            Assert.Equal("User1", result.Data.Results[0].Name);
            Assert.Equal("User2", result.Data.Results[1].Name);
        }
        
        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetOneAsync(x => x.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetByIdAsync(userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsNotFound);
            Assert.Equal("entity not found", result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var getUser = new GetUser { Id = userId, Name = "Test User" };

            _repositoryMock
                .Setup(r => r.GetOneAsync(x => x.Id == userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapperMock
                .Setup(m => m.Map<GetUser>(user))
                .Returns(getUser);

            // Act
            var result = await _userService.GetByIdAsync(userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.Id);
            Assert.Equal("Test User", result.Data.Name);
        }
    }
}