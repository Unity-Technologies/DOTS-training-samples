using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    public class NullElement<TType> : VisualElement
    {
        public NullElement(string name)
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.properties/Editor Default Resources/uss/null-element.uss"));
            AddToClassList("unity-properties-null");

            var label = new Label(name);
            label.AddToClassList("unity-properties-null--label");
            Add(label);

            var constructibleTypes = TypeConstruction.GetAllConstructibleTypes<TType>();
            if (constructibleTypes.Count > 0)
            {
                var button = new Button()
                {
                    text = "New Instance",
                    bindingPath = name
                };

                // Multiple choices are available 
                if (constructibleTypes.Count > 1)
                {
                    button.clickable.clicked += () =>
                    {
                        var menu = new GenericMenu();
                        foreach (var type in TypeConstruction.GetAllConstructibleTypes<TType>())
                        {
                            var t = type;
                            menu.AddItem(new GUIContent(t.Name), false, () =>
                            {
                                var newValue = TypeConstruction.Construct<TType>(type);
                                var binding = GetFirstAncestorOfType<PropertyElement>();
                                binding.SetValue(button, newValue);
                                binding.Refresh(true);
                            });
                        }

                        menu.DropDown(button.worldBound);
                    };
                }
                else
                {
                    button.clickable.clicked += () =>
                    {
                        var type = constructibleTypes[0];
                        var newValue = type == typeof(TType)
                            ? TypeConstruction.Construct<TType>()
                            : TypeConstruction.Construct<TType>(type);

                        var binding = GetFirstAncestorOfType<PropertyElement>();
                        binding.SetValue(button, newValue);
                        binding.Refresh(true);
                    };
                }

                button.AddToClassList("unity-properties-null--button");
                Add(button);
            }
            else
            {
                var value = new Label("Cannot create new Instance");
                value.AddToClassList("unity-properties-null--value");
                Add(value);
            }
        }
    }
}
