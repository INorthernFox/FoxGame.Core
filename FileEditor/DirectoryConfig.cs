namespace Core.FileEditor
{
    public sealed class DirectoryConfig
    {
        public string Path { get; }
        public DirectoryPermission Permission { get; }

        public DirectoryConfig(string path, DirectoryPermission permission)
        {
            Path = path;
            Permission = permission;
        }

        public bool CanRead => Permission == DirectoryPermission.ReadOnly || Permission == DirectoryPermission.ReadWrite;
        public bool CanWrite => Permission == DirectoryPermission.ReadWrite;
    }
}
