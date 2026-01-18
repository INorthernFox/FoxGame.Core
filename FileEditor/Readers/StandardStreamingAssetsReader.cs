using System;
using System.IO;
using System.Threading.Tasks;
using FluentResults;

namespace Core.FileEditor.Readers
{
    public sealed class StandardStreamingAssetsReader : IStreamingAssetsReader
    {
        public async Task<Result<string>> ReadTextAsync(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    return Result.Fail<string>($"File not found: {fullPath}");
                }

                var content = await File.ReadAllTextAsync(fullPath);
                return Result.Ok(content);
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Failed to read file: {ex.Message}");
            }
        }

        public Task<Result<bool>> ExistsAsync(string fullPath)
        {
            try
            {
                return Task.FromResult(Result.Ok(File.Exists(fullPath)));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Fail<bool>($"Failed to check file existence: {ex.Message}"));
            }
        }
    }
}
