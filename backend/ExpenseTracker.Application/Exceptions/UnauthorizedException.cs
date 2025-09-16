namespace ExpenseTracker.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access.") 
        : base(message) { }
}