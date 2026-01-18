using System.Collections.Generic;
using Core.FileEditor.Readers;
using Core.FileEditor.Serialization;
using Core.Loggers;

namespace Core.FileEditor
{
    public sealed class FileServiceFactory
    {
        private readonly IGameLogger _logger;
        private readonly IFileSerializer _serializer;
        private readonly IStreamingAssetsReader _streamingAssetsReader;

        public FileServiceFactory(
            IGameLogger logger,
            IFileSerializer serializer,
            IStreamingAssetsReader streamingAssetsReader)
        {
            _logger = logger;
            _serializer = serializer;
            _streamingAssetsReader = streamingAssetsReader;
        }

        public IFileService Create(IReadOnlyDictionary<DirectoryType, DirectoryConfig> directories)
        {
            return new FileService(_logger, _serializer, _streamingAssetsReader, directories);
        }
    }
}
