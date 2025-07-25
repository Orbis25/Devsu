using System.Linq.Expressions;
using Devsu.Application.Dtos.Accounts;
using Devsu.Application.Dtos.Core;
using Devsu.Application.Dtos.Transactions;
using Devsu.Application.Services.Core.Pdf;
using Devsu.Application.Services.Transactions;

namespace Devsu.Test.Services;

public class TransactionServiceTest
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ILogger<TransactionService>> _loggerMock;
    private readonly Mock<IPdfService> _pdfServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TransactionService _transactionService;

    public TransactionServiceTest()
    {
        _transactionRepositoryMock = new();
        _accountRepositoryMock = new();
        _loggerMock = new();
        _pdfServiceMock = new();
        _mapperMock = new();

        _transactionService = new TransactionService(
            _transactionRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _accountRepositoryMock.Object,
            _pdfServiceMock.Object
        );
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenAccountNotFound()
    {
        // Arrange
        var input = new CreateTransaction
        {
            AccountId = Guid.NewGuid(),
            Amount = 100,
            Type = "debit"
        };

        _accountRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == input.AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _transactionService.CreateAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("Account not found", result.Message);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenTransactionIsCreated()
    {
        CancellationToken cancellationToken = new();
        // Arrange
        var accountId = Guid.Parse("f02da3fb-715c-4aa8-9df3-9e83e7554970");
        var account = new Account
        {
            Id = accountId,
            CurrentBalance = 500,
            DailyDebitLimit = 1000,
            DailyDebit = 0
        };

        var input = new CreateTransaction
        {
            AccountId = accountId,
            Amount = 100,
            Type = "debito"
        };

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Amount = input.Amount,
            Type = input.Type,
            CurrentBalance = account.CurrentBalance - input.Amount
        };

        _accountRepositoryMock
            .Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Account,bool>>>(), cancellationToken))
            .ReturnsAsync(account);

        _transactionRepositoryMock
            .Setup(r => r.Attach(It.IsAny<Transaction>()))
            .Returns(transaction);

        // Act
        var result = await _transactionService.CreateAsync(input, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(transaction.Id, result.Data);
        _transactionRepositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenAccountNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var input = new EditTransaction
        {
            AccountId = Guid.NewGuid(),
            Amount = 200,
            Type = "debit"
        };

        _accountRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == input.AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _transactionService.UpdateAsync(transactionId, input, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("Account no encontrada", result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenTransactionIsUpdated()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var account = new Account
        {
            Id = accountId,
            CurrentBalance = 500,
            DailyDebitLimit = 1000,
            DailyDebit = 100
        };

        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 100,
            Type = "debit",
            CurrentBalance = 400
        };

        var input = new EditTransaction
        {
            AccountId = accountId,
            Amount = 200,
            Type = "debit"
        };

        _accountRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == input.AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _transactionRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        _transactionRepositoryMock
            .Setup(r => r.Attach(It.IsAny<Transaction>()))
            .Returns(transaction);

        // Act
        var result = await _transactionService.UpdateAsync(transactionId, input, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _transactionRepositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task RemoveAsync_ShouldReturnError_WhenTransactionNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        _transactionRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction)null);

        // Act
        var result = await _transactionService.RemoveAsync(transactionId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("Transaction no encontrada", result.Message);
    }
    
    [Fact]
    public async Task RemoveAsync_ShouldReturnSuccess_WhenTransactionIsRemoved()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 100,
            Type = "debit",
            CurrentBalance = 400
        };

        var account = new Account
        {
            Id = accountId,
            CurrentBalance = 500,
            DailyDebit = 100
        };

        _transactionRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        _accountRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _transactionRepositoryMock
            .Setup(r => r.SoftRemoveAsync(transactionId, It.IsAny<CancellationToken>()));

        // Act
        var result = await _transactionService.RemoveAsync(transactionId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _transactionRepositoryMock.Verify(r => r.SoftRemoveAsync(transactionId,It.IsAny<CancellationToken>()), Times.Once);
        _accountRepositoryMock.Verify(r => r.Attach(account), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnError_WhenTransactionNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        _transactionRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction)null);

        // Act
        var result = await _transactionService.GetByIdAsync(transactionId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsNotFound);
        Assert.Equal("entity not found", result.Message);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenTransactionIsFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            Amount = 100,
            Type = "debit",
            CurrentBalance = 400
        };

        var mappedTransaction = new GetTransaction
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            Type = transaction.Type,
            CurrentBalance = transaction.CurrentBalance
        };

        _transactionRepositoryMock
            .Setup(r => r.GetOneAsync(x => x.Id == transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        _mapperMock
            .Setup(m => m.Map<GetTransaction>(transaction))
            .Returns(mappedTransaction);

        // Act
        var result = await _transactionService.GetByIdAsync(transactionId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(mappedTransaction, result.Data);
    }
    
    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyResult_WhenNoTransactionsFound()
    {
        // Arrange
        var searchCriteria = new SearchTransactions
        {
            Query = "nonexistent",
            From = DateTime.MinValue,
            To = DateTime.MaxValue,
            Page = 1,
            Qyt = 10
        };

        var emptyResult = new PaginationResult<Transaction>
        {
            Results = new List<Transaction>(),
            ActualPage = 1,
            Qyt = 0,
            PageTotal = 0,
            Total = 0
        };

        _transactionRepositoryMock
            .Setup(r => r.GetPaginatedListAsync(searchCriteria, It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _transactionService.SearchAsync<GetTransaction>(searchCriteria, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Data.Results);
    }
    
    [Fact]
    public async Task SearchAsync_ShouldReturnPaginatedResult_WhenTransactionsFound()
    {
        // Arrange
        var searchCriteria = new SearchTransactions
        {
            Query = "test",
            From = DateTime.MinValue,
            To = DateTime.MaxValue,
            Page = 1,
            Qyt = 10
        };

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = 100,
                Type = "debit",
                CurrentBalance = 400
            }
        };

        var paginatedResult = new PaginationResult<Transaction>
        {
            Results = transactions,
            ActualPage = 1,
            Qyt = 1,
            PageTotal = 1,
            Total = 1
        };

        var mappedTransactions = transactions.Select(t => new GetTransaction
        {
            Id = t.Id,
            AccountId = t.AccountId,
            Amount = t.Amount,
            Type = t.Type,
            CurrentBalance = t.CurrentBalance
        }).ToList();

        _transactionRepositoryMock
            .Setup(r => r.GetPaginatedListAsync(searchCriteria, It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        _mapperMock
            .Setup(m => m.Map<List<GetTransaction>>(transactions))
            .Returns(mappedTransactions);

        // Act
        var result = await _transactionService.SearchAsync<GetTransaction>(searchCriteria, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(mappedTransactions, result.Data.Results);
        Assert.Equal(1, result.Data.Qyt);
    }
    
    [Fact]
    public async Task ExportTransactionReportAsync_ShouldReturnEmptyReport_WhenNoTransactionsFound()
    {
        // Arrange
        var search = new ExportTransactionsSearch
        {
            Query = "nonexistent",
            From = DateTime.MinValue,
            To = DateTime.MaxValue
        };

        var emptyResult = new PaginationResult<Transaction>
        {
            Results = new List<Transaction>()
        };

        _transactionRepositoryMock
            .Setup(r => r.GetPaginatedListAsync(It.IsAny<SearchTransactions>(), It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResult);

        _pdfServiceMock
            .Setup(p => p.GeneratePdfAsBase64(It.IsAny<string>()))
            .Returns(string.Empty);

        // Act
        var result = await _transactionService.ExportTransactionReportAsync(search, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Data);
    }
    
    [Fact]
    public async Task ExportTransactionReportAsync_ShouldReturnReport_WhenTransactionsFound()
    {
        // Arrange
        var search = new ExportTransactionsSearch
        {
            Query = "test",
            From = DateTime.MinValue,
            To = DateTime.MaxValue
        };

        var transactions = new List<Transaction>
        {
            new Transaction()
            {
                CreatedAt = DateTime.Now,
                Account = new()
                {
                    AccountNumber = "123456",
                    AccountType = "Savings",
                    InitialBalance = 1000
                },
                Status = true,
                Type = "debit",
                Amount = 100,
                CurrentBalance = 900
            }
        };

        var paginatedResult = new PaginationResult<Transaction>
        {
            Results = transactions
        };

        _transactionRepositoryMock
            .Setup(r => r.GetPaginatedListAsync(It.IsAny<SearchTransactions>(), It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        _pdfServiceMock
            .Setup(p => p.GeneratePdfAsBase64(It.IsAny<string>()))
            .Returns("base64pdfstring");

        // Act
        var result = await _transactionService.ExportTransactionReportAsync(search, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("base64pdfstring", result.Data);
    }
}