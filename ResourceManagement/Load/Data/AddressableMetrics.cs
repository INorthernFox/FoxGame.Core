using System.Collections.Generic;
using System.Text;
using Core.SceneManagers.Data;

namespace Core.ResourceManagement.Load.Data
{
    public readonly struct AddressableMetrics
    {
        public int TotalHandles { get; }
        public int TotalReferences { get; }
        public IReadOnlyDictionary<SceneType, int> HandlesByScene { get; }

        public AddressableMetrics(
            int totalHandles,
            int totalReferences,
            Dictionary<SceneType, int> handlesByScene)
        {
            TotalHandles = totalHandles;
            TotalReferences = totalReferences;
            HandlesByScene = handlesByScene;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Addressable Metrics:");
            sb.AppendLine($"  Total Handles: {TotalHandles}");
            sb.AppendLine($"  Total References: {TotalReferences}");
            sb.AppendLine($"  By Scene:");

            foreach (var kvp in HandlesByScene)
            {
                sb.AppendLine($"    {kvp.Key}: {kvp.Value} handles");
            }

            return sb.ToString();
        }
    }
}
