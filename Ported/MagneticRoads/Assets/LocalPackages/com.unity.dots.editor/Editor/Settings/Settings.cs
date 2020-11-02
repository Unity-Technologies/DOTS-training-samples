using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Properties.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using SettingsProvider = UnityEditor.SettingsProvider;

namespace Unity.Entities.Editor
{
    abstract class Settings<T> : SettingsProvider
        where T : SettingsAttribute
    {
        readonly struct SettingWrapper
        {
            public readonly ISetting Setting;
            public readonly bool Internal;

            public SettingWrapper(ISetting setting, bool isInternal)
            {
                Setting = setting;
                Internal = isInternal;
            }
        }

        static readonly Dictionary<string, List<SettingWrapper>> s_Settings = new Dictionary<string, List<SettingWrapper>>();
        static readonly string k_Prefix = $"{typeof(Settings<T>).FullName}: ";

        protected static bool HasAnySettings
        {
            get
            {
                if (!Unsupported.IsDeveloperMode())
                    return s_Settings.Any(category => category.Value.Any(w => !w.Internal));
                return s_Settings.Count > 0;
            }
        }

        static Settings()
        {
            try
            {
                CacheSettings();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        static void CacheSettings()
        {
            if (typeof(T) == typeof(SettingsAttribute))
            {
                UnityEngine.Debug.LogError(
                    $"{k_Prefix} Constraint of type `{nameof(SettingsAttribute)}` is not allowed, you must use a derived type of type `{nameof(SettingsAttribute)}`.");
                return;
            }

            var userSettingsType = typeof(Unity.Serialization.Editor.UserSettings<>);

            foreach (var type in UnityEditor.TypeCache.GetTypesWithAttribute<T>())
            {
                if (type.IsAbstract || type.IsGenericType || !type.IsClass)
                    continue;

                if (!typeof(ISetting).IsAssignableFrom(type))
                {
                    Debug.LogError(
                        $"{k_Prefix} type `{type.FullName}` must implement `{typeof(ISetting)}` in order to be used as a setting.");
                    continue;
                }

                var typedUserSettings = userSettingsType.MakeGenericType(type);
                var getOrCreateMethod =
                    typedUserSettings.GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.Public);
                if (null == getOrCreateMethod)
                {
                    Debug.LogError(
                        $"{k_Prefix} Could not find the `GetOrCreate` method on `{userSettingsType.FullName}` class.");
                    continue;
                }

                var attributes = type.GetCustomAttributes<T>();
                foreach (var attribute in attributes)
                {
                    var setting = (ISetting)getOrCreateMethod.Invoke(null, new object[] { attribute.SectionName });
                    if (!s_Settings.TryGetValue(attribute.SectionName, out var list))
                    {
                        s_Settings[attribute.SectionName] = list = new List<SettingWrapper>();
                    }

                    list.Add(new SettingWrapper(setting, type.GetCustomAttributes<InternalSettingAttribute>().Any()));
                }
            }
        }

        static string PathForScope(SettingsScope scope)
        {
            switch (scope)
            {
                case SettingsScope.User:
                    return "Preferences/";
                case SettingsScope.Project:
                    return "Project/";
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }

        protected virtual string Title { get; }

        protected Settings(string path, SettingsScope scope, IEnumerable<string> keywords = null) : base(PathForScope(scope) + path, scope, keywords)
        {
            Title = path.Replace("/", " ");
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // TODO: Switch to use uxml/uss for this.
            var root = new VisualElement();
            rootElement.Add(root);
            var title = new Label(Title);
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginTop = 1;
            title.style.fontSize = 19;
            title.style.unityTextAlign = TextAnchor.MiddleLeft;
            title.style.height = 26;
            root.Add(title);

            Resources.Templates.Settings.AddStyles(rootElement);
            foreach (var kvp in s_Settings)
            {
                if (kvp.Value.All(w => w.Internal) && !Unsupported.IsDeveloperMode())
                    continue;

                var label = new Label { text = kvp.Key };
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                root.Add(label);

                foreach (var wrapper in kvp.Value)
                {
                    if (wrapper.Internal && !Unsupported.IsDeveloperMode())
                        continue;

                    var setting = wrapper.Setting;
                    var element = new PropertyElement();
                    element.style.marginLeft = -3;
                    element.SetAttributeFilter(AttributeFilter);
                    element.SetTarget(setting);
                    element.OnChanged += (propertyElement, path) => setting.OnSettingChanged(path);
                    root.Add(element);
                    element.RegisterCallback<GeometryChangedEvent>(evt =>
                        StylingUtility.AlignInspectorLabelWidth(element));
                }
            }
            root.AddToClassList("unity-inspector-element");
            root.style.paddingLeft = 10;

            base.OnActivate(searchContext, rootElement);
        }

        bool AttributeFilter(IEnumerable<Attribute> attributes)
        {
            return !attributes.OfType<InternalSettingAttribute>().Any() || Unsupported.IsDeveloperMode();
        }
    }
}
