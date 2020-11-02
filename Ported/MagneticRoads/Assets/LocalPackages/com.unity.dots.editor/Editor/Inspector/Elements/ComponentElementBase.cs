using System.Collections.Generic;
using Unity.Entities.Editor.Inspectors;
using Unity.Properties;
using Unity.Properties.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    abstract class ComponentElementBase : BindableElement
    {
        static readonly Dictionary<string, string> s_DisplayNames = new Dictionary<string, string>();

        public ComponentPropertyType Type { get; }
        public string Path { get; }
        protected string DisplayName { get; }
        protected EntityInspectorContext Context { get; }
        protected EntityContainer Container { get; }

        protected ComponentElementBase(IComponentProperty property, EntityInspectorContext context)
        {
            Type = property.Type;
            Path = property.Name;
            DisplayName = GetDisplayName(property.Name);
            Context = context;
            Container = Context.EntityContainer;
        }

        protected PropertyElement CreateContent<TValue>(IComponentProperty property, ref TValue value)
        {
            Resources.Templates.Inspector.InspectorStyle.AddStyles(this);
            AddToClassList(UssClasses.Resources.Inspector);
            AddToClassList(UssClasses.Resources.ComponentIcons);

            InspectorUtility.CreateComponentHeader(this, property.Type, DisplayName);
            var foldout = this.Q<Foldout>(className: UssClasses.Inspector.Component.Header);
            var toggle = foldout.Q<Toggle>();
            var container = Container;

            toggle.AddManipulator(new ContextualMenuManipulator(evt => { OnPopulateMenu(evt.menu); }));

            var componentMenu = this.Q<VisualElement>(className: UssClasses.Inspector.Component.Menu);

            var content = new PropertyElement();
            foldout.contentContainer.Add(content);
            content.AddContext(Context);
            content.SetTarget(value);

            content.OnChanged += OnComponentChanged;

            foldout.contentContainer.AddToClassList(UssClasses.Inspector.Component.Container);

            if (container.IsReadOnly)
            {
                foldout.contentContainer.SetEnabled(false);
                foldout.RegisterCallback<ClickEvent, EntityInspectorContext>(OnClicked, Context, TrickleDown.TrickleDown);
            }

            return content;
        }

        protected abstract void OnComponentChanged(PropertyElement element, PropertyPath path);

        protected abstract void OnPopulateMenu(DropdownMenu menu);

        static void OnClicked(ClickEvent evt, EntityInspectorContext context)
        {
            var element = (VisualElement)evt.target;
            using (var pooled = Pooling.GetList<Foldout>())
            {
                var list = pooled.List;
                element.Query<Foldout>().ToList(list);
                foreach (var foldout in list)
                {
                    if (element == foldout)
                        continue;
                    if (!foldout.Q<Toggle>().worldBound.Contains(evt.position))
                        continue;

                    foldout.value = !foldout.value;
                    break;
                }
            }

            using (var pooled = Pooling.GetList<ObjectField>())
            {
                var list = pooled.List;
                element.Query<ObjectField>().ToList(list);
                foreach (var field in list)
                {
                    var display = field.Q(className: UssClasses.UIToolkit.ObjectField.Display);
                    if (null == display)
                        continue;
                    if (!display.worldBound.Contains(evt.position))
                        continue;

                    if (evt.clickCount == 1)
                        EditorGUIUtility.PingObject(field.value);
                    else
                    {
                        var value = field.value;
                        if (null != value && value)
                            Selection.activeObject = value;
                    }

                    break;
                }
            }

            using (var pooled = Pooling.GetList<EntityField>())
            {
                var list = pooled.List;
                element.Query<EntityField>().ToList(list);
                foreach (var field in list)
                {
                    var input = field.Q(className: "unity-entity-field__input");
                    if (null == input)
                        continue;
                    if (!input.worldBound.Contains(evt.position))
                        continue;

                    if (evt.clickCount > 1)
                    {
                        var world = context.World;
                        if (null == world || !world.IsCreated)
                            continue;
                        if (!context.EntityManager.Exists(field.value))
                            continue;

                        EntitySelectionProxy.SelectEntity(context.World, field.value);
                    }

                    break;
                }
            }
        }

        static string GetDisplayName(string propertyName)
        {
            if (!s_DisplayNames.TryGetValue(propertyName, out var displayName))
            {
                s_DisplayNames[propertyName] = displayName = ObjectNames.NicifyVariableName(propertyName.Replace("<", "[").Replace(">", "]"))
                    .Replace("_", " | ")
                    .Replace("[", "<")
                    .Replace("]", ">");
            }

            return displayName;
        }
    }
}
