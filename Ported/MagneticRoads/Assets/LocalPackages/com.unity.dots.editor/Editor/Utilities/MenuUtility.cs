using Unity.Properties;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    static partial class MenuUtility
    {
        const string k_CopyPrefix = "Copy/";

        abstract class CopyVisitor : PropertyVisitor
        {
            protected sealed override void VisitProperty<TContainer, TValue>(Property<TContainer, TValue> property,
                ref TContainer container, ref TValue value)
                => AddItem(property, ref value);

            protected sealed override void VisitCollection<TContainer, TCollection, TElement>(Property<TContainer, TCollection> property, ref TContainer container,
                ref TCollection value)
            {
            }

            protected abstract void AddItem<TContainer, TValue>(Property<TContainer, TValue> property, ref TValue value);
        }

        class DropdownMenuCopyVisitor : CopyVisitor
        {
            DropdownMenu m_Menu;

            public DropdownMenuCopyVisitor(DropdownMenu menu)
            {
                m_Menu = menu;
            }

            protected override void AddItem<TContainer, TValue>(Property<TContainer, TValue> property, ref TValue value)
            {
                var v = value;
                m_Menu.AppendAction($"{k_CopyPrefix}{property.Name}",
                    action => { EditorGUIUtility.systemCopyBuffer = JsonSerialization.ToJson(v); });
            }
        }

        class GenericMenuCopyVisitor : CopyVisitor
        {
            GenericMenu m_Menu;

            public GenericMenuCopyVisitor(GenericMenu menu)
            {
                m_Menu = menu;
            }

            protected override void AddItem<TContainer, TValue>(Property<TContainer, TValue> property, ref TValue value)
            {
                var v = value;
                m_Menu.AddItem(new GUIContent($"{k_CopyPrefix}{property.Name}"), false,
                    () => { EditorGUIUtility.systemCopyBuffer = JsonSerialization.ToJson(v); });
            }
        }

        public static void AddCopyValue<TValue>(this GenericMenu menu, TValue value)
        {
            menu.AddItem(new GUIContent($"{k_CopyPrefix}All"), false, () => { EditorGUIUtility.systemCopyBuffer = JsonSerialization.ToJson(value); });
            menu.AddSeparator(k_CopyPrefix);
            var visitor = new GenericMenuCopyVisitor(menu);
            PropertyContainer.Visit(value, visitor);
        }

        public static void AddCopyValue<TValue>(this DropdownMenu menu, TValue value)
        {
            menu.AppendAction($"{k_CopyPrefix}All", action => { EditorGUIUtility.systemCopyBuffer = JsonSerialization.ToJson(value); });
            menu.AppendSeparator(k_CopyPrefix);
            var visitor = new DropdownMenuCopyVisitor(menu);
            PropertyContainer.Visit(value, visitor);
        }
    }
}
