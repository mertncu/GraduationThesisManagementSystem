using GTMS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GTMS.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment environment, ILogger<FileStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderPath)
    {
        try
        {
            var absoluteFolder = Path.Combine(_environment.WebRootPath, folderPath);
            if (!Directory.Exists(absoluteFolder))
            {
                Directory.CreateDirectory(absoluteFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var absolutePath = Path.Combine(absoluteFolder, uniqueFileName);

            using (var outputStream = new FileStream(absolutePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(outputStream);
            }

            return $"/{folderPath}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName} to {FolderPath}", fileName, folderPath);
            throw;
        }
    }
}
