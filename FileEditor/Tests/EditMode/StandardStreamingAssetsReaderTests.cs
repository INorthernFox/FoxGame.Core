using System;
using System.IO;
using System.Threading.Tasks;
using Core.FileEditor.Readers;
using NUnit.Framework;

namespace Core.FileEditor.Tests.EditMode
{
    [TestFixture]
    public sealed class StandardStreamingAssetsReaderTests
    {
        private StandardStreamingAssetsReader _reader;
        private string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _reader = new StandardStreamingAssetsReader();
            _tempDirectory = Path.Combine(Path.GetTempPath(), $"StreamingAssetsReaderTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }

        #region ReadTextAsync Tests

        [Test]
        public async Task ReadTextAsync_ExistingFile_ReturnsContent()
        {
            const string expectedContent = "test content";
            var filePath = Path.Combine(_tempDirectory, "test.txt");
            await File.WriteAllTextAsync(filePath, expectedContent);

            var result = await _reader.ReadTextAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedContent, result.Value);
        }

        [Test]
        public async Task ReadTextAsync_NonExistingFile_ReturnsFailure()
        {
            var filePath = Path.Combine(_tempDirectory, "nonexistent.txt");

            var result = await _reader.ReadTextAsync(filePath);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(result.Errors[0].Message.Contains("not found"));
        }

        [Test]
        public async Task ReadTextAsync_EmptyFile_ReturnsEmptyString()
        {
            var filePath = Path.Combine(_tempDirectory, "empty.txt");
            await File.WriteAllTextAsync(filePath, string.Empty);

            var result = await _reader.ReadTextAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(string.Empty, result.Value);
        }

        [Test]
        public async Task ReadTextAsync_JsonContent_ReturnsValidJson()
        {
            const string jsonContent = "{\"name\":\"test\",\"value\":42}";
            var filePath = Path.Combine(_tempDirectory, "test.json");
            await File.WriteAllTextAsync(filePath, jsonContent);

            var result = await _reader.ReadTextAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(jsonContent, result.Value);
        }

        #endregion

        #region ExistsAsync Tests

        [Test]
        public async Task ExistsAsync_ExistingFile_ReturnsTrue()
        {
            var filePath = Path.Combine(_tempDirectory, "existing.txt");
            await File.WriteAllTextAsync(filePath, "content");

            var result = await _reader.ExistsAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

        [Test]
        public async Task ExistsAsync_NonExistingFile_ReturnsFalse()
        {
            var filePath = Path.Combine(_tempDirectory, "nonexistent.txt");

            var result = await _reader.ExistsAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Value);
        }

        [Test]
        public async Task ExistsAsync_Directory_ReturnsFalse()
        {
            var result = await _reader.ExistsAsync(_tempDirectory);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Value);
        }

        [Test]
        public async Task ExistsAsync_NestedFile_ReturnsTrue()
        {
            var nestedDir = Path.Combine(_tempDirectory, "nested", "dir");
            Directory.CreateDirectory(nestedDir);
            var filePath = Path.Combine(nestedDir, "file.txt");
            await File.WriteAllTextAsync(filePath, "content");

            var result = await _reader.ExistsAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
        }

        #endregion
    }
}
