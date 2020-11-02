using Unity.Properties;
using Unity.Properties.Adapters.Contravariant;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer : IVisit<Object>
    {
        public VisitStatus Visit<TContainer>(IProperty<TContainer> property, ref TContainer container, Object value)
        {
            var type = value ? value.GetType() : typeof(Object);
            GUI.enabled = false;
            EditorGUILayout.ObjectField(GetDisplayName(property), value, type, true);
            GUI.enabled = true;
            return VisitStatus.Stop;
        }
    }
}
