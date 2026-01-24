using System.Collections.Generic;
using System.IO;
using Core.FileEditor;
using Core.FileEditor.Readers;
using Core.FileEditor.Serialization;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class FileEditorInstaller : MonoBehaviour
    {
        public void InstallBindings(DiContainer container)
        {
            BindSerializer(container);
            BindStreamingAssetsReader(container);
            BindFactory(container);
            BindFileService(container);
        }

        private void BindSerializer(DiContainer container)
        {
            container.BindInterfacesTo<JsonFileSerializer>().AsSingle();
        }

        private void BindStreamingAssetsReader(DiContainer container)
        {
            // Android: APK is a ZIP archive, requires UnityWebRequest
#if UNITY_ANDROID && !UNITY_EDITOR
            container.BindInterfacesTo<AndroidStreamingAssetsReader>().AsSingle();

            // WebGL: No file system access, requires UnityWebRequest
#elif UNITY_WEBGL && !UNITY_EDITOR
            container.BindInterfacesTo<WebGLStreamingAssetsReader>().AsSingle();

            // Windows, Mac, Linux, iOS, Editor: Direct file system access
#else
            container.BindInterfacesTo<StandardStreamingAssetsReader>().AsSingle();
#endif
        }

        private void BindFactory(DiContainer container)
        {
            container.Bind<FileServiceFactory>().AsSingle();
        }

        private void BindFileService(DiContainer container)
        {
            container.Bind<IFileService>().FromMethod(CreateFileService).AsSingle();
        }

        private IFileService CreateFileService(InjectContext context)
        {
            var factory = context.Container.Resolve<FileServiceFactory>();
            var directories = CreateDirectories();
            return factory.Create(directories);
        }

        private IReadOnlyDictionary<DirectoryType, DirectoryConfig> CreateDirectories()
        {
            return new Dictionary<DirectoryType, DirectoryConfig>
            {
                {
                    DirectoryType.StreamingAssets,
                    new DirectoryConfig(Application.streamingAssetsPath, DirectoryPermission.ReadOnly)
                },
                {
                    DirectoryType.PersistentData,
                    new DirectoryConfig(GetPersistentDataPath(), GetPersistentDataPermission())
                }
            };
        }

        private string GetPersistentDataPath()
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "dev_files");
#else
            return Application.persistentDataPath;
#endif
        }

        private DirectoryPermission GetPersistentDataPermission()
        {
            // WebGL has no persistent file system - read-only mode
#if UNITY_WEBGL && !UNITY_EDITOR
            return DirectoryPermission.ReadOnly;
#else
            return DirectoryPermission.ReadWrite;
#endif
        }
    }
}