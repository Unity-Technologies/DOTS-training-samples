using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Build
{
    static class StylesUtility
    {
        const string k_BasePath = "Packages/com.unity.entities/Unity.Build/GUI/uss";

        public static void AddStyleSheetAndVariant(this VisualElement ve, string styleSheetName)
        {
            ve.styleSheets.Add(EditorGUIUtility.Load($"{k_BasePath}/{styleSheetName}.uss") as StyleSheet);
            ve.styleSheets.Add(EditorGUIUtility.Load($"{k_BasePath}/{styleSheetName}_{(EditorGUIUtility.isProSkin? "dark":"light")}.uss") as StyleSheet);
        }
    }
}