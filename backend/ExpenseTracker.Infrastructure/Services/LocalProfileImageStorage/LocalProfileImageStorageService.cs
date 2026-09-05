using ExpenseTracker.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Infrastructure.Services.LocalProfileImageStorage;

public class LocalProfileImageStorageService : IProfileImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LocalProfileImageStorageService(IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveAsync(Stream image, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "profile-images");

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(uploadsFolder, storedFileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await image.CopyToAsync(fileStream, cancellationToken);

        var request = _httpContextAccessor.HttpContext?.Request;

        // This will save the imageUrl as: http://localhost:5167/uploads/profile-images/76b3e34e-906b-48b7-9e82-2cc881a236d9.png
        return $"{request?.Scheme}://{request?.Host}/uploads/profile-images/{storedFileName}";
    }

    public Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Task.CompletedTask;

        var relativePath = imageUrl.TrimStart('/');

        var filePath = Path.Combine(
            _environment.WebRootPath,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}