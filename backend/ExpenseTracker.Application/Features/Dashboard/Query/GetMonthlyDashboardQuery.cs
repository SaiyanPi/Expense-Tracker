using ExpenseTracker.Application.DTOS.Dashboard;
using MediatR;

namespace ExpenseTracker.Application.Features.Dashboard.Query;

public record GetMonthlyDashboardQuery(string UserId) : IRequest<DashboardSummaryDto>;