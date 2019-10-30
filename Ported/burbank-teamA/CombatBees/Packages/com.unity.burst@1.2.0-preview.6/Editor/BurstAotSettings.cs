using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;

namespace Unity.Burst.Editor
{
    // To add a setting,
    //  Add a
    //          [SerializeField] internal type settingname;
    //  Add a
    //          internal static readonly string settingname_DisplayName = "Name of option to be displayed in the editor (and searched for)";
    //  Add a
    //          internal static readonly string settingname_ToolTip = "tool tip information to display when hovering mouse

    class BurstPlatformAotSettings : ScriptableObject
    {
        [SerializeField]
        internal bool DisableOptimisations;
        [SerializeField]
        internal bool DisableSafetyChecks;
        [SerializeField]
        internal bool DisableBurstCompilation;

        internal static readonly string DisableOptimisations_DisplayName = "Disable Optimisations";
        internal static readonly string DisableOptimisations_ToolTip = "Disables all optimisations for the currently selected platform.";

        internal static readonly string DisableSafetyChecks_DisplayName = "Disable Safety Checks";
        internal static readonly string DisableSafetyChecks_ToolTip = "Disables safety checks, results in faster runtime, but Out Of Bounds checks etc are not validated.";

        internal static readonly string DisableBurstCompilation_DisplayName = "Disable Burst Compilation";
        internal static readonly string DisableBurstCompilation_ToolTip = "Disables burst compilation for the selected platform.";

        BurstPlatformAotSettings(BuildTarget target)
        {
            InitialiseDefaults(target);
        }
        internal void InitialiseDefaults(BuildTarget target)
        {
            DisableSafetyChecks = true;
            DisableBurstCompilation = false;
            DisableOptimisations = false;
        }

        internal static string GetPath(BuildTarget target)
        {
            return "ProjectSettings/BurstAotSettings_"+target.ToString()+".json";
        }

        internal static BuildTarget ResolveTarget(BuildTarget target)
        {
            // Treat the 32/64 platforms the same from the point of view of burst settings
            // since there is no real distinguishment from the platforms selector
            if (target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneWindows)
                return BuildTarget.StandaloneWindows;

#if UNITY_2019_2_OR_NEWER
            //32 bit linux support was deprecated
            if (target == BuildTarget.StandaloneLinux64)
                return BuildTarget.StandaloneLinux64;
#else
            if (target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinux)
                return BuildTarget.StandaloneLinux;
#endif

            return target;
        }

        internal static BurstPlatformAotSettings GetOrCreateSettings(BuildTarget target)
        {
            target = ResolveTarget(target);
            BurstPlatformAotSettings settings = ScriptableObject.CreateInstance<BurstPlatformAotSettings>();
            settings.InitialiseDefaults(target);
            string path = GetPath(target);

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                EditorJsonUtility.FromJsonOverwrite(json, settings);
            }
            else
            {
                settings.Save(target);
            }

            return settings;
        }

        internal void Save(BuildTarget target)
        {
            target = ResolveTarget(target);
            File.WriteAllText(GetPath(target), EditorJsonUtility.ToJson(this, true));
        }

        internal static SerializedObject GetSerializedSettings(BuildTarget target)
        {
            return new SerializedObject(GetOrCreateSettings(target));
        }
    }

    static class BurstAotSettingsIMGUIRegister
    {
        class BurstAotSettingsProvider : SettingsProvider
        {
            SerializedObject[] m_PlatformSettings;
            SerializedProperty[][] m_PlatformProperties;
            GUIContent[][] m_PlatformToolTips;

            BuildPlatform[] validPlatforms;

            public BurstAotSettingsProvider()
                : base("Project/Burst AOT Settings", SettingsScope.Project, null)
            {
                int a;

                validPlatforms = BuildPlatforms.instance.GetValidPlatforms(true).ToArray();

                var platformFields = typeof(BurstPlatformAotSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                int numPlatformFields = platformFields.Length;
                int numKeywords = numPlatformFields;
                var tempKeywords = new string[numKeywords];

                for (a = 0; a < numPlatformFields; a++)
                {
                    tempKeywords[a] = typeof(BurstPlatformAotSettings).GetField(platformFields[a].Name + "_ToolTip", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as string;
                }

                keywords = new HashSet<string>(tempKeywords);

                m_PlatformSettings = new SerializedObject[validPlatforms.Length];
                m_PlatformProperties = new SerializedProperty[validPlatforms.Length][];
                m_PlatformToolTips=new GUIContent[validPlatforms.Length][];
            }

            public override void OnActivate(string searchContext, VisualElement rootElement)
            {
                var platformFields = typeof(BurstPlatformAotSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                for (int p = 0; p < validPlatforms.Length; p++)
                {
                    m_PlatformSettings[p] = BurstPlatformAotSettings.GetSerializedSettings(validPlatforms[p].defaultTarget);
                    m_PlatformProperties[p]=new SerializedProperty[platformFields.Length];
                    m_PlatformToolTips[p]=new GUIContent[platformFields.Length];
                    for (int i = 0; i < platformFields.Length; i++)
                    {
                        m_PlatformProperties[p][i] = m_PlatformSettings[p].FindProperty(platformFields[i].Name);
                        m_PlatformToolTips[p][i] = EditorGUIUtility.TrTextContent(
                            typeof(BurstPlatformAotSettings).GetField(platformFields[i].Name + "_DisplayName", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as string,
                            typeof(BurstPlatformAotSettings).GetField(platformFields[i].Name + "_ToolTip", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as string);
                    }
                }
            }

            public override void OnGUI(string searchContext)
            {
                EditorGUILayout.BeginVertical();

                int selectedPlatform = EditorGUILayout.BeginPlatformGrouping(validPlatforms, null);

                for (int i = 0; i < m_PlatformProperties[selectedPlatform].Length; i++)
                {
                    EditorGUILayout.PropertyField(m_PlatformProperties[selectedPlatform][i], m_PlatformToolTips[selectedPlatform][i]);
                }

                EditorGUILayout.EndPlatformGrouping();

                EditorGUILayout.EndVertical();

                if (m_PlatformSettings[selectedPlatform].hasModifiedProperties)
                {
                    m_PlatformSettings[selectedPlatform].ApplyModifiedPropertiesWithoutUndo();
                    ((BurstPlatformAotSettings)m_PlatformSettings[selectedPlatform].targetObject).Save(validPlatforms[selectedPlatform].defaultTarget);
                }
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateBurstAotSettingsProvider()
        {
            return new BurstAotSettingsProvider();
        }
    }
}
#else
// Mirror old behaviour
namespace Unity.Burst.Editor
{
    class BurstPlatformAotSettings
    {
        internal bool DisableOptimisations;
        internal bool DisableSafetyChecks;
        internal bool DisableBurstCompilation;

        internal static BurstPlatformAotSettings GetOrCreateSettings(BuildTarget target)
        {
            BurstPlatformAotSettings settings = new BurstPlatformAotSettings();

            settings.DisableOptimisations = false;
            settings.DisableSafetyChecks=!BurstEditorOptions.EnableBurstSafetyChecks;
            settings.DisableBurstCompilation=!BurstEditorOptions.EnableBurstCompilation;

            return settings;
        }
    }
}

#endif
