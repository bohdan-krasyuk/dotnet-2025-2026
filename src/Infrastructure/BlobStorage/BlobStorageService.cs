using Application.Common.Interfaces;
using LanguageExt;

namespace Infrastructure.BlobStorage;

public class BlobStorageService : IFileStorage
{
    public Task<Unit> UploadAsync(Stream stream, string fileFullPath, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Default);
    }
}