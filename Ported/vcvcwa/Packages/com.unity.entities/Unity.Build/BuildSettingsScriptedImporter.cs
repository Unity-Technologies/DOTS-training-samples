using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Unity.Build
{
    [ScriptedImporter(1, new[] { BuildSettingsExtension })]
    class BuildSettingsScriptedImporter : ScriptedImporter
    {
        internal const string BuildSettingsExtension = "buildsettings";

        public override void OnImportAsset(AssetImportContext context)
        {
            var buildSettings = ScriptableObject.CreateInstance<BuildSettings>();
            BuildSettings.DeserializeFromPath(buildSettings, context.assetPath);
            context.AddObjectToAsset("asset", buildSettings/*, icon*/);
            context.SetMainObject(buildSettings);
        }
    }
}
