using System;
using UnityEditor;

namespace Unity.Build.Common
{
    [BuildStep(description = k_Description, category = "Classic")]
    public sealed class BuildStepApplyPlayerSettings : BuildStep
    {
        const string k_Description = "Apply Player Settings";

        public override string Description => k_Description;

        class PlayerSettingsState
        {
            public string Contents { set; get; }
            public bool IsDirty { set; get; }
            public static PlayerSettings Target => AssetDatabase.LoadAssetAtPath<PlayerSettings>("ProjectSettings/ProjectSettings.asset");
        }

        private BuildStepResult FindProperty(SerializedObject serializedObject, string name, out SerializedProperty serializedProperty)
        {
            serializedProperty = serializedObject.FindProperty(name);
            if (serializedProperty == null)
            {
                return Failure($"Failed to find: {name}");
            }
            return Success();
        }

        public override BuildStepResult RunStep(BuildContext context)
        {
            context.Set(new PlayerSettingsState()
            {
                Contents = EditorJsonUtility.ToJson(PlayerSettingsState.Target),
                IsDirty = EditorUtility.GetDirtyCount(PlayerSettingsState.Target) > 0
            });

            var serializedObject = new SerializedObject(PlayerSettingsState.Target);
            var settings = context.BuildSettings;
            var profile = settings.GetComponent<ClassicBuildProfile>();
            var generalSettings = settings.GetComponent<GeneralSettings>();
            var scriptingSettings = settings.GetComponent<ClassicScriptingSettings>();
            var targetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(profile.Target);

            // Get serialized properties for things which don't have API exposed
            SerializedProperty gcIncremental;
            var result = FindProperty(serializedObject, nameof(gcIncremental), out gcIncremental);
            if (result.Failed)
                return result;

            PlayerSettings.productName = generalSettings.ProductName;
            PlayerSettings.companyName = generalSettings.CompanyName;

            // Scripting Settings
            PlayerSettings.SetScriptingBackend(targetGroup, scriptingSettings.ScriptingBackend);
            PlayerSettings.SetIl2CppCompilerConfiguration(targetGroup, scriptingSettings.Il2CppCompilerConfiguration);
            gcIncremental.boolValue = scriptingSettings.UseIncrementalGC;

            switch (profile.Target)
            {
                case BuildTarget.Android:
                {
                    var androidSettings = settings.GetComponent<AndroidSettings>();
                    AndroidBuildType androidBuildType;
                    switch (profile.Configuration)
                    {
                        case BuildConfiguration.Debug: androidBuildType = AndroidBuildType.Debug; break;
                        case BuildConfiguration.Develop: androidBuildType = AndroidBuildType.Development; break;
                        case BuildConfiguration.Release: androidBuildType = AndroidBuildType.Release; break;
                        default: throw new NotImplementedException("AndroidBuildType");
                    }
                    EditorUserBuildSettings.androidBuildType = androidBuildType;
                    PlayerSettings.Android.targetArchitectures = androidSettings.TargetArchitectures;
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, androidSettings.PackageName);
                }
                break;
            }

            EditorUtility.ClearDirty(PlayerSettingsState.Target);

            return Success();
        }
        public override BuildStepResult CleanupStep(BuildContext context)
        {
            var savedState = context.Get<PlayerSettingsState>();
            // Note: EditorJsonUtility.FromJsonOverwrite doesn't dirty PlayerSettings
            EditorJsonUtility.FromJsonOverwrite(savedState.Contents, PlayerSettingsState.Target);
            if (savedState.IsDirty)
                EditorUtility.SetDirty(PlayerSettingsState.Target);

            return Success();
        }
    }

}
