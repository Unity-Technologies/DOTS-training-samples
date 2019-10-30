using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Build;
using Unity.Build.Common;
using Unity.Scenes;
using Unity.Scenes.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Unity.Entities.Editor
{
    class LiveLinkBuildSettingsDropdown : PopupWindowContent
    {
        public LiveLinkBuildSettingsDropdown()
        {
            LiveLinkBuildSettings.CurrentLiveLinkBuildSettingsChanged += LiveLinkToolbar.RepaintPlaybar;
        }

        public void DrawDropdown()
        {
            var dropdownRect = new Rect(130, 0, 40, 22);

            var target = LiveLinkBuildSettings.CurrentLiveLinkBuildSettings?.GetComponent<ClassicBuildProfile>().Target ?? BuildTarget.NoTarget;
            var icon = Icons.Platform.GetIcon(target) ?? Icons.BuildSettingsDropdownDefaultIcon;

            icon.tooltip = LiveLinkBuildSettings.CurrentLiveLinkBuildSettings != null
                    ? $"Current Live Link build settings: {Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(LiveLinkBuildSettings.CurrentLiveLinkBuildSettings))}."
                    : "Set current Live Link build settings.";

            if (EditorGUI.DropdownButton(dropdownRect, icon, FocusType.Keyboard, LiveLinkStyles.Dropdown))
            {
                PopupWindow.Show(dropdownRect, this);
            }
        }

        public override Vector2 GetWindowSize() => SizeHelper.GetDropdownSize(LiveLinkBuildSettingsCache.Count);

        public override void OnGUI(Rect rect) { }

        public override void OnOpen()
        {
            const string basePath = "Packages/com.unity.entities/Editor/Resources/LiveLink";

            if (LiveLinkBuildSettingsCache.Count == 0)
            {
                editorWindow.rootVisualElement.Add(UIElementHelpers.LoadTemplate(basePath, "LiveLinkBuildSettingsDropdown.Empty", "LiveLinkBuildSettingsDropdown"));
                return;
            }

            var visualElement = UIElementHelpers.LoadTemplate(basePath, "LiveLinkBuildSettingsDropdown");
            var tpl = UIElementHelpers.LoadClonableTemplate(basePath, "LiveLinkBuildSettingsDropdown.ItemTemplate");

            var radioButtonGroup = visualElement.Q<RadioButtonGroup>();
            radioButtonGroup.RegisterValueChangedCallback(evt => LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = (BuildSettings) evt.newValue.userData);

            foreach (var buildSettingsCache in LiveLinkBuildSettingsCache.BuildSettings)
            {
                var item = tpl.GetNewInstance();
                var toggle = item.Q<Toggle>();
                var label = item.Q<Label>();
                var image = item.Q<Image>();

                label.RegisterCallback<PointerDownEvent>(e => OnClick(e, toggle));
                image.RegisterCallback<PointerDownEvent>(e => OnClick(e, toggle));

                toggle.userData = buildSettingsCache.Asset;
                toggle.SetValueWithoutNotify(LiveLinkBuildSettings.CurrentLiveLinkBuildSettings == buildSettingsCache.Asset);
                var fileName = Path.GetFileNameWithoutExtension(buildSettingsCache.Path);

                label.text = fileName.Length > SizeHelper.MaxCharCount ? fileName.Substring(0, SizeHelper.MaxCharCount) + "..." : fileName;
                label.tooltip = buildSettingsCache.Path;

                var target = buildSettingsCache.Asset.GetComponent<ClassicBuildProfile>().Target;
                image.AddToClassList(UIElementHelpers.Style.GetUssClass(target));

                radioButtonGroup.Add(item);
            }

            editorWindow.rootVisualElement.Add(visualElement);
        }

        static void OnClick(IPointerEvent evt, Toggle toggle)
        {
            if (evt.button != (int)MouseButton.LeftMouse) return;
            toggle.value = !toggle.value;
        }

        static bool CanBuildLiveLinkPlayer(BuildSettings buildSettings)
        {
            if (!buildSettings.CanBuild)
                return false;

            if (!buildSettings.TryGetComponent<ClassicBuildProfile>(out var profile) || profile.Target == BuildTarget.NoTarget)
                return false;

            // if CanBuild is true we know this build settings has an IBuildProfileComponent and a pipeline on it.
            return buildSettings.GetComponent<IBuildProfileComponent>().GetBuildPipeline().HasStep<BuildStepBuildClassicLiveLink>();
        }

        static class SizeHelper
        {
            static readonly Vector2 s_EmptyDropdownSize = new Vector2(280, ItemHeight + PaddingHeight);
            const int Width = 250;
            const int ItemHeight = 19;
            const int PaddingHeight = 12;
            const int TitleHeight = 20;

            internal const int MaxCharCount = 30;

            public static Vector2 GetDropdownSize(int itemCount)
                => itemCount == 0 ? s_EmptyDropdownSize : new Vector2(Width, itemCount * ItemHeight + PaddingHeight + TitleHeight);
        }

        internal static class LiveLinkBuildSettingsCache
        {
            static bool s_IsLoaded;
            static CachedBuildSetting[] s_CachedBuildSettings;

            public static void Reload()
            {
                s_IsLoaded = false;
                EnsureIsLoaded();
            }

            static void EnsureIsLoaded()
            {
                if (s_IsLoaded)
                    return;

                s_CachedBuildSettings = FindLiveLinkBuildSettingsInProject().ToArray();
                s_IsLoaded = true;
            }

            public static IEnumerable<CachedBuildSetting> BuildSettings => s_CachedBuildSettings;

            static IEnumerable<CachedBuildSetting> FindLiveLinkBuildSettingsInProject()
            {
                foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(BuildSettings).FullName}"))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    var asset = AssetDatabase.LoadAssetAtPath<BuildSettings>(path);
                    if (asset == null || !CanBuildLiveLinkPlayer(asset))
                        continue;

                    yield return new CachedBuildSetting(path, asset);
                }
            }

            public static int Count
            {
                get
                {
                    EnsureIsLoaded();
                    return s_CachedBuildSettings.Length;
                }
            }
        }

        internal readonly struct CachedBuildSetting
        {
            public readonly string Path;
            public readonly BuildSettings Asset;

            public CachedBuildSetting(string path, BuildSettings asset)
            {
                Path = path;
                Asset = asset;
            }
        }

        class LiveLinkBuildSettingsAssetPostprocessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                var buildSettingsExtension = '.' + BuildSettingsScriptedImporter.BuildSettingsExtension;
                if (importedAssets.Any(x => Path.GetExtension(x) == buildSettingsExtension)
                    || deletedAssets.Any(x => Path.GetExtension(x) == buildSettingsExtension)
                    || movedAssets.Any(x => Path.GetExtension(x) == buildSettingsExtension))
                {
                    LiveLinkBuildSettingsCache.Reload();

                    if (LiveLinkBuildSettings.CurrentLiveLinkBuildSettings != null && !CanBuildLiveLinkPlayer(LiveLinkBuildSettings.CurrentLiveLinkBuildSettings))
                        LiveLinkBuildSettings.CurrentLiveLinkBuildSettings = null;
                }
            }
        }
    }
}
