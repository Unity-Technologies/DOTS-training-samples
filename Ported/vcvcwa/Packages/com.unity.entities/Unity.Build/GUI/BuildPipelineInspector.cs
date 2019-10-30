using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Searcher;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.Build
{
    [CustomEditor(typeof(BuildPipeline), true)]
    class BuildPipelineInspector : Editor
    {
        BuildPipeline m_BuildPipeline;
        ReorderableList m_BuildStepsList;
        readonly List<IBuildStep> m_BuildSteps = new List<IBuildStep>();
        bool m_PipelineIsDirty;

        void MarkDirty()
        {
            m_PipelineIsDirty = true;
        }

        void Apply()
        {
            var path = AssetDatabase.GetAssetPath(m_BuildPipeline);
            m_BuildPipeline.SetSteps(m_BuildSteps.ToArray());
            EditorUtility.SetDirty(m_BuildPipeline);
            AssetDatabase.ForceReserializeAssets(new string[] { path }, ForceReserializeAssetsOptions.ReserializeAssets);
            m_PipelineIsDirty = false;
        }

        void Revert()
        {
            m_BuildSteps.Clear();
            if (!m_BuildPipeline.GetSteps(m_BuildSteps))
                Debug.LogError( $"Failed to get build steps from pipeline '{m_BuildPipeline.name}'", m_BuildPipeline);

            m_PipelineIsDirty = false;
        }

        private void OnEnable()
        {
            m_BuildPipeline = (target as BuildPipeline);
            Revert();
            m_BuildStepsList = new ReorderableList(m_BuildSteps, typeof(IBuildStep), true, true, true, true);
            m_BuildStepsList.onAddDropdownCallback = AddDropdownCallbackDelegate;
            m_BuildStepsList.drawElementCallback = ElementCallbackDelegate;
            m_BuildStepsList.drawHeaderCallback = HeaderCallbackDelegate;
            m_BuildStepsList.onReorderCallback = ReorderCallbackDelegate;
            m_BuildStepsList.onRemoveCallback = RemoveCallbackDelegate;
            m_BuildStepsList.drawFooterCallback = FooterCallbackDelegate;
            m_BuildStepsList.drawNoneElementCallback = DrawNoneElementCallback;
        }

        private void OnDisable()
        {
            if (m_PipelineIsDirty)
            {
                if (EditorUtility.DisplayDialog("Unapplied Changes Detected", AssetDatabase.GetAssetPath(m_BuildPipeline), "Apply", "Revert"))
                    Apply();
            }
        }

        static string GetDisplayName(Type t)
        {
            var attr = t.GetCustomAttribute<BuildStepAttribute>();
            var name = (attr == null || string.IsNullOrEmpty(attr.description)) ? t.Name : attr.description;
            var cat = (attr == null || string.IsNullOrEmpty(attr.category)) ? string.Empty : attr.category;
            if (string.IsNullOrEmpty(cat))
                return name;
            return $"{cat}/{name}";
        }

        static string GetCategory(Type t)
        {
            if (t == null)
                return string.Empty;
            var cat = t.GetCustomAttribute<BuildStepAttribute>()?.category;
            if (cat == null)
                return string.Empty;
            return cat;
        }

        static bool IsShown(Type t)
        {
            var flags = t.GetCustomAttribute<BuildStepAttribute>()?.flags;
            return (flags & BuildStepAttribute.Flags.Hidden) != BuildStepAttribute.Flags.Hidden;
        }

        bool AddStep(SearcherItem item)
        {
            if (item is TypeSearcherItem typeItem)
            {
                m_BuildSteps.Add(BuildPipeline.CreateStepFromType(typeItem.Type));
                MarkDirty();
                return true;
            }

            return false;
        }

        static bool FilterSearch(Type t, string searchString)
        {
            if (t == null && !string.IsNullOrEmpty(searchString))
                return false;
            return GetDisplayName(t).ToLower().Contains(searchString.ToLower());
        }

        bool OnFooter(Rect r)
        {
            if (!GUI.Button(r, new GUIContent("Browse...")))
                return true;

            var selPath = EditorUtility.OpenFilePanel("Select Build Pipeline Step Asset", "Assets", "asset");
            if (string.IsNullOrEmpty(selPath))
                return true;

            var assetsPath = Application.dataPath;
            if (!selPath.StartsWith(assetsPath))
            {
                Debug.LogErrorFormat("Assets are required to be in the Assets folder.");
                return false;
            }

            var relPath = "Assets" + selPath.Substring(assetsPath.Length);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relPath);
            if (obj == null)
            {
                Debug.LogErrorFormat("Unable to load asset at path {0}.", selPath);
                return false;
            }
            var step = obj as IBuildStep;
            if (step == null)
            {
                Debug.LogErrorFormat("Asset at path {0} is not a valid IBuildStep.", selPath);
                return false;
            }

            if (step == m_BuildPipeline as IBuildStep)
            {
                Debug.LogErrorFormat("IBuildStep at path {0} cannot be added to itself.", selPath);
                return false;
            }

            m_BuildSteps.Add(step);
            MarkDirty();
            return false;
        }

        void AddDropdownCallbackDelegate(Rect buttonRect, ReorderableList list)
        {
            var databases = new[]
            {
                ComponentSearcherDatabases.GetBuildStepsDatabase(
                    new HashSet<Type>(BuildStep.GetAvailableTypes(type => !IsShown(type))),
                    GetDisplayName),
            };

            var searcher = new Searcher(
                databases,
                new AddBuildSettingsComponentAdapter("Add Build Step"));

            var editorWindow = EditorWindow.focusedWindow;

            SearcherWindow.Show(
                editorWindow,
                searcher,
                AddStep,
                buttonRect.min + Vector2.up * 90.0f,
                a => { },
                new SearcherWindow.Alignment(SearcherWindow.Alignment.Vertical.Top,
                    SearcherWindow.Alignment.Horizontal.Left)
            );
        }

        void HandleDragDrop(Rect rect, int index)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.ContextClick:

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!rect.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (IBuildStep step in DragAndDrop.objectReferences)
                        {
                            m_BuildSteps.Insert(index, step);
                            MarkDirty();
                        }
                    }
                    break;
            }
        }

        void DrawNoneElementCallback(Rect rect)
        {
            ReorderableList.defaultBehaviours.DrawNoneElement(rect, false);
            HandleDragDrop(rect, 0);
        }

        void FooterCallbackDelegate(Rect rect)
        {
            ReorderableList.defaultBehaviours.DrawFooter(rect, m_BuildStepsList);
            HandleDragDrop(rect, m_BuildSteps.Count);
        }

        void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused)
        {
            var step = m_BuildSteps[index];
            GUI.Label(rect, step != null ? step.Description : "<null>");
            HandleDragDrop(rect, index);
        }

        void ReorderCallbackDelegate(ReorderableList list)
        {
            MarkDirty();
        }

        void HeaderCallbackDelegate(Rect rect)
        {
            GUI.Label(rect, new GUIContent("Build Steps"));
            HandleDragDrop(rect, 0);
        }

        void RemoveCallbackDelegate(ReorderableList list)
        {
            m_BuildSteps.RemoveAt(list.index);
            MarkDirty();
        }

        public override void OnInspectorGUI()
        {
            m_BuildStepsList.DoLayoutList();
            GUILayout.Space(10);
            EditorGUI.BeginDisabledGroup(!m_PipelineIsDirty);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Revert"))
                        Revert();
                    if (GUILayout.Button("Apply"))
                        Apply();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
