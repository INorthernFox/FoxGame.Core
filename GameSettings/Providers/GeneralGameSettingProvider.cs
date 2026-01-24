using System.Threading.Tasks;
using Core.FileEditor;
using Core.GameConfigs;
using Core.Loggers;
using FluentResults;

namespace Core.GameSettings.Providers
{
    public class GeneralGameSettingProvider : BaseSettingsProvider<GeneralGameSettings>
    {
        private const string GeneralGameSettingsDefaultName = "general_game_settings.json";
        
        public GeneralGameSettingProvider(IFileService fileService, IGameLogger logger)
            : base(fileService, logger)
        {
        }

        public async Task<Result<GeneralGameSettings>> LoadDefault() =>
            await Load(GeneralGameSettingsDefaultName);

        protected override bool IsWritableSettings => false;

        protected override string GetCustomPath(string fileName) =>
            fileName;
    }
}