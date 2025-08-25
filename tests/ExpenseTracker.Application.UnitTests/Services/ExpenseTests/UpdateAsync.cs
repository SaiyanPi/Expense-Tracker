using AutoMapper;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.DTOs.Expense;
using ExpenseTrackler.Application.Mappings;
using FluentAssertions;
using Moq;

namespace ExpenseTests
{
    public class UpdateAsyncTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepository = new();
        private readonly IMapper _mapper;

        public UpdateAsyncTests()
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
        public async Task WhenExpenseExists_ShouldUpdateAndCallRepository()
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

            var updateDto = new UpdateExpenseDto
            {
                Title = "Lunch Updated",
                Description = "Lunch at new cafe",
                Amount = 15.0m,
                Date = DateTime.Today,
                CategoryId = existingExpense.CategoryId
            };

            _expenseRepository
                .Setup(r => r.GetByIdAsync(existingExpense.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingExpense);

            Expense? capturedExpense = null;
            _expenseRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => capturedExpense = e)
                .Returns(Task.CompletedTask);

            var sut = CreateSut();

            // Act
            await sut.UpdateAsync(existingExpense.Id, updateDto);

            // Assert
            _expenseRepository.Verify(r => r.GetByIdAsync(existingExpense.Id, It.IsAny<CancellationToken>()), Times.Once);
            _expenseRepository.Verify(r => r.UpdateAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Once);

            capturedExpense.Should().NotBeNull();
            capturedExpense!.Id.Should().Be(existingExpense.Id); // Id should remain the same
            capturedExpense.Title.Should().Be(updateDto.Title);
            capturedExpense.Description.Should().Be(updateDto.Description);
            capturedExpense.Amount.Should().Be(updateDto.Amount);
            capturedExpense.CategoryId.Should().Be(updateDto.CategoryId);
        }

        [Fact]
        public async Task WhenExpenseDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updateDto = new UpdateExpenseDto
            {
                Title = "Any",
                Description = "Any",
                Amount = 10m,
                Date = DateTime.Today,
                CategoryId = Guid.NewGuid()
            };

            _expenseRepository
                .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expense?)null);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.UpdateAsync(nonExistentId, updateDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Expense with id {nonExistentId} not found");

            _expenseRepository.Verify(r => r.UpdateAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
