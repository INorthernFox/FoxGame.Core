using Core.FileEditor;

namespace Core.ConfigProviders.GeneralConfigs
{
    public class GeneralConfigsProvider : BaseConfigsProvider<GeneralConfig >
    {
        public GeneralConfigsProvider(IFileService fileService) : base(fileService)
        {
        }
        
        public override string DefaultFileName => "general_config.json";

        protected override string AddToPathPath(string path) =>
            path;
    }
}