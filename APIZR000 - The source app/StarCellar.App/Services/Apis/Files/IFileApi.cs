using Refit;

namespace StarCellar.App.Services.Apis.Files
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }
}
