using LanguageExt;

namespace Application.Common.Interfaces;

public interface IFileStorage
{
    Task<Unit> UploadAsync(Stream stream, string fileFullPath, CancellationToken cancellationToken);
}