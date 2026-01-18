using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.FileEditor.Tests.EditMode.Mocks;
using Core.FileEditor.Tests.EditMode.TestData;
using NUnit.Framework;

namespace Core.FileEditor.Tests.EditMode
{
    [TestFixture]
    public sealed class FileServiceTests
    {
        private MockGameLogger _logger;
        private MockFileSerializer _serializer;
        private MockStreamingAssetsReader _streamingAssetsReader;
        private FileService _fileService;
        private string _tempDirectory;
        private string _streamingAssetsPath;
        private string _persistentDataPath;

        [SetUp]
        public void SetUp()
        {
            _logger = new MockGameLogger();
            _serializer = new MockFileSerializer();
            _streamingAssetsReader = new MockStreamingAssetsReader();

            _tempDirectory = Path.Combine(Path.GetTempPath(), $"FileServiceTests_{Guid.NewGuid()}");
            _streamingAssetsPath = Path.Combine(_tempDirectory, "StreamingAssets");
            _persistentDataPath = Path.Combine(_tempDirectory, "PersistentData");

            Directory.CreateDirectory(_streamingAssetsPath);
            Directory.CreateDirectory(_persistentDataPath);

            var directories = new Dictionary<DirectoryType, DirectoryConfig>
            {
                { DirectoryType.StreamingAssets, new DirectoryConfig(_streamingAssetsPath, DirectoryPermission.ReadOnly) },
                { DirectoryType.PersistentData, new DirectoryConfig(_persistentDataPath, DirectoryPermission.ReadWrite) }
            };

            _fileService = new FileService(_logger, _serializer, _streamingAssetsReader, directories);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }

        #region GetBasePath Tests

        [Test]
        public void GetBasePath_StreamingAssets_ReturnsCorrectPath()
        {
            var result = _fileService.GetBasePath(DirectoryType.StreamingAssets);

            Assert.AreEqual(_streamingAssetsPath, result);
        }

        [Test]
        public void GetBasePath_PersistentData_ReturnsCorrectPath()
        {
            var result = _fileService.GetBasePath(DirectoryType.PersistentData);

            Assert.AreEqual(_persistentDataPath, result);
        }

        [Test]
        public void GetBasePath_None_ReturnsEmptyString()
        {
            var result = _fileService.GetBasePath(DirectoryType.None);

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetBasePath_UnregisteredType_ReturnsEmptyString()
        {
            var directories = new Dictionary<DirectoryType, DirectoryConfig>();
            var service = new FileService(_logger, _serializer, _streamingAssetsReader, directories);

            var result = service.GetBasePath(DirectoryType.StreamingAssets);

            Assert.AreEqual(string.Empty, result);
        }

        #endregion

        #region ReadAsync Tests

        [Test]
        public async Task ReadAsync_FromStreamingAssets_UsesStreamingAssetsReader()
        {
            var filePath = Path.Combine(_streamingAssetsPath, "test.json");
            const string content = "{\"Name\":\"Test\",\"Value\":1}";
            var expectedData = new SimpleData("Test", 1);

            _streamingAssetsReader.AddFile(filePath, content);
            _serializer.SetupDeserializeSuccess(expectedData);

            var result = await _fileService.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, _streamingAssetsReader.ReadCallCount);
            Assert.AreEqual(filePath, _streamingAssetsReader.LastReadPath);
        }

        [Test]
        public async Task ReadAsync_FromPersistentData_ReadsFromFileSystem()
        {
            var filePath = Path.Combine(_persistentDataPath, "test.json");
            const string content = "{\"Name\":\"Test\",\"Value\":1}";
            var expectedData = new SimpleData("Test", 1);

            await File.WriteAllTextAsync(filePath, content);
            _serializer.SetupDeserializeSuccess(expectedData);

            var result = await _fileService.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, _streamingAssetsReader.ReadCallCount);
        }

        [Test]
        public async Task ReadAsync_NullPath_ReturnsFailure()
        {
            var result = await _fileService.ReadAsync<SimpleData>(null);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("null or empty"));
        }

        [Test]
        public async Task ReadAsync_EmptyPath_ReturnsFailure()
        {
            var result = await _fileService.ReadAsync<SimpleData>(string.Empty);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("null or empty"));
        }

        [Test]
        public async Task ReadAsync_PathOutsideAllowedDirectories_ReturnsFailure()
        {
            var outsidePath = Path.Combine(Path.GetTempPath(), "outside.json");

            var result = await _fileService.ReadAsync<SimpleData>(outsidePath);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("not in allowed directories"));
        }

        [Test]
        public async Task ReadAsync_FileNotFound_ReturnsFailure()
        {
            var filePath = Path.Combine(_persistentDataPath, "nonexistent.json");

            var result = await _fileService.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public async Task ReadAsync_DeserializationFails_LogsErrorAndReturnsFailure()
        {
            var filePath = Path.Combine(_persistentDataPath, "test.json");
            await File.WriteAllTextAsync(filePath, "invalid json");
            _serializer.SetupDeserializeFailure("Deserialization error");

            var result = await _fileService.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("deserialize"));
        }

        [Test]
        public async Task ReadAsync_Success_LogsInfo()
        {
            var filePath = Path.Combine(_persistentDataPath, "test.json");
            const string content = "{\"Name\":\"Test\"}";
            var expectedData = new SimpleData("Test", 0);

            await File.WriteAllTextAsync(filePath, content);
            _serializer.SetupDeserializeSuccess(expectedData);

            var result = await _fileService.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(_logger.HasInfo("Read:"));
        }

        #endregion

        #region WriteAsync Tests

        [Test]
        public async Task WriteAsync_ToPersistentData_Success()
        {
            var filePath = Path.Combine(_persistentDataPath, "output.json");
            var data = new SimpleData("Test", 42);
            const string serializedJson = "{\"Name\":\"Test\",\"Value\":42}";

            _serializer.SetupSerializeSuccess(serializedJson);

            var result = await _fileService.WriteAsync(filePath, data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(File.Exists(filePath));
            Assert.AreEqual(serializedJson, await File.ReadAllTextAsync(filePath));
        }

        [Test]
        public async Task WriteAsync_ToStreamingAssets_ReturnsFailure()
        {
            var filePath = Path.Combine(_streamingAssetsPath, "output.json");
            var data = new SimpleData("Test", 42);

            var result = await _fileService.WriteAsync(filePath, data);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("Write access denied"));
        }

        [Test]
        public async Task WriteAsync_NullPath_ReturnsFailure()
        {
            var data = new SimpleData("Test", 42);

            var result = await _fileService.WriteAsync(null, data);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public async Task WriteAsync_EmptyPath_ReturnsFailure()
        {
            var data = new SimpleData("Test", 42);

            var result = await _fileService.WriteAsync(string.Empty, data);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public async Task WriteAsync_PathOutsideAllowedDirectories_ReturnsFailure()
        {
            var outsidePath = Path.Combine(Path.GetTempPath(), "outside.json");
            var data = new SimpleData("Test", 42);

            var result = await _fileService.WriteAsync(outsidePath, data);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("not in allowed directories"));
        }

        [Test]
        public async Task WriteAsync_SerializationFails_LogsErrorAndReturnsFailure()
        {
            var filePath = Path.Combine(_persistentDataPath, "output.json");
            var data = new SimpleData("Test", 42);

            _serializer.SetupSerializeFailure("Serialization error");

            var result = await _fileService.WriteAsync(filePath, data);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("serialize"));
        }

        [Test]
        public async Task WriteAsync_CreatesNestedDirectories()
        {
            var filePath = Path.Combine(_persistentDataPath, "nested", "dir", "output.json");
            var data = new SimpleData("Test", 42);

            _serializer.SetupSerializeSuccess("{}");

            var result = await _fileService.WriteAsync(filePath, data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public async Task WriteAsync_Success_LogsInfo()
        {
            var filePath = Path.Combine(_persistentDataPath, "output.json");
            var data = new SimpleData("Test", 42);

            _serializer.SetupSerializeSuccess("{}");

            var result = await _fileService.WriteAsync(filePath, data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(_logger.HasInfo("Wrote:"));
        }

        #endregion

        #region ExistsAsync Tests

        [Test]
        public async Task ExistsAsync_InStreamingAssets_UsesStreamingAssetsReader()
        {
            var filePath = Path.Combine(_streamingAssetsPath, "test.json");
            _streamingAssetsReader.AddFile(filePath, "content");

            var result = await _fileService.ExistsAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
            Assert.AreEqual(1, _streamingAssetsReader.ExistsCallCount);
        }

        [Test]
        public async Task ExistsAsync_InPersistentData_UsesFileSystem()
        {
            var filePath = Path.Combine(_persistentDataPath, "test.json");
            await File.WriteAllTextAsync(filePath, "content");

            var result = await _fileService.ExistsAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value);
            Assert.AreEqual(0, _streamingAssetsReader.ExistsCallCount);
        }

        [Test]
        public async Task ExistsAsync_NonExistingFile_ReturnsFalse()
        {
            var filePath = Path.Combine(_persistentDataPath, "nonexistent.json");

            var result = await _fileService.ExistsAsync(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Value);
        }

        [Test]
        public async Task ExistsAsync_NullPath_ReturnsFailure()
        {
            var result = await _fileService.ExistsAsync(null);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public async Task ExistsAsync_PathOutsideAllowedDirectories_ReturnsFailure()
        {
            var outsidePath = Path.Combine(Path.GetTempPath(), "outside.json");

            var result = await _fileService.ExistsAsync(outsidePath);

            Assert.IsTrue(result.IsFailed);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ExistingFile_Success()
        {
            var filePath = Path.Combine(_persistentDataPath, "test.json");
            await File.WriteAllTextAsync(filePath, "content");

            var result = _fileService.Delete(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Delete_NonExistingFile_ReturnsFailureAndLogsWarning()
        {
            var filePath = Path.Combine(_persistentDataPath, "nonexistent.json");

            var result = _fileService.Delete(filePath);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasWarning("not found"));
        }

        [Test]
        public void Delete_FromStreamingAssets_ReturnsFailure()
        {
            var filePath = Path.Combine(_streamingAssetsPath, "test.json");

            var result = _fileService.Delete(filePath);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("Write access denied"));
        }

        [Test]
        public void Delete_NullPath_ReturnsFailure()
        {
            var result = _fileService.Delete(null);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public void Delete_EmptyPath_ReturnsFailure()
        {
            var result = _fileService.Delete(string.Empty);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public void Delete_PathOutsideAllowedDirectories_ReturnsFailure()
        {
            var outsidePath = Path.Combine(Path.GetTempPath(), "outside.json");

            var result = _fileService.Delete(outsidePath);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public async Task Delete_Success_LogsInfo()
        {
            var filePath = Path.Combine(_persistentDataPath, "test.json");
            await File.WriteAllTextAsync(filePath, "content");

            var result = _fileService.Delete(filePath);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(_logger.HasInfo("Deleted:"));
        }

        #endregion

        #region Path Normalization Tests

        [Test]
        public async Task ReadAsync_BackslashPath_NormalizesCorrectly()
        {
            var filePath = _persistentDataPath.Replace('/', '\\') + "\\test.json";
            await File.WriteAllTextAsync(filePath.Replace('\\', '/'), "content");
            _serializer.SetupDeserializeSuccess(new SimpleData());

            var result = await _fileService.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task WriteAsync_TrailingSlashPath_Works()
        {
            var baseWithSlash = _persistentDataPath + "/";
            var filePath = baseWithSlash + "test.json";
            _serializer.SetupSerializeSuccess("{}");

            var result = await _fileService.WriteAsync(filePath, new SimpleData());

            Assert.IsTrue(result.IsSuccess);
        }

        #endregion

        #region Permission Tests

        [Test]
        public async Task ReadAsync_NonePermission_ReturnsFailure()
        {
            var noReadPath = Path.Combine(_tempDirectory, "NoRead");
            Directory.CreateDirectory(noReadPath);
            var filePath = Path.Combine(noReadPath, "test.json");
            await File.WriteAllTextAsync(filePath, "content");

            var directories = new Dictionary<DirectoryType, DirectoryConfig>
            {
                { DirectoryType.PersistentData, new DirectoryConfig(noReadPath, DirectoryPermission.None) }
            };
            var service = new FileService(_logger, _serializer, _streamingAssetsReader, directories);

            var result = await service.ReadAsync<SimpleData>(filePath);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(_logger.HasError("Read access denied"));
        }

        #endregion
    }
}
