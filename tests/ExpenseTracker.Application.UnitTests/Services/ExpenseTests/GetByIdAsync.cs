using AutoMapper;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.Mappings;
using FluentAssertions;
using Moq;

namespace ExpenseTests
{
    public class GetByIdAsyncTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepository = new();
        private readonly IMapper _mapper;

        public GetByIdAsyncTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ExpenseMappingProfile>();
            });
            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        private ExpenseService CreateSut() =>
            new ExpenseService(_expenseRepository.Object, _mapper);

        [Fact]
        public async Task WhenExpenseExists_ShouldReturnMappedDto()
        {
            // Arrange
            var existingExpense = new Expense
            {
                Id = Guid.NewGuid(),
                Title = "Lunch",
                Description = "Lunch at cafe",
                Amount = 12.5m,
                Date = DateTime.Today,
                CategoryId = Guid.NewGuid(),
                UserId = "001",
                Category = new Category { Id = Guid.NewGuid(), Name = "Food" }
            };

            _expenseRepository
                .Setup(r => r.GetByIdAsync(existingExpense.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingExpense);

            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(existingExpense.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(existingExpense.Id);
            result.Title.Should().Be(existingExpense.Title);
            result.Description.Should().Be(existingExpense.Description);
            result.Amount.Should().Be(existingExpense.Amount);
            result.CategoryId.Should().Be(existingExpense.CategoryId);

            _expenseRepository.Verify(r => r.GetByIdAsync(existingExpense.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WhenExpenseDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _expenseRepository
                .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expense?)null);

            var sut = CreateSut();

            // Act
            var result = await sut.GetByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();

            _expenseRepository.Verify(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
