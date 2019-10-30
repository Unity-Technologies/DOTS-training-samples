using System;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    static class UIElementHelpers
    {
        public static VisualElement LoadTemplate(string basePath, string uxmlFileName, string ussFileName = null, string uxmlSubDirectoryName = "uxml", string ussSubDirectoryName = "uss", string lightSkinUssSuffix = "light", string darkSkinUssSuffix = "dark")
        {
            return LoadClonableTemplate(basePath, uxmlFileName, ussFileName, uxmlSubDirectoryName, ussSubDirectoryName, lightSkinUssSuffix, darkSkinUssSuffix).GetNewInstance();
        }

        public static VisualElementTemplate LoadClonableTemplate(string basePath, string uxmlFileName, string ussFileName = null, string uxmlSubDirectoryName = "uxml", string ussSubDirectoryName = "uss", string lightSkinUssSuffix = "light", string darkSkinUssSuffix = "dark")
        {
            return new VisualElementTemplate(basePath, uxmlFileName, ussFileName, uxmlSubDirectoryName, ussSubDirectoryName, lightSkinUssSuffix, darkSkinUssSuffix);
        }

        public struct VisualElementTemplate
        {
            readonly VisualTreeAsset m_template;
            readonly StyleSheet m_baseStyle;
            readonly StyleSheet m_skinStyle;

            public VisualElementTemplate(string basePath, string uxmlFileName, string ussFileName, string uxmlSubDirectoryName, string ussSubDirectoryName, string lightSkinUssSuffix, string darkSkinUssSuffix)
            {
                if (string.IsNullOrEmpty(basePath))
                    throw new ArgumentNullException(basePath);
                if (string.IsNullOrEmpty(uxmlFileName))
                    throw new ArgumentNullException(uxmlFileName);
                if (string.IsNullOrEmpty(lightSkinUssSuffix))
                    throw new ArgumentNullException(lightSkinUssSuffix);
                if (string.IsNullOrEmpty(darkSkinUssSuffix))
                    throw new ArgumentNullException(darkSkinUssSuffix);

                if (Path.HasExtension(uxmlFileName) && Path.GetExtension(uxmlFileName) == "uxml")
                    uxmlFileName = Path.GetFileNameWithoutExtension(uxmlFileName);

                var templatePath = Path.Combine(basePath, uxmlSubDirectoryName, uxmlFileName + ".uxml");
                m_template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(templatePath);
                if (m_template == null)
                    throw new ArgumentException("No UXML template found at location " + templatePath);

                var ussName = ussFileName ?? uxmlFileName;
                m_baseStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(basePath, ussSubDirectoryName, $"{ussName}.uss"));
                var skinStylePath = EditorGUIUtility.isProSkin ? Path.Combine(basePath, ussSubDirectoryName, $"{ussName}_{darkSkinUssSuffix}.uss") : Path.Combine(basePath, ussSubDirectoryName, $"{ussName}_{lightSkinUssSuffix}.uss");
                m_skinStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(skinStylePath);
            }

            public VisualElement GetNewInstance()
            {
                var visualElement = new VisualElement();
                m_template.CloneTree(visualElement);
                if (m_baseStyle != null)
                    visualElement.styleSheets.Add(m_baseStyle);
                if (m_skinStyle != null)
                    visualElement.styleSheets.Add(m_skinStyle);

                return visualElement;
            }
        }

        public static class Style
        {
            public static string GetUssClass(BuildTarget buildTarget)
            {
                switch (buildTarget)
                {
                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.WSAPlayer:
                    case BuildTarget.StandaloneWindows:
                        return "live-link-build-settings-dropdown__item__device-windows";
                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.StandaloneOSX:
                        return "live-link-build-settings-dropdown__item__device-standalone";
                    case BuildTarget.XboxOne:
                        return "live-link-build-settings-dropdown__item__device-xboxOne";
                    case BuildTarget.iOS:
                        return "live-link-build-settings-dropdown__item__device-iOS";
                    case BuildTarget.Android:
                        return "live-link-build-settings-dropdown__item__device-android";
                    case BuildTarget.WebGL:
                        return "live-link-build-settings-dropdown__item__device-webGL";
                    case BuildTarget.PS4:
                        return "live-link-build-settings-dropdown__item__device-ps4";
                    case BuildTarget.tvOS:
                        return "live-link-build-settings-dropdown__item__device-tvOS";
                    case BuildTarget.Switch:
                        return "live-link-build-settings-dropdown__item__device-switch";
                    case BuildTarget.Lumin:
                        return "live-link-build-settings-dropdown__item__device-lumin";
                    case BuildTarget.Stadia:
                        return "live-link-build-settings-dropdown__item__device-stadia";
                }

                return null;
            }
        }
    }
}
