namespace ExpenseTracker.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message = "One or more validation errors occurred.") 
        : base(message) { }
}