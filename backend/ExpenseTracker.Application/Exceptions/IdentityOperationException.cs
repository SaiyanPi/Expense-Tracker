namespace ExpenseTracker.Application.Exceptions;
public class IdentityOperationException : Exception
{
    public IdentityOperationException(string message)
        : base(message)
    {
    }
}