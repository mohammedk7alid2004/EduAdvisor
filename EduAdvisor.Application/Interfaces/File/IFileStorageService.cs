namespace EduAdvisor.Application.Interfaces.File;

public interface IFileStorageService
{
    Task<string?> SaveFileAsync(Stream fileStream, string fileName, string folderName);
    Task DeleteFileAsync(string? filePath);
    Task<string?> UpdateFileAsync(Stream fileStream, string fileName, string? oldFilePath, string folderName);
}