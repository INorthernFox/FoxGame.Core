using System.Threading.Tasks;
using FluentResults;

namespace Core.FileEditor.Readers
{
    public interface IStreamingAssetsReader
    {
        Task<Result<string>> ReadTextAsync(string fullPath);
        Task<Result<bool>> ExistsAsync(string fullPath);
    }
}
