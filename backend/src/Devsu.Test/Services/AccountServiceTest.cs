using Devsu.Application.Dtos.Accounts;
using Devsu.Application.Dtos.Core;
using Devsu.Application.Services.Accounts;

namespace Devsu.Test.Services;

public class AccountServiceTest
{
    private readonly Mock<IAccountRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AccountService>> _loggerMock;
    private readonly AccountService _accountService;

    public AccountServiceTest()
    {
        _repositoryMock = new();
        _mapperMock = new();
        _loggerMock = new();
        _accountService = new (_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenAccountAlreadyExists()
    {
        // Arrange
        var input = new CreateAccount
        {
            AccountNumber = "123456",
            InitialBalance = 1000
        };

        _repositoryMock
            .Setup(r => r.ExistAsync(x => x.AccountNumber == input.AccountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _accountService.CreateAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Cuenta con AccountNumber ya existe", result.Message);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenAccountIsCreated()
    {
        // Arrange
        var input = new CreateAccount
        {
            AccountNumber = "123456",
            InitialBalance = 1000
        };

        var createdAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = input.AccountNumber,
            CurrentBalance = input.InitialBalance
        };

        _repositoryMock
            .Setup(r => r.ExistAsync(x => x.AccountNumber == input.AccountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(m => m.Map<Account>(input))
            .Returns(createdAccount);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdAccount);

        // Act
        var result = await _accountService.CreateAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(createdAccount.Id, result.Data);
    }
    
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenAccountNotFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var input = new EditAccount
        {
            AccountNumber = "123456",
            AccountType = "Savings"
        };

        _repositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _accountService.UpdateAsync(accountId, input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("Account not found", result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenAccountIsUpdated()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var existingAccount = new Account
        {
            Id = accountId,
            AccountNumber = "123456",
            AccountType = "Checking"
        };

        var input = new EditAccount
        {
            AccountNumber = "654321",
            AccountType = "Savings"
        };

        _repositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAccount);

        _repositoryMock
            .Setup(r => r.ExistAsync(x => x.AccountNumber == input.AccountNumber && x.Id != accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAccount);

        // Act
        var result = await _accountService.UpdateAsync(accountId, input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Account>(a => a.AccountNumber == input.AccountNumber && a.AccountType == input.AccountType), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenNotFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _accountService.RemoveAsync(accountId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("entity not found or already removed", result.Message);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenAccountIsDeleted()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account { Id = accountId };

        _repositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _repositoryMock
            .Setup(r => r.SoftRemoveAsync(account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _accountService.RemoveAsync(accountId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.SoftRemoveAsync(account.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnError_WhenAccountNotFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _accountService.GetByIdAsync(accountId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("entity not found", result.Message);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenAccountIsFound()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account
        {
            Id = accountId,
            AccountNumber = "123456",
            AccountType = "Savings"
        };

        var accountDto = new GetAccount
        {
            Id = accountId,
            AccountNumber = "123456",
            AccountType = "Savings"
        };

        _repositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _mapperMock
            .Setup(m => m.Map<GetAccount>(account))
            .Returns(accountDto);

        // Act
        var result = await _accountService.GetByIdAsync(accountId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(accountDto, result.Data);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyResult_WhenNoAccountsFound()
    {
        // Arrange
        var paginate = new Paginate { Page = 1, Qyt = 10 };
        var emptyResult = new PaginationResult<Account>
        {
            Results = new List<Account>(),
            Total = 0
        };

        _repositoryMock
            .Setup(r => r.GetPaginatedListAsync(paginate, null,It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _accountService.GetAllAsync(paginate, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Data.Results);
        Assert.Equal(0, result.Data.Total);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccess_WhenAccountsAreFound()
    {
        // Arrange
        var paginate = new Paginate { Page = 1, Qyt = 10 };
        var accounts = new List<Account>
        {
            new() { Id = Guid.NewGuid(), AccountNumber = "123456", AccountType = "Savings" },
            new () { Id = Guid.NewGuid(), AccountNumber = "654321", AccountType = "Checking" }
        };

        var resultData = new PaginationResult<Account>
        {
            Results = accounts,
            Total = accounts.Count
        };

        _repositoryMock
            .Setup(r => r.GetPaginatedListAsync(paginate, default,It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultData);

        // Act
        var result = await _accountService.GetAllAsync(paginate, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(accounts.Count, result.Data.Total);
    }
    
}