using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Controls
{
    internal class PaginationField
    {
        private int m_Count = 0;

        private static GUIStyle s_ToolbarButtonStyle;
        private const float k_ButtonSize = 20.0f;
        private const float k_PageSize = 50.0f;

        public int ItemsPerPage { get; set; }

        public int Page
        {
            get => m_Page;
            set => m_Page = Mathf.Clamp(value, 0, LastPage);
        }

        private int m_Page;

        public int Count
        {
            get => m_Count;
            set
            {
                m_Count = value;
                Page = Mathf.Min(Page, LastPage);
            }
        }

        public int LastPage => (Count - 1) / ItemsPerPage;


        public void OnGUI()
        {
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            var enabled = GUI.enabled;

            EditorGUILayout.BeginHorizontal();
            try
            {
                EditorGUI.indentLevel = 0;
                GUI.enabled = Count > 0;

                InitStyles();

                GUILayout.FlexibleSpace();

                var pageLabel = new GUIContent(L10n.Tr("Page"));
                var pageContLabel = new GUIContent($"{Page + 1} of {LastPage + 1}");

                var pageLabelWidth = EditorStyles.label.CalcSize(pageLabel).x + 2.0f;

                if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.FirstKey"), s_ToolbarButtonStyle, GUILayout.Width(k_ButtonSize)))
                {
                    Page = 0;
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.PrevKey"), s_ToolbarButtonStyle, GUILayout.Width(k_ButtonSize)))
                {
                    Page -= 1;
                }

                EditorGUIUtility.labelWidth = pageLabelWidth;
                Page = EditorGUILayout.IntField(pageLabel, Page + 1) - 1;

                if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.NextKey"), s_ToolbarButtonStyle, GUILayout.Width(k_ButtonSize)))
                {
                    Page += 1;
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.LastKey"), s_ToolbarButtonStyle, GUILayout.Width(k_ButtonSize)))
                {
                    Page = LastPage;
                }

                GUILayout.Label(pageContLabel);
            }
            finally
            {
                // Restore global state
                EditorGUI.indentLevel = indent;
                EditorGUIUtility.labelWidth = labelWidth;
                GUI.enabled = enabled;
                EditorGUILayout.EndHorizontal();
            }
        }

        public void OnGUI(Rect rect)
        {
            // Cache global state
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            var enabled = GUI.enabled;

            try
            {
                EditorGUI.indentLevel = 0;
                GUI.enabled = Count > 0;

                InitStyles();

                // We want to show the pagination at the right-most, so we will pre-calculate the total width we need and
                // offset content accordingly.
                var width = rect.width;

                var pageLabel = new GUIContent(L10n.Tr("Page"));
                var pageContLabel = new GUIContent($"{Page + 1} of {LastPage + 1}");
                var pageCountWidth = EditorStyles.label.CalcSize(pageContLabel).x + 4.0f;

                var pageLabelWidth = EditorStyles.label.CalcSize(pageLabel).x + 2.0f;

                var usedWidth = 4 * k_ButtonSize
                    + pageLabelWidth
                    + k_PageSize
                    + pageCountWidth;

                rect.x += width - usedWidth;
                rect.width = usedWidth;

                rect.width = k_ButtonSize;
                if (GUI.Button(rect, EditorGUIUtility.IconContent("Animation.FirstKey"), s_ToolbarButtonStyle))
                {
                    Page = 0;
                }

                rect.x += rect.width;

                if (GUI.Button(rect, EditorGUIUtility.IconContent("Animation.PrevKey"), s_ToolbarButtonStyle))
                {
                    Page -= 1;
                }

                rect.x += rect.width;
                rect.width = pageLabelWidth + k_PageSize;

                EditorGUIUtility.labelWidth = pageLabelWidth;
                Page = EditorGUI.IntField(rect, pageLabel, Page + 1) - 1;
                rect.x += rect.width + 2.0f;

                rect.width = k_ButtonSize;

                if (GUI.Button(rect, EditorGUIUtility.IconContent("Animation.NextKey"), s_ToolbarButtonStyle))
                {
                    Page += 1;
                }

                rect.x += rect.width;
                if (GUI.Button(rect, EditorGUIUtility.IconContent("Animation.LastKey"), s_ToolbarButtonStyle))
                {
                    Page = LastPage;
                }

                rect.x += rect.width;
                var remaining = rect;
                remaining.width = pageCountWidth;

                GUI.Label(remaining, pageContLabel);
            }
            finally
            {
                // Restore global state
                EditorGUI.indentLevel = indent;
                EditorGUIUtility.labelWidth = labelWidth;
                GUI.enabled = enabled;
            }
        }

        private static void InitStyles()
        {
            if (null == s_ToolbarButtonStyle)
            {
                s_ToolbarButtonStyle = new GUIStyle("toolbarbutton")
                {
                    stretchWidth = true,
                    fixedHeight = 20,
                    border = new RectOffset(),
                    padding = new RectOffset()
                };
            }
        }
    }
}
