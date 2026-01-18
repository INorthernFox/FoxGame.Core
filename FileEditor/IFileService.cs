using System.Threading.Tasks;
using FluentResults;

namespace Core.FileEditor
{
    public interface IFileService
    {
        string GetBasePath(DirectoryType directoryType);

        Task<Result<T>> ReadAsync<T>(string fullPath) where T : class;
        Task<Result> WriteAsync<T>(string fullPath, T data) where T : class;
        Task<Result<bool>> ExistsAsync(string fullPath);
        Result Delete(string fullPath);
    }
}
