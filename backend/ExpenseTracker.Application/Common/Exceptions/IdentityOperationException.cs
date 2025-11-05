namespace ExpenseTracker.Application.Common.Exceptions;
public class IdentityOperationException : Exception
{
    public IdentityOperationException(string message)
        : base(message)
    {
    }
}