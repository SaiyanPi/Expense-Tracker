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
    public class CreateAsyncTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepository = new();
        private readonly IMapper _mapper;

        public CreateAsyncTests()
        {
            // Use your actual mapping profile for consistency
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
        public async Task WhenValidDtoIsProvided_ShouldReturnNewExpenseId_AndCallRepository()
        {
            // Arrange
            var dto = new CreateExpenseDto
            {
                Title = "Coffee",
                Description = "Morning coffee",
                Amount = 5.0m,
                Date = DateTime.Today,
                CategoryId = Guid.NewGuid(),
                UserId = "001"
            };

            Expense? capturedExpense = null;

            _expenseRepository
                .Setup(r => r.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
                .Callback<Expense, CancellationToken>((e, _) => capturedExpense = e)
                .Returns(Task.CompletedTask);

            var sut = CreateSut();

            // Act
            var result = await sut.CreateAsync(dto);

            // Assert
            result.Should().NotBe(Guid.Empty);

            _expenseRepository.Verify(r => r.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Once);

            capturedExpense.Should().NotBeNull();
            capturedExpense!.Title.Should().Be(dto.Title);
            capturedExpense.Description.Should().Be(dto.Description);
            capturedExpense.Amount.Should().Be(dto.Amount);
            capturedExpense.CategoryId.Should().Be(dto.CategoryId);
            capturedExpense.UserId.Should().Be(dto.UserId);
        }
    }
}
