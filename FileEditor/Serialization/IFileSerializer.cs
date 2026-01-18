using FluentResults;

namespace Core.FileEditor.Serialization
{
    public interface IFileSerializer
    {
        Result<string> Serialize<T>(T data) where T : class;
        Result<T> Deserialize<T>(string content) where T : class;
    }
}
