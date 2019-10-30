using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Properties;
using Unity.Properties.Editor;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Searcher;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Build
{
    [CustomEditor(typeof(BuildSettingsScriptedImporter))]
    sealed class BuildSettingsScriptedImporterEditor : ScriptedImporterEditor
    {
        static class ClassNames
        {
            public const string BaseClassName = "build-settings";
            public const string Dependencies = BaseClassName + "__asset-dependencies";
            public const string Header = BaseClassName + "__asset-header";
            public const string HeaderLabel = BaseClassName + "__asset-header-label";
            public const string BuildAction = BaseClassName + "__build-action";
            public const string BuildDropdown = BaseClassName + "__build-dropdown";
            public const string AddComponent = BaseClassName + "__add-component-button";
        }
        
        struct BuildAction
        {
            public string Name;
            public Func<BuildSettings, bool> IsEnabled;
            public Action<BuildSettings> Action;
        }

        static readonly BuildAction k_Build = new BuildAction
        {
            Name = "Build", Action = bs => { bs.Build(); },
            IsEnabled = bs => bs.CanBuild
        };

        static readonly BuildAction k_BuildAndRun = new BuildAction
        {
            Name = "Build and Run", Action = bs =>
            {
                if (bs.Build().Failed)
                {
                    return;
                }
                if (bs.CanRun)
                {
                    bs.Run();
                }
            },
            IsEnabled = bs => bs.CanBuild
        };

        static readonly BuildAction k_Run = new BuildAction
        {
            Name = "Run", Action = bs => { bs.Run(); },
            IsEnabled = bs => bs.CanRun
        };
        
        // Needed because properties don't handle root collections well. 
        class DependenciesWrapper
        {
            public readonly List<BuildSettings> Dependencies = new List<BuildSettings>();
        }

        const string k_CurrentActionKey = "BuildAction-CurrentAction";
        bool m_IsModified;
        protected override bool needsApplyRevert { get; } = true;
        public override bool showImportedObject { get; } = false;

        readonly DependenciesWrapper m_DependenciesWrapper = new DependenciesWrapper();
        BuildAction CurrentBuildAction => BuildActions[CurrentActionIndex];
        int m_LastSerializedVersion;
        bool m_LastEditState;

        static List<BuildAction> BuildActions { get; } = new List<BuildAction>
        {
            k_Build,
            k_BuildAndRun,
            k_Run,
        };

        static int CurrentActionIndex
        {
            get => EditorPrefs.HasKey(k_CurrentActionKey) ? EditorPrefs.GetInt(k_CurrentActionKey) : BuildActions.IndexOf(k_BuildAndRun);
            set => EditorPrefs.SetInt(k_CurrentActionKey, value);
        }

        public override void OnEnable()
        {
            BuildSettings.AssetChanged += OnBuildSettingImported;
            base.OnEnable();
        }

        void OnBuildSettingImported(ComponentContainer<IBuildSettingsComponent> obj)
        {
            if (null != m_BuildSettingsRoot)
            {
                Refresh(m_BuildSettingsRoot);
            }
        }

        public override void OnDisable()
        {
            BuildSettings.AssetChanged -= OnBuildSettingImported;
            base.OnDisable();
        }

        protected override void OnHeaderGUI()
        {
            // Intentional
            //base.OnHeaderGUI();
        }

        public override bool HasModified()
        {
            return m_IsModified;
        }

        protected override void Apply()
        {
            Save();
            m_IsModified = false;
            base.Apply();
            Revert();
        }

        protected override void ResetValues()
        {
            Revert();
            m_IsModified = false;
            base.ResetValues();
        }

        BindableElement m_BuildSettingsRoot;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            m_BuildSettingsRoot = new BindableElement();
            m_BuildSettingsRoot.AddStyleSheetAndVariant(ClassNames.BaseClassName);
            Refresh(m_BuildSettingsRoot);

            root.contentContainer.Add(m_BuildSettingsRoot);
            root.contentContainer.Add(new IMGUIContainer(ApplyRevertGUI));
            return root;
        }
        
        void Refresh(BindableElement root)
        {
            root.Clear();
            var buildSettings = assetTarget as BuildSettings;
            if (null == buildSettings)
            {
                return;
            }
            m_LastEditState = AssetDatabase.IsOpenForEdit(buildSettings);
            var openedForEditUpdater = UIUpdaters.MakeBinding(buildSettings, root);
            openedForEditUpdater.OnPreUpdate += updater =>
            {
                if (!updater.Source)
                {
                    return;
                }
                m_LastEditState = AssetDatabase.IsOpenForEdit(updater.Source);
            };
            root.binding = openedForEditUpdater;
            // Show header + dependencies
            var headerRoot = new VisualElement();
            headerRoot.AddToClassList(ClassNames.Header);

            var nameLabel = new Label(buildSettings.name);
            nameLabel.AddToClassList(ClassNames.HeaderLabel);
            var labelUpdater = UIUpdaters.MakeBinding(buildSettings, nameLabel);
            labelUpdater.OnUpdate += (binding) =>
            {
                if (binding.Source)
                {
                    binding.Element.text = binding.Source.name;
                }
            };
            nameLabel.binding = labelUpdater;
            
            var dropdownButton = new VisualElement();
            dropdownButton.style.flexDirection = FlexDirection.Row;
            dropdownButton.style.justifyContent = Justify.FlexEnd;

            var currentAction = new Button { text = BuildActions[CurrentActionIndex].Name };
            currentAction.AddToClassList(ClassNames.BuildAction);
            currentAction.clickable = new Clickable(() => CurrentBuildAction.Action(buildSettings));
            var actionUpdater = UIUpdaters.MakeBinding(this, currentAction);
            actionUpdater.OnUpdate += (binding) =>
            {
                if (binding.Source)
                {
                    var buildAction = CurrentBuildAction;
                    binding.Element.text = buildAction.Name;
                    binding.Element.SetEnabled(buildAction.IsEnabled(buildSettings));
                }
            };
            currentAction.binding = actionUpdater;
            dropdownButton.Add(currentAction);
            nameLabel.Add(dropdownButton);
            
            var buildSettingsButton = new PopupField<BuildAction>(
                BuildActions,
                CurrentActionIndex,
                a => string.Empty,
                a => a.Name);
            buildSettingsButton.AddToClassList(ClassNames.BuildDropdown);
            dropdownButton.Add(buildSettingsButton);
            buildSettingsButton.RegisterValueChangedCallback(evt =>
            {
                CurrentActionIndex = BuildActions.IndexOf(evt.newValue);
                currentAction.clickable = new Clickable(() => CurrentBuildAction.Action(buildSettings));
            });

            headerRoot.Add(nameLabel);
            root.Add(headerRoot);
            var assetField = new ObjectField
            {
                objectType = typeof(BuildSettings)
            };
            assetField.Q<VisualElement>(className:"unity-object-field__selector").SetEnabled(false);
            assetField.SetValueWithoutNotify(assetTarget);
            var assetUpdater = UIUpdaters.MakeBinding(buildSettings, assetField);
            assetField.SetEnabled(m_LastEditState);
            assetUpdater.OnPreUpdate += updater => updater.Element.SetEnabled(m_LastEditState);
            assetField.binding = assetUpdater;
            headerRoot.Add(assetField);

            var dependencies = buildSettings.GetDependencies().ToList();
            m_DependenciesWrapper.Dependencies.Clear();
            m_DependenciesWrapper.Dependencies.AddRange(dependencies.Where(s => s == null || s is BuildSettings).Cast<BuildSettings>());
            var dependencyElement = new PropertyElement();
            dependencyElement.AddToClassList(ClassNames.BaseClassName);
            dependencyElement.SetTarget(m_DependenciesWrapper);
            var foldout = dependencyElement.Q<Foldout>();
            foldout.AddToClassList(ClassNames.Dependencies);
            foldout.Q<Toggle>().AddToClassList("component-container__component-header");
            foldout.contentContainer.AddToClassList("component-container__component-fields");

            dependencyElement.OnChanged += element =>
            {
                buildSettings.ClearDependencies();
                buildSettings.Dependencies.AddRange(m_DependenciesWrapper.Dependencies);
                Refresh(root);
                m_IsModified = true;

            };

            dependencyElement.SetEnabled(m_LastEditState);
            var dependencyUpdater = UIUpdaters.MakeBinding(buildSettings, dependencyElement);
            dependencyUpdater.OnPreUpdate += updater => updater.Element.SetEnabled(m_LastEditState);
            dependencyElement.binding = dependencyUpdater;
            root.Add(dependencyElement);

            // Show components
            var componentRoot = new BindableElement();
            var components = buildSettings.GetComponents();
            foreach (var component in components)
            {
                componentRoot.Add(GetComponentElement(buildSettings, component));
            }

            componentRoot.SetEnabled(m_LastEditState);
            var componentUpdater = UIUpdaters.MakeBinding(buildSettings, componentRoot);
            componentUpdater.OnUpdate += updater => updater.Element.SetEnabled(m_LastEditState);
            componentRoot.binding = componentUpdater;
            root.Add(componentRoot);

            var addComponentButton = new Button();
            addComponentButton.AddToClassList(ClassNames.AddComponent);
            addComponentButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                var databases = new[]
                {
                    ComponentSearcherDatabases.GetBuildSettingsDatabase(new HashSet<Type>(buildSettings.GetComponents().Select(com => com.GetType()))),
                };

                var searcher = new Searcher(
                    databases,
                    new AddBuildSettingsComponentAdapter("Add Component"));

                var editorWindow = EditorWindow.focusedWindow;
                var button = evt.target as Button;

                SearcherWindow.Show(
                    editorWindow,
                    searcher,
                    AddType,
                    button.worldBound.min + Vector2.up * 15.0f,
                    a => { },
                    new SearcherWindow.Alignment(SearcherWindow.Alignment.Vertical.Top,
                        SearcherWindow.Alignment.Horizontal.Left)
                );
            });

            addComponentButton.SetEnabled(m_LastEditState);
            var addButtonUpdater = UIUpdaters.MakeBinding(buildSettings, addComponentButton);
            addButtonUpdater.OnPreUpdate += updater => updater.Element.SetEnabled(m_LastEditState);
            addComponentButton.binding = addButtonUpdater;
            root.contentContainer.Add(addComponentButton);
        }

        void Revert()
        {
            var buildSettings = assetTarget as BuildSettings;
            var importer = target as BuildSettingsScriptedImporter;
            if (null == buildSettings || null == importer)
            {
                return;
            }
            
            var json = File.ReadAllText(importer.assetPath);
            BuildSettings.DeserializeFromJson(assetTarget as BuildSettings, json);
            Refresh(m_BuildSettingsRoot);
        }

        void Save()
        {
            var buildSettings = assetTarget as BuildSettings;
            var importer = target as BuildSettingsScriptedImporter;
            if (null == buildSettings || null == importer)
            {
                return;
            }
            buildSettings.SerializeToPath(importer.assetPath);
        }

        bool AddType(SearcherItem arg)
        {
            if (!(arg is TypeSearcherItem typeItem))
            {
                return false;
            }
            var type = typeItem.Type;
            (assetTarget as BuildSettings)?.SetComponent(type, TypeConstruction.Construct<IBuildSettingsComponent>(type));
            Refresh(m_BuildSettingsRoot);
            m_IsModified = true;
            return true;

        }

        VisualElement GetComponentElement(BuildSettings container, object component)
        {
            var componentType = component.GetType();
            var element = (VisualElement) Activator.CreateInstance(typeof(ComponentContainerElement<,>)
                .MakeGenericType(typeof(IBuildSettingsComponent), componentType), container, component);

            if (element is IChangeHandler changeHandler)
            {
                changeHandler.OnChanged += () => { m_IsModified = true; };
            }

            return element;
        }
    }
}
