using ExpenseTracker.Persistence.DI;
using ExpenseTracker.Application.DI;
using ExpenseTracker.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

// Register Application services
builder.Services.AddApplicationServices();
// Register Persistence services
builder.Services.AddPersistenceServices(builder.Configuration);
// Register Infrastructure services (optional)
builder.Services.AddInfrastructureServices();

// Add controllers / minimal APIs
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();


