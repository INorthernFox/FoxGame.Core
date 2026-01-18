using System.Collections.Generic;
using Core.FileEditor.Tests.EditMode.Mocks;
using NUnit.Framework;

namespace Core.FileEditor.Tests.EditMode
{
    [TestFixture]
    public sealed class FileServiceFactoryTests
    {
        private MockGameLogger _logger;
        private MockFileSerializer _serializer;
        private MockStreamingAssetsReader _streamingAssetsReader;
        private FileServiceFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _logger = new MockGameLogger();
            _serializer = new MockFileSerializer();
            _streamingAssetsReader = new MockStreamingAssetsReader();
            _factory = new FileServiceFactory(_logger, _serializer, _streamingAssetsReader);
        }

        [Test]
        public void Create_ReturnsFileServiceInstance()
        {
            var directories = new Dictionary<DirectoryType, DirectoryConfig>
            {
                { DirectoryType.PersistentData, new DirectoryConfig("/persistent", DirectoryPermission.ReadWrite) }
            };

            var result = _factory.Create(directories);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<FileService>(result);
        }

        [Test]
        public void Create_WithEmptyDirectories_ReturnsFileService()
        {
            var directories = new Dictionary<DirectoryType, DirectoryConfig>();

            var result = _factory.Create(directories);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Create_MultipleDirectories_ReturnsConfiguredFileService()
        {
            var directories = new Dictionary<DirectoryType, DirectoryConfig>
            {
                { DirectoryType.StreamingAssets, new DirectoryConfig("/streaming", DirectoryPermission.ReadOnly) },
                { DirectoryType.PersistentData, new DirectoryConfig("/persistent", DirectoryPermission.ReadWrite) }
            };

            var result = _factory.Create(directories);

            Assert.IsNotNull(result);
            Assert.AreEqual("/streaming", result.GetBasePath(DirectoryType.StreamingAssets));
            Assert.AreEqual("/persistent", result.GetBasePath(DirectoryType.PersistentData));
        }
    }
}
