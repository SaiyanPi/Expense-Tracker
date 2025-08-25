using AutoMapper;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.Mappings;
using FluentAssertions;
using Moq;

namespace ExpenseTests
{
    public class DeleteAsyncTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepository = new();
        private readonly IMapper _mapper;

        public DeleteAsyncTests()
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
        public async Task WhenExpenseExists_ShouldCallDeleteOnRepository()
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
                UserId = "001"
            };

            _expenseRepository
                .Setup(r => r.GetByIdAsync(existingExpense.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingExpense);

            Expense? deletedExpense = null;
            _expenseRepository
                .Setup(r => r.DeleteAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => deletedExpense = e)
                .Returns(Task.CompletedTask);

            var sut = CreateSut();

            // Act
            await sut.DeleteAsync(existingExpense.Id);

            // Assert
            _expenseRepository.Verify(r => r.GetByIdAsync(existingExpense.Id, It.IsAny<CancellationToken>()), Times.Once);
            _expenseRepository.Verify(r => r.DeleteAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Once);

            deletedExpense.Should().NotBeNull();
            deletedExpense!.Id.Should().Be(existingExpense.Id);
            deletedExpense.Title.Should().Be(existingExpense.Title);
        }

        [Fact]
        public async Task WhenExpenseDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _expenseRepository
                .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expense?)null);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.DeleteAsync(nonExistentId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Expense with id {nonExistentId} not found");

            _expenseRepository.Verify(r => r.DeleteAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
