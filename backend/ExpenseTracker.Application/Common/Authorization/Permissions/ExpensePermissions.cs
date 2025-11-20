namespace ExpenseTracker.Application.Common.Authorization.Permissions;

public static class ExpensePermissions
{
    public const string GroupName = "Permissions.Expenses";
    
    public const string ViewExpenses = "Permissions.Expenses.View";
    public const string CreateExpense = "Permissions.Expenses.Create";
    public const string EditExpense = "Permissions.Expenses.Edit";
    public const string DeleteExpense = "Permissions.Expenses.Delete";
}