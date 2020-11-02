using Unity.Entities;
using Unity.Mathematics;
using Unity.Properties;
using Unity.Properties.Adapters;
using UnityEditor;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer :
        IVisit<Hash128>
        , IVisit<quaternion>
        , IVisit<float2>
        , IVisit<float3>
        , IVisit<float4>
        , IVisit<float2x2>
        , IVisit<float3x3>
        , IVisit<float4x4>
    {
        public VisitStatus Visit<TContainer>(Property<TContainer, Hash128> property, ref TContainer container, ref Hash128 value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, quaternion> property, ref TContainer container, ref quaternion value)
        {
            using (MakePathScope(property))
            {
                CustomEditorGUILayout.Vector4Label(GetDisplayName(property), value.value, IsMixedValue("value", value.value));
            }
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float2> property, ref TContainer container, ref float2 value)
        {
            CustomEditorGUILayout.Vector2Label(GetDisplayName(property), value, IsMixedValue(property.Name, value));
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float3> property, ref TContainer container, ref float3 value)
        {
            CustomEditorGUILayout.Vector3Label(GetDisplayName(property), value, IsMixedValue(property.Name, value));
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float4> property, ref TContainer container, ref float4 value)
        {
            CustomEditorGUILayout.Vector4Label(GetDisplayName(property), value, IsMixedValue(property.Name, value));
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float2x2> property, ref TContainer container, ref float2x2 value)
        {
            using (MakePathScope(property))
            {
                CustomEditorGUILayout.Vector2Label(GetDisplayName(property), value.c0, IsMixedValue("c0", value.c0));
                CustomEditorGUILayout.Vector2Label(GetEmptyNameForRow(), value.c1, IsMixedValue("c1", value.c1));
            }

            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float3x3> property, ref TContainer container, ref float3x3 value)
        {
            using (MakePathScope(property))
            {
                CustomEditorGUILayout.Vector3Label(GetDisplayName(property), value.c0, IsMixedValue("c0", value.c0));
                CustomEditorGUILayout.Vector3Label(GetEmptyNameForRow(), value.c1, IsMixedValue("c1", value.c1));
                CustomEditorGUILayout.Vector3Label(GetEmptyNameForRow(), value.c2, IsMixedValue("c2", value.c2));
            }

            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float4x4> property, ref TContainer container, ref float4x4 value)
        {
            using (MakePathScope(property))
            {
                CustomEditorGUILayout.Vector4Label(GetDisplayName(property), value.c0, IsMixedValue("c0", value.c0));
                CustomEditorGUILayout.Vector4Label(GetEmptyNameForRow(), value.c1, IsMixedValue("c1", value.c1));
                CustomEditorGUILayout.Vector4Label(GetEmptyNameForRow(), value.c2, IsMixedValue("c2", value.c2));
                CustomEditorGUILayout.Vector4Label(GetEmptyNameForRow(), value.c3, IsMixedValue("c3", value.c3));
            }

            return VisitStatus.Stop;
        }

        static string GetEmptyNameForRow() => EditorGUIUtility.wideMode ? " " : string.Empty;

        bool2 IsMixedValue(string name, float2 value)
        {
            using (MakePathScope(name))
            {
                return new bool2(
                    IsMixedValue("x", value.x),
                    IsMixedValue("y", value.y));
            }
        }

        bool3 IsMixedValue(string name, float3 value)
        {
            using (MakePathScope(name))
            {
                return new bool3(
                    IsMixedValue("x", value.x),
                    IsMixedValue("y", value.y),
                    IsMixedValue("z", value.z));
            }
        }

        bool4 IsMixedValue(string name, float4 value)
        {
            using (MakePathScope(name))
            {
                return new bool4(
                    IsMixedValue("x", value.x),
                    IsMixedValue("y", value.y),
                    IsMixedValue("z", value.z),
                    IsMixedValue("w", value.w));
            }
        }
    }
}
