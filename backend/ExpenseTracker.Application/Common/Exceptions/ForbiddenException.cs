namespace ExpenseTracker.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Forbidden resource.") 
        : base(message) { }
}