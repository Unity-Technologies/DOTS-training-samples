using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    readonly struct UITemplate
    {
        public static UITemplate Null = default;

        readonly string m_UxmlPath;
        readonly string m_UssPath;

        VisualTreeAsset Template => EditorGUIUtility.Load(m_UxmlPath) as VisualTreeAsset;
        StyleSheet StyleSheet => AssetDatabase.LoadAssetAtPath<StyleSheet>(m_UssPath);

        public UITemplate(string name)
        {
            m_UxmlPath = Resources.UxmlFromName(name);
            m_UssPath = Resources.UssFromName(name);
        }

        /// <summary>
        /// Clones the template into the given root element and applies the style sheets from the template.
        /// </summary>
        /// <param name="root">The element that will serve as the root for cloning the template.</param>
        public VisualElement Clone(VisualElement root = null)
        {
            root = CloneTemplate(root);
            AddStyleSheetSkinVariant(root);
            return root;
        }

        public void AddStyles(VisualElement element)
        {
            AddStyleSheetSkinVariant(element);
        }

        public void RemoveStyles(VisualElement element)
        {
            RemoveStyleSheetSkinVariant(element);
        }

        VisualElement CloneTemplate(VisualElement element)
        {
            if (null == Template)
            {
                return element;
            }

            if (null == element)
            {
                return Template.CloneTree();
            }

            Template.CloneTree(element);
            return element;
        }

        void AddStyleSheetSkinVariant(VisualElement element)
        {
            if (null == StyleSheet)
            {
                return;
            }

            if (null == element)
            {
                return;
            }

            element.styleSheets.Add(StyleSheet);
            var assetPath = AssetDatabase.GetAssetPath(StyleSheet);
            assetPath = assetPath.Insert(assetPath.LastIndexOf('.'), Resources.SkinSuffix);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath) is var skin && null != skin)
            {
                element.styleSheets.Add(skin);
            }
        }

        void RemoveStyleSheetSkinVariant(VisualElement element)
        {
            if (null == StyleSheet)
            {
                return;
            }

            if (null == element)
            {
                return;
            }

            element.styleSheets.Remove(StyleSheet);
            var assetPath = AssetDatabase.GetAssetPath(StyleSheet);
            assetPath = assetPath.Insert(assetPath.LastIndexOf('.'), Resources.SkinSuffix);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath) is var skin && null != skin)
            {
                element.styleSheets.Remove(skin);
            }
        }
    }
}
