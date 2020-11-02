using Unity.Editor.Bridge;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using GUIUtility = UnityEngine.GUIUtility;

namespace Unity.Editor.Legacy
{
    static class CustomEditorGUILayout
    {
        static readonly GUIContent s_Text = new GUIContent();

        internal static GUIContent TempContent(string t)
        {
            s_Text.image = null;
            s_Text.text = t;
            s_Text.tooltip = null;
            return s_Text;
        }

        public static void Vector2Label(
            string label,
            Vector2 value,
            params GUILayoutOption[] options)
        {
            Vector2Label(TempContent(label), value, new bool2(), options);
        }

        public static void Vector2Label(
            string label,
            Vector2 value,
            bool2 mixed,
            params GUILayoutOption[] options)
        {
            Vector2Label(TempContent(label), value, mixed, options);
        }

        static void Vector2Label(
            GUIContent label,
            Vector2 value,
            bool2 mixed,
            params GUILayoutOption[] options)
        {
            CustomEditorGUI.Vector2Label(EditorGUILayoutBridge.s_LastRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label), EditorStyles.numberField, options), label, value, mixed);
        }

        public static void Vector3Label(
            string label,
            Vector3 value,
            params GUILayoutOption[] options)
        {
            Vector3Label(TempContent(label), value, new bool3(), options);
        }

        public static void Vector3Label(
            string label,
            Vector3 value,
            bool3 mixed,
            params GUILayoutOption[] options)
        {
            Vector3Label(TempContent(label), value, mixed, options);
        }

        static void Vector3Label(
            GUIContent label,
            Vector3 value,
            bool3 mixed,
            params GUILayoutOption[] options)
        {
            CustomEditorGUI.Vector3Label(EditorGUILayoutBridge.s_LastRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label), EditorStyles.numberField, options), label, value, mixed);
        }

        public static void Vector4Label(
            string label,
            Vector4 value,
            params GUILayoutOption[] options)
        {
            Vector4Label(TempContent(label), value, new bool4(), options);
        }

        public static void Vector4Label(
            string label,
            Vector4 value,
            bool4 mixed,
            params GUILayoutOption[] options)
        {
            Vector4Label(TempContent(label), value, mixed, options);
        }

        public static void Vector4Label(
            GUIContent label,
            Vector4 value,
            bool4 mixed,
            params GUILayoutOption[] options)
        {
            CustomEditorGUI.Vector4Label(EditorGUILayoutBridge.s_LastRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label), EditorStyles.numberField, options), label, value, mixed);
        }
    }

    static class CustomEditorGUI
    {
        static readonly int s_FoldoutHash = "Foldout".GetHashCode();

        static readonly float[] s_VectorComponents = new float[4];
        static readonly bool[] s_IsMixed = new bool[4];

        static readonly GUIContent[] s_VectorComponentsLabels = new GUIContent[4]
        {
            new GUIContent("X"),
            new GUIContent("Y"),
            new GUIContent("Z"),
            new GUIContent("W")
        };

        static readonly GUIContent s_TempContent1 = new GUIContent();
        static readonly GUIContent s_TempContent2 = new GUIContent();

        static Rect m_Rect;
        static Rect m_LastRect;

        static GUIContent TempContent(string text, Texture image, string tooltip)
        {
            return TempContent(s_TempContent1, text, image, tooltip);
        }

        static GUIContent TempContent2(string text, Texture image, string tooltip)
        {
            return TempContent(s_TempContent2, text, image, tooltip);
        }

        static GUIContent TempContent(GUIContent temp, string text, Texture image, string tooltip)
        {
            temp.text = text;
            temp.image = image;
            temp.tooltip = tooltip;
            return temp;
        }

        public static void Vector2Label(Rect position, GUIContent label, Vector2 value, bool2 mixed)
        {
            var controlId = GUIUtility.GetControlID(s_FoldoutHash, FocusType.Keyboard, position);
            position = MultiFieldPrefixLabel(position, controlId, label, 3);
            position.height = 18f;
            Vector2Label(position, value, mixed);
        }

        static void Vector2Label(Rect position, Vector2 value, bool2 mixed)
        {
            s_VectorComponents[0] = value.x;
            s_VectorComponents[1] = value.y;
            s_IsMixed[0] = mixed.x;
            s_IsMixed[1] = mixed.y;
            position.height = 18f;
            MultiFloatFieldAsLabel(position, s_VectorComponentsLabels, s_VectorComponents, s_IsMixed, 2);
        }

        public static void Vector3Label(Rect position, GUIContent label, Vector3 value, bool3 mixed)
        {
            var controlId = GUIUtility.GetControlID(s_FoldoutHash, FocusType.Keyboard, position);
            position = MultiFieldPrefixLabel(position, controlId, label, 3);
            position.height = 18f;
            Vector3Label(position, value, mixed);
        }

        static void Vector3Label(Rect position, Vector3 value, bool3 mixed)
        {
            s_VectorComponents[0] = value.x;
            s_VectorComponents[1] = value.y;
            s_VectorComponents[2] = value.z;
            s_IsMixed[0] = mixed.x;
            s_IsMixed[1] = mixed.y;
            s_IsMixed[2] = mixed.z;
            position.height = 18f;
            MultiFloatFieldAsLabel(position, s_VectorComponentsLabels, s_VectorComponents, s_IsMixed, 3);
        }

        public static void Vector4Label(Rect position, GUIContent label, Vector4 value, bool4 mixed)
        {
            var controlId = GUIUtility.GetControlID(s_FoldoutHash, FocusType.Keyboard, position);
            position = MultiFieldPrefixLabel(position, controlId, label, 4);
            position.height = 18f;
            Vector4FieldNoIndent(position, value, mixed);
        }

        static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns)
        {
            if (!LabelHasContent(label))
            {
                return EditorGUI.IndentedRect(totalPosition);
            }

            if (EditorGUIUtility.wideMode)
            {
                var labelPosition = new Rect(totalPosition.x + EditorGUIBridge.indent, totalPosition.y, EditorGUIUtility.labelWidth - EditorGUIBridge.indent, 18f);
                var rect = totalPosition;
                rect.xMin += EditorGUIUtility.labelWidth + 2f;
                if (columns == 2)
                {
                    var num = (float)((rect.width - 8.0) / 3.0);
                    rect.xMax -= num + 4f;
                }

                if (null != label)
                {
                    EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, id);
                }

                return rect;
            }

            var labelPosition1 = new Rect(totalPosition.x + EditorGUIBridge.indent, totalPosition.y, totalPosition.width - EditorGUIBridge.indent, 18f);
            EditorGUI.HandlePrefixLabel(totalPosition, labelPosition1, label, id);

            var rect1 = totalPosition;
            rect1.xMin += EditorGUI.indentLevel + 15f;
            rect1.yMin += 18f;
            return rect1;
        }

        static void Vector4FieldNoIndent(Rect position, Vector4 value, bool4 mixed)
        {
            s_VectorComponents[0] = value.x;
            s_VectorComponents[1] = value.y;
            s_VectorComponents[2] = value.z;
            s_VectorComponents[3] = value.w;
            s_IsMixed[0] = mixed.x;
            s_IsMixed[1] = mixed.y;
            s_IsMixed[2] = mixed.z;
            s_IsMixed[3] = mixed.w;
            position.height = 18f;
            MultiFloatFieldAsLabel(position, s_VectorComponentsLabels, s_VectorComponents, s_IsMixed, 4);
        }

        static void MultiFloatFieldAsLabel(Rect position, GUIContent[] subLabels, float[] values, bool[] mixed, int count)
        {
            var length = count;
            var num = (position.width - (float)(length - 1) * 4f) / (float)length;
            var position1 = new Rect(position)
            {
                width = num
            };
            var labelWidth = EditorGUIUtility.labelWidth;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            for (var index = 0; index < count; ++index)
            {
                EditorGUIUtility.labelWidth = CalcPrefixLabelWidth(subLabels[index], (GUIStyle)null);
                var richText = EditorStyles.label.richText;
                EditorStyles.label.richText = true;
                EditorGUI.LabelField(position1, subLabels[index], mixed[index] ? EditorGUIBridge.mixedValueContent : TempContent($"<b>{values[index]:0.##}</b>", null, string.Empty));
                EditorStyles.label.richText = richText;
                position1.x += num + 4f;
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;
        }

        static float CalcPrefixLabelWidth(GUIContent label, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.label;
            return style.CalcSize(label).x;
        }

        static bool LabelHasContent(GUIContent label)
        {
            if (label == null)
                return true;

            return label.text != string.Empty || label.image != null;
        }
    }
}
