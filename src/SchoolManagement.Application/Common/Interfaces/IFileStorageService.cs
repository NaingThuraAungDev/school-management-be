using Microsoft.AspNetCore.Http;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string subDirectory, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    string GetFileUrl(string filePath);
}
