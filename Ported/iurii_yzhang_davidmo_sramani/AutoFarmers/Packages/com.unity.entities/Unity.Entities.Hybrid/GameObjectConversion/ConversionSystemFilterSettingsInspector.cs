using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Conversion
{
#if UNITY_EDITOR
    [UsedImplicitly]
    sealed class ConversionSystemFilterSettingsInspector : Properties.Editor.IInspector<ConversionSystemFilterSettings>
    {
        int m_CurrentPickerWindow;

        public VisualElement Build(Properties.Editor.InspectorContext<ConversionSystemFilterSettings> context)
        {
            return new IMGUIContainer(() =>
            {
                if (OnGUI(context.Data))
                {
                    context.NotifyChanged();
                }
            });
        }

        public void Update(Properties.Editor.InspectorContext<ConversionSystemFilterSettings> context)
        {
        }

        bool OnGUI(ConversionSystemFilterSettings settings)
        {
            // Placeholder GUI until we'll have generic inspector
            GUILayout.Label("Assemblies to exclude Conversion Systems from:");

            for (var i = 0; i < settings.ExcludedConversionSystemAssemblies.Count; i++)
            {
                using (new UnityEditor.EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(settings.ExcludedConversionSystemAssemblies[i].name);
                    if (GUILayout.Button("Remove"))
                    {
                        settings.ExcludedConversionSystemAssemblies.RemoveAt(i);
                        settings.SetDirty();
                        return true;
                    }
                }
            }

            m_CurrentPickerWindow = UnityEditor.EditorGUIUtility.GetControlID(FocusType.Passive) + 100;

            if (GUILayout.Button("Add assembly"))
                UnityEditor.EditorGUIUtility.ShowObjectPicker<UnityEditorInternal.AssemblyDefinitionAsset>(null, false, "", m_CurrentPickerWindow);

            if (Event.current.commandName == "ObjectSelectorClosed" &&
                UnityEditor.EditorGUIUtility.GetObjectPickerControlID() == m_CurrentPickerWindow)
            {
                var assembly = UnityEditor.EditorGUIUtility.GetObjectPickerObject() as UnityEditorInternal.AssemblyDefinitionAsset;
                if (assembly == null || !assembly)
                    return false;

                if (!settings.ExcludedConversionSystemAssemblies.Contains(assembly))
                {
                    settings.ExcludedConversionSystemAssemblies.Add(assembly);
                    settings.SetDirty();
                    return true;
                }

                m_CurrentPickerWindow = -1;
                // Note: EditorGUI.BeginChangeCheck works incorrectly with object selector
                return false;
            }

            return false;
        }
    }
#endif
}