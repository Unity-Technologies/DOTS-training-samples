using System;
using Unity.Properties;
using Unity.Serialization.Editor;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    static class InspectorUtility
    {
        public static InspectorSettings Settings => UserSettings<InspectorSettings>.GetOrCreate(Constants.Settings.Inspector);

        public static void CreateComponentHeader(VisualElement parent, ComponentPropertyType type, string displayName)
        {
            Resources.Templates.Inspector.ComponentHeader.Clone(parent);
            var foldout = parent.Q<Foldout>(className: UssClasses.Inspector.Component.Header);
            foldout.text = displayName;
            foldout.Q<Label>(className: UssClasses.UIToolkit.Toggle.Text).AddToClassList(UssClasses.Inspector.Component.Name);

            var icon = new BindableElement();
            icon.AddToClassList(UssClasses.Inspector.Component.Icon);
            icon.AddToClassList(UssClasses.Inspector.Icons.Small);
            icon.AddToClassList(GetComponentClass(type));
            var input = foldout.Q<VisualElement>(className: UssClasses.UIToolkit.Toggle.Input);
            input.AddToClassList("shrink");
            input.Insert(1, icon);
            var categoryLabel = new Label(GetComponentCategoryPostfix(type));
            categoryLabel.AddToClassList(UssClasses.Inspector.Component.Category);
            input.Add(categoryLabel);
            categoryLabel.binding = new BooleanVisibilityPreferenceBinding
            { Target = categoryLabel, PreferencePath = new PropertyPath(nameof(InspectorSettings.DisplayComponentType)) };
            categoryLabel.binding.Update();
            var menu = new VisualElement();
            menu.AddToClassList(UssClasses.Inspector.Component.Menu);
            menu.AddToClassList(UssClasses.Inspector.Icons.Small);
            input.Add(menu);
            // TODO: Remove once we add menu items
            menu.Hide();
        }

        static string GetComponentCategoryPostfix(ComponentPropertyType type)
        {
            switch (type)
            {
                case ComponentPropertyType.Component: return "(Component)";
                case ComponentPropertyType.Tag: return "(Tag)";
                case ComponentPropertyType.SharedComponent: return "(Shared)";
                case ComponentPropertyType.ChunkComponent: return "(Chunk)";
                case ComponentPropertyType.HybridComponent: return "(Hybrid)";
                case ComponentPropertyType.Buffer: return "(Buffer)";
                case ComponentPropertyType.None:
                case ComponentPropertyType.All:
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        static string GetComponentClass(ComponentPropertyType type)
        {
            switch (type)
            {
                case ComponentPropertyType.Component: return UssClasses.Inspector.ComponentTypes.Component;
                case ComponentPropertyType.Tag: return UssClasses.Inspector.ComponentTypes.Tag;
                case ComponentPropertyType.SharedComponent: return UssClasses.Inspector.ComponentTypes.SharedComponent;
                case ComponentPropertyType.ChunkComponent: return UssClasses.Inspector.ComponentTypes.ChunkComponent;
                case ComponentPropertyType.HybridComponent: return UssClasses.Inspector.ComponentTypes.ManagedComponent;
                case ComponentPropertyType.Buffer: return UssClasses.Inspector.ComponentTypes.BufferComponent;
                case ComponentPropertyType.None:
                case ComponentPropertyType.All:
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
