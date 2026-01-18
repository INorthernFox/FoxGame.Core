using NUnit.Framework;

namespace Core.FileEditor.Tests.EditMode
{
    [TestFixture]
    public sealed class DirectoryConfigTests
    {
        [Test]
        public void Constructor_SetsPathCorrectly()
        {
            const string expectedPath = "/test/path";
            var config = new DirectoryConfig(expectedPath, DirectoryPermission.ReadOnly);

            Assert.AreEqual(expectedPath, config.Path);
        }

        [Test]
        public void Constructor_SetsPermissionCorrectly()
        {
            var config = new DirectoryConfig("/test", DirectoryPermission.ReadWrite);

            Assert.AreEqual(DirectoryPermission.ReadWrite, config.Permission);
        }

        [Test]
        public void CanRead_ReadOnlyPermission_ReturnsTrue()
        {
            var config = new DirectoryConfig("/test", DirectoryPermission.ReadOnly);

            Assert.IsTrue(config.CanRead);
        }

        [Test]
        public void CanRead_ReadWritePermission_ReturnsTrue()
        {
            var config = new DirectoryConfig("/test", DirectoryPermission.ReadWrite);

            Assert.IsTrue(config.CanRead);
        }

        [Test]
        public void CanRead_NonePermission_ReturnsFalse()
        {
            var config = new DirectoryConfig("/test", DirectoryPermission.None);

            Assert.IsFalse(config.CanRead);
        }

        [Test]
        public void CanWrite_ReadWritePermission_ReturnsTrue()
        {
            var config = new DirectoryConfig("/test", DirectoryPermission.ReadWrite);

            Assert.IsTrue(config.CanWrite);
        }

        [Test]
        public void CanWrite_ReadOnlyPermission_ReturnsFalse()
        {
            var config = new DirectoryConfig("/test", DirectoryPermission.ReadOnly);

            Assert.IsFalse(config.CanWrite);
        }
    }
}
