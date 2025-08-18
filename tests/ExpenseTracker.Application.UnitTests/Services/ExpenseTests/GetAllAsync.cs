using AutoMapper;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.Mappings;
using FluentAssertions;
using Moq;

namespace ExpenseTests
{
    public class GetAllAsyncTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepository = new();
        private readonly IMapper _mapper;

        public GetAllAsyncTests()
        {
            // Configure AutoMapper with your ExpenseMappingProfile
            // 1. Create configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ExpenseMappingProfile>();
            });

            // 2. Validate (optional but recommended)
            config.AssertConfigurationIsValid();

            // 3. Create IMapper instance
            _mapper = config.CreateMapper();
        }

        private ExpenseService CreateSut() =>
            new ExpenseService(_expenseRepository.Object, _mapper);

        [Fact]
        public async Task WhenExpensesExist_ShouldReturnMappedDtos()
        {
            // Arrange
            var expenses = new List<Expense>
            {
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Title = "Lunch",
                    Description = "Lunch at cafe",
                    Amount = 12.5m,
                    Date = DateTime.Today,
                    CategoryId = Guid.NewGuid(),
                     UserId = "002",
                    Category = new Category { Id = Guid.NewGuid(), Name = "Food" }
                },
                new Expense
                {
                    Id = Guid.NewGuid(),
                    Title = "Taxi",
                    Description = "Airport taxi",
                    Amount = 25m,
                    Date = DateTime.Today,
                    CategoryId = Guid.NewGuid(),
                    UserId = "001",
                    Category = new Category { Id = Guid.NewGuid(), Name = "Transport" }

                }
            };

            _expenseRepository
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expenses);

            var sut = CreateSut();

            // Act
            var result = await sut.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Select(x => x.Title).Should().Contain(new[] { "Lunch", "Taxi" });
            result.Select(x => x.Amount).Should().Contain(new[] { 12.5m, 25m });

            _expenseRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WhenNoExpensesExist_ShouldReturnEmptyList()
        {
            // Arrange
            _expenseRepository
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Expense>());

            var sut = CreateSut();

            // Act
            var result = await sut.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
