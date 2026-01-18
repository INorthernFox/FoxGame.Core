#if UNITY_WEBGL
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FluentResults;
using UnityEngine.Networking;

namespace Core.FileEditor.Readers
{
    /// <summary>
    /// StreamingAssets reader for WebGL platform.
    /// WebGL has no file system access, so UnityWebRequest is required.
    /// </summary>
    public sealed class WebGLStreamingAssetsReader : IStreamingAssetsReader
    {
        public async Task<Result<string>> ReadTextAsync(string fullPath)
        {
            try
            {
                using var request = UnityWebRequest.Get(fullPath);
                await request.SendWebRequest().ToUniTask();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    return Result.Fail<string>($"Failed to read file: {request.error}");
                }

                return Result.Ok(request.downloadHandler.text);
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Failed to read file: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ExistsAsync(string fullPath)
        {
            try
            {
                using var request = UnityWebRequest.Head(fullPath);
                await request.SendWebRequest().ToUniTask();

                var exists = request.result == UnityWebRequest.Result.Success;
                return Result.Ok(exists);
            }
            catch (Exception ex)
            {
                return Result.Fail<bool>($"Failed to check file existence: {ex.Message}");
            }
        }
    }
}
#endif
