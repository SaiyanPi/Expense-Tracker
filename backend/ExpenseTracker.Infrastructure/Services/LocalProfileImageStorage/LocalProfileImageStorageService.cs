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

    public async Task<string> SaveAsync(Stream image, string fileName,
        CancellationToken cancellationToken = default)
    {
        const long maxFileSize = 5 * 1024 * 1024; // 5 MB

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                "Only JPG, JPEG, and PNG images are allowed.");
        }

        if (image.CanSeek && image.Length > maxFileSize)
        {
            throw new InvalidOperationException(
                "Profile image cannot exceed 5 MB.");
        }

        // Validate the actual file content.
        if (!await IsValidImageSignatureAsync(
                image,
                extension,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "The uploaded file is not a valid image.");
        }

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "profile-images");

        Directory.CreateDirectory(uploadsFolder);

        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            storedFileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await image.CopyToAsync(
            fileStream,
            cancellationToken);

        var request = _httpContextAccessor.HttpContext?.Request;

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

    private static async Task<bool> IsValidImageSignatureAsync(Stream image, string extension,
        CancellationToken cancellationToken)
    {
        if (!image.CanSeek)
            return false;

        image.Position = 0;

        byte[] signature = extension switch
        {
            ".jpg" or ".jpeg" =>
                [0xFF, 0xD8, 0xFF],

            ".png" =>
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],

            _ => []
        };

        if (signature.Length == 0)
            return false;

        var buffer = new byte[signature.Length];

        var bytesRead = await image.ReadAsync(
            buffer,
            cancellationToken);

        image.Position = 0;

        return bytesRead == signature.Length &&
            buffer.SequenceEqual(signature);
    }
}