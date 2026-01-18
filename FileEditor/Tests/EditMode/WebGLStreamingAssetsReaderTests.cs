#if UNITY_WEBGL
using Core.FileEditor.Readers;
using NUnit.Framework;

namespace Core.FileEditor.Tests.EditMode
{
    [TestFixture]
    public sealed class WebGLStreamingAssetsReaderTests
    {
        [Test]
        public void WebGLStreamingAssetsReader_ImplementsInterface()
        {
            var reader = new WebGLStreamingAssetsReader();

            Assert.IsInstanceOf<IStreamingAssetsReader>(reader);
        }
    }
}
#endif
