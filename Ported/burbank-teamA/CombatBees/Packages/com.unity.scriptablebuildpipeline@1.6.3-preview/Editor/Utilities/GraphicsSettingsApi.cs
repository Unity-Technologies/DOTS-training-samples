using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Utilities
{
#if !UNITY_2019_1_OR_NEWER
    using System.Reflection;
    using UnityEngine.Rendering;

    using Object = UnityEngine.Object;
#endif

    static class GraphicsSettingsApi
    {
#if UNITY_2019_1_OR_NEWER
        internal static BuildUsageTagGlobal GetGlobalUsage()
        {
            return ContentBuildInterface.GetGlobalUsageFromGraphicsSettings();
        }
#else
        static SerializedObject m_SerializedObject;
        static SerializedProperty m_LightmapStripping;
        static SerializedProperty m_LightmapKeepPlain;
        static SerializedProperty m_LightmapKeepDirCombined;
        static SerializedProperty m_LightmapKeepDynamicPlain;
        static SerializedProperty m_LightmapKeepDynamicDirCombined;
        static SerializedProperty m_LightmapKeepShadowMask;
        static SerializedProperty m_LightmapKeepSubtractive;
        static SerializedProperty m_FogStripping;
        static SerializedProperty m_FogKeepLinear;
        static SerializedProperty m_FogKeepExp;
        static SerializedProperty m_FogKeepExp2;
        static SerializedProperty m_InstancingStripping;

        static FieldInfo m_LightmapModesUsed;
        static FieldInfo m_LegacyLightmapModesUsed;
        static FieldInfo m_DynamicLightmapsUsed;
        static FieldInfo m_FogModesUsed;
        static FieldInfo m_ForceInstancingStrip;
        static FieldInfo m_ForceInstancingKeep;
        static FieldInfo m_ShadowMasksUsed;
        static FieldInfo m_SubtractiveUsed;

        static uint m_LightmapModesUsed_Value;
        static uint m_LegacyLightmapModesUsed_Value;
        static uint m_DynamicLightmapsUsed_Value;
        static uint m_FogModesUsed_Value;
        static bool m_ForceInstancingStrip_Value;
        static bool m_ForceInstancingKeep_Value;
        static bool m_ShadowMasksUsed_Value;
        static bool m_SubtractiveUsed_Value;

        static void OnEnable()
        {
            m_LightmapModesUsed_Value = 0;
            m_LegacyLightmapModesUsed_Value = 0;
            m_DynamicLightmapsUsed_Value = 0;
            m_FogModesUsed_Value = 0;
            m_ForceInstancingStrip_Value = false;
            m_ForceInstancingKeep_Value = false;
            m_ShadowMasksUsed_Value = false;
            m_SubtractiveUsed_Value = false;

            if (m_SerializedObject != null)
                return;

            var getGraphicsSettings = typeof(GraphicsSettings).GetMethod("GetGraphicsSettings", BindingFlags.Static | BindingFlags.NonPublic);
            var graphicsSettings = getGraphicsSettings.Invoke(null, null) as Object;
            m_SerializedObject = new SerializedObject(graphicsSettings);

            m_LightmapStripping = m_SerializedObject.FindProperty("m_LightmapStripping");
            m_LightmapKeepPlain = m_SerializedObject.FindProperty("m_LightmapKeepPlain");
            m_LightmapKeepDirCombined = m_SerializedObject.FindProperty("m_LightmapKeepDirCombined");
            m_LightmapKeepDynamicPlain = m_SerializedObject.FindProperty("m_LightmapKeepDynamicPlain");
            m_LightmapKeepDynamicDirCombined = m_SerializedObject.FindProperty("m_LightmapKeepDynamicDirCombined");
            m_LightmapKeepShadowMask = m_SerializedObject.FindProperty("m_LightmapKeepShadowMask");
            m_LightmapKeepSubtractive = m_SerializedObject.FindProperty("m_LightmapKeepSubtractive");
            m_FogStripping = m_SerializedObject.FindProperty("m_FogStripping");
            m_FogKeepLinear = m_SerializedObject.FindProperty("m_FogKeepLinear");
            m_FogKeepExp = m_SerializedObject.FindProperty("m_FogKeepExp");
            m_FogKeepExp2 = m_SerializedObject.FindProperty("m_FogKeepExp2");
            m_InstancingStripping = m_SerializedObject.FindProperty("m_InstancingStripping");

            var globalUsageType = typeof(BuildUsageTagGlobal);
            m_LightmapModesUsed = globalUsageType.GetField("m_LightmapModesUsed", BindingFlags.Instance | BindingFlags.NonPublic);
            m_LegacyLightmapModesUsed = globalUsageType.GetField("m_LegacyLightmapModesUsed", BindingFlags.Instance | BindingFlags.NonPublic);
            m_DynamicLightmapsUsed = globalUsageType.GetField("m_DynamicLightmapsUsed", BindingFlags.Instance | BindingFlags.NonPublic);
            m_FogModesUsed = globalUsageType.GetField("m_FogModesUsed", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ForceInstancingStrip = globalUsageType.GetField("m_ForceInstancingStrip", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ForceInstancingKeep = globalUsageType.GetField("m_ForceInstancingKeep", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ShadowMasksUsed = globalUsageType.GetField("m_ShadowMasksUsed", BindingFlags.Instance | BindingFlags.NonPublic);
            m_SubtractiveUsed = globalUsageType.GetField("m_SubtractiveUsed", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        internal static BuildUsageTagGlobal GetGlobalUsage()
        {
            OnEnable();
            m_SerializedObject.Update();

            if (m_LightmapStripping.intValue != 0)
            {
                if (m_LightmapKeepPlain.boolValue)
                    m_LegacyLightmapModesUsed_Value |= (1 << 0);
                if (m_LightmapKeepDirCombined.boolValue)
                    m_LegacyLightmapModesUsed_Value |= (1 << 2);

                if (m_LightmapKeepPlain.boolValue)
                    m_LightmapModesUsed_Value |= (1 << 0);
                if (m_LightmapKeepDirCombined.boolValue)
                    m_LightmapModesUsed_Value |= (1 << 1);

                if (m_LightmapKeepDynamicPlain.boolValue)
                    m_DynamicLightmapsUsed_Value |= (1 << 0);
                if (m_LightmapKeepDynamicDirCombined.boolValue)
                    m_DynamicLightmapsUsed_Value |= (1 << 1);

                if (m_LightmapKeepShadowMask.boolValue)
                    m_ShadowMasksUsed_Value = true;
                if (m_LightmapKeepSubtractive.boolValue)
                    m_SubtractiveUsed_Value = true;
            }

            if (m_FogStripping.intValue != 0)
            {
                if (m_FogKeepLinear.boolValue)
                    m_FogModesUsed_Value |= (1 << 1);
                if (m_FogKeepExp.boolValue)
                    m_FogModesUsed_Value |= (1 << 2);
                if (m_FogKeepExp2.boolValue)
                    m_FogModesUsed_Value |= (1 << 3);
            }

            m_ForceInstancingStrip_Value = (m_InstancingStripping.intValue == 1);
            m_ForceInstancingKeep_Value = (m_InstancingStripping.intValue == 2);

            BuildUsageTagGlobal globalUsage = new BuildUsageTagGlobal();
            var boxedUsage = (object)globalUsage;
            m_LightmapModesUsed.SetValue(boxedUsage, m_LightmapModesUsed_Value);
            m_LegacyLightmapModesUsed.SetValue(boxedUsage, m_LegacyLightmapModesUsed_Value);
            m_DynamicLightmapsUsed.SetValue(boxedUsage, m_DynamicLightmapsUsed_Value);
            m_FogModesUsed.SetValue(boxedUsage, m_FogModesUsed_Value);
            m_ForceInstancingStrip.SetValue(boxedUsage, m_ForceInstancingStrip_Value);
            m_ForceInstancingKeep.SetValue(boxedUsage, m_ForceInstancingKeep_Value);
            m_ShadowMasksUsed.SetValue(boxedUsage, m_ShadowMasksUsed_Value);
            m_SubtractiveUsed.SetValue(boxedUsage, m_SubtractiveUsed_Value);
            globalUsage = (BuildUsageTagGlobal)boxedUsage;
            return globalUsage;
        }
#endif
    }
}
