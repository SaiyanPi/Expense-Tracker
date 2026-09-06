namespace ExpenseTracker.Application.Common.ProfileImage;

public sealed record ProfileImageUpdate(
    Stream Content,
    string FileName,
    long Length,
    string? ContentType);