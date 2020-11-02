using System;
using Unity.Entities;
using Unity.Properties;
using UnityEditor;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer : Properties.Adapters.IVisit
    {
        public VisitStatus Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
        {
            if (typeof(TValue).IsEnum)
            {
                // @TODO Handle mixed values.

                var options = Enum.GetNames(typeof(TValue));
                var local = value;

                var index = Array.FindIndex(options, name => name == local.ToString());

                EditorGUILayout.Popup
                    (
                        GetDisplayName(property),
                        index,
                        options
                    );

                return VisitStatus.Handled;
            }

            if (null == value)
            {
                PropertyField(property, value);
                return VisitStatus.Stop;
            }

            if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(BlobAssetReference<>))
            {
                return VisitStatus.Stop;
            }

            return VisitStatus.Unhandled;
        }
    }
}
