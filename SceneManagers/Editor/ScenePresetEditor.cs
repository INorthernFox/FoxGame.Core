#if UNITY_EDITOR
using System.Linq;
using Core.SceneManagers.Data;
using UnityEditor;
using UnityEngine;

namespace Core.SceneManagers.Editor
{
    [CustomEditor(typeof(ScenePreset))]
    public sealed class ScenePresetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(8);

            if(GUILayout.Button("Apply Scenes To Build Settings (by Order)"))
            {
                ApplyToBuildSettings((ScenePreset) target);
            }
        }

        private static void ApplyToBuildSettings(ScenePreset preset)
        {
            if(preset == null) 
                return;

            SceneData[] scenes = preset.Scenes
                .OrderBy(s => s.Order)
                .ToArray();

            int[] duplicateOrders = scenes
                .GroupBy(s => s.Order)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            if(duplicateOrders.Length > 0)
            {
                Debug.LogError(
                    $"ScenePreset '{preset.name}': duplicated Order values: {string.Join(", ", duplicateOrders)}. Fix orders first.",
                    preset);
                return;
            }

            SceneData[] missing = scenes.Where(s => s.Scene == null).ToArray();
            if(missing.Length > 0)
            {
                Debug.LogError(
                    $"ScenePreset '{preset.name}': some Scene fields are empty. Fill them before applying.",
                    preset);
                return;
            }

            EditorBuildSettingsScene[] buildScenes = scenes
                .Select(s =>
                {
                    string path = AssetDatabase.GetAssetPath(s.Scene);
                    return new EditorBuildSettingsScene(path, true);
                })
                .ToArray();

            EditorBuildSettings.scenes = buildScenes;

            EditorUtility.SetDirty(preset);
            AssetDatabase.SaveAssets();

            Debug.Log($"ScenePreset '{preset.name}': applied {buildScenes.Length} scenes to Build Settings.");
        }
    }
}
#endif