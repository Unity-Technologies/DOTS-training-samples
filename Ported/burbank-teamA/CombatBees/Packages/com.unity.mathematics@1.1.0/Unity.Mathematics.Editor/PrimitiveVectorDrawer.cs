using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Mathematics.Editor
{
    [CustomPropertyDrawer(typeof(bool2)), CustomPropertyDrawer(typeof(bool3)), CustomPropertyDrawer(typeof(bool4))]
    [CustomPropertyDrawer(typeof(double2)), CustomPropertyDrawer(typeof(double3)), CustomPropertyDrawer(typeof(double4))]
    [CustomPropertyDrawer(typeof(float2)), CustomPropertyDrawer(typeof(float3)), CustomPropertyDrawer(typeof(float4))]
    [CustomPropertyDrawer(typeof(int2)), CustomPropertyDrawer(typeof(int3)), CustomPropertyDrawer(typeof(int4))]
    [CustomPropertyDrawer(typeof(uint2)), CustomPropertyDrawer(typeof(uint3)), CustomPropertyDrawer(typeof(uint4))]
    [CustomPropertyDrawer(typeof(DoNotNormalizeAttribute))]
    class PrimitiveVectorDrawer : PropertyDrawer
    {
        static class Content
        {
            public static readonly string doNotNormalizeCompatibility = L10n.Tr(
                $"{typeof(DoNotNormalizeAttribute).Name} only works with {typeof(quaternion)} and primitive vector types."
            );
            public static readonly string doNotNormalizeTooltip =
                L10n.Tr("This value is not normalized, which may produce unexpected results.");

            public static readonly GUIContent[] labels2 = { new GUIContent("X"), new GUIContent("Y") };
            public static readonly GUIContent[] labels3 = { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
            public static readonly GUIContent[] labels4 = { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z"), new GUIContent("W") };
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (!EditorGUIUtility.wideMode)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var subLabels = Content.labels4;
            var startIter = "x";
            switch (property.type[property.type.Length - 1])
            {
                case '2':
                    subLabels = Content.labels2;
                    break;
                case '3':
                    subLabels = Content.labels3;
                    break;
                case '4':
                    subLabels = Content.labels4;
                    break;
                default:
                {
                    if (property.type == nameof(quaternion))
                        startIter = "value.x";
                    else if (attribute is DoNotNormalizeAttribute)
                    {
                        EditorGUI.HelpBox(EditorGUI.PrefixLabel(position, label), Content.doNotNormalizeCompatibility, MessageType.None);
                        return;
                    }
                    break;
                }
            }

            if (attribute is DoNotNormalizeAttribute && string.IsNullOrEmpty(label.tooltip))
                label.tooltip = Content.doNotNormalizeTooltip;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.MultiPropertyField(position, subLabels, property.FindPropertyRelative(startIter), label);
            EditorGUI.EndProperty();
        }
    }
}