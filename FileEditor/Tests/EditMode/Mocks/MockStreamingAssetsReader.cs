using System.Collections.Generic;
using System.Threading.Tasks;
using Core.FileEditor.Readers;
using FluentResults;

namespace Core.FileEditor.Tests.EditMode.Mocks
{
    public sealed class MockStreamingAssetsReader : IStreamingAssetsReader
    {
        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();
        private readonly HashSet<string> _failingPaths = new HashSet<string>();

        public int ReadCallCount { get; private set; }
        public int ExistsCallCount { get; private set; }
        public string LastReadPath { get; private set; }
        public string LastExistsPath { get; private set; }

        public void Reset()
        {
            _files.Clear();
            _failingPaths.Clear();
            ReadCallCount = 0;
            ExistsCallCount = 0;
            LastReadPath = null;
            LastExistsPath = null;
        }

        public void AddFile(string path, string content)
        {
            var normalizedPath = NormalizePath(path);
            _files[normalizedPath] = content;
        }

        public void RemoveFile(string path)
        {
            var normalizedPath = NormalizePath(path);
            _files.Remove(normalizedPath);
        }

        public void SetPathToFail(string path)
        {
            var normalizedPath = NormalizePath(path);
            _failingPaths.Add(normalizedPath);
        }

        public Task<Result<string>> ReadTextAsync(string fullPath)
        {
            ReadCallCount++;
            LastReadPath = fullPath;

            var normalizedPath = NormalizePath(fullPath);

            if (_failingPaths.Contains(normalizedPath))
            {
                return Task.FromResult(Result.Fail<string>($"Read failed for path: {fullPath}"));
            }

            if (_files.TryGetValue(normalizedPath, out var content))
            {
                return Task.FromResult(Result.Ok(content));
            }

            return Task.FromResult(Result.Fail<string>($"File not found: {fullPath}"));
        }

        public Task<Result<bool>> ExistsAsync(string fullPath)
        {
            ExistsCallCount++;
            LastExistsPath = fullPath;

            var normalizedPath = NormalizePath(fullPath);

            if (_failingPaths.Contains(normalizedPath))
            {
                return Task.FromResult(Result.Fail<bool>($"Exists check failed for path: {fullPath}"));
            }

            var exists = _files.ContainsKey(normalizedPath);
            return Task.FromResult(Result.Ok(exists));
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimEnd('/');
        }
    }
}
