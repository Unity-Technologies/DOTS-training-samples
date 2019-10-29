using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes.Editor
{
    internal static class SubSceneInspectorUtility
    {
        public static Transform GetUncleanHierarchyObject(SubScene[] subscenes)
        {
            foreach (var scene in subscenes)
            {
                var res = GetUncleanHierarchyObject(scene.transform);
                if (res != null)
                    return res;
            }

            return null;
        }

        public static Transform GetUncleanHierarchyObject(Transform child)
        {
            while (child)
            {
                if (child.localPosition != Vector3.zero)
                    return child;
                if (child.localRotation != Quaternion.identity)
                    return child;
                if (child.localScale!= Vector3.one)
                    return child;

                child = child.parent;
            }

            return null;
        }

        public static bool HasChildren(SubScene[] scenes)
        {
            foreach (var scene in scenes)
            {
                if (scene.transform.childCount != 0)
                    return true;
            }

            return false;
        }

        public static void CloseSceneWithoutSaving(params SubScene[] scenes)
        {
            foreach(var scene in scenes)
                EditorSceneManager.CloseScene(scene.EditingScene, true);
        }

        public struct LoadableScene
        {
            public Entity Scene;
            public string Name;
        }

        static NativeArray<Entity> GetActiveWorldSections(World world, Hash128 sceneGUID)
        {
            var sceneSystem = world?.GetExistingSystem<SceneSystem>();
            var entities = world?.EntityManager;
            if (sceneSystem == null)
                return default;

            var sceneEntity = sceneSystem.GetSceneEntity(sceneGUID);

            if (!entities.HasComponent<ResolvedSectionEntity>(sceneEntity))
                return default;
            return entities.GetBuffer<ResolvedSectionEntity>(sceneEntity).Reinterpret<Entity>().AsNativeArray();
        }

        public static LoadableScene[] GetLoadableScenes(SubScene[] scenes)
        {
            var loadables = new List<LoadableScene>();

            foreach (var scene in scenes)
            {
                foreach (var section in GetActiveWorldSections(World.DefaultGameObjectInjectionWorld, scene.SceneGUID))
                {
                    var name = scene.SceneAsset.name;
                    if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<SceneSectionData>(section))
                    {
                        var sectionIndex = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<SceneSectionData>(section).SubSectionIndex;
                        if (sectionIndex != 0)
                            name += $" Section: {sectionIndex}";

                        loadables.Add(new LoadableScene
                        {
                            Scene = section,
                            Name = name
                        });
                    }
                }
            }

            return loadables.ToArray();
        }

        public static bool IsEditingAll(SubScene[] scenes)
        {
            foreach (var scene in scenes)
            {
                if (!scene.IsLoaded)
                    return false;
            }

            return true;
        }

        public static bool CanEditScene(SubScene scene)
        {
#if UNITY_EDITOR
            // Disallow editing when in prefab edit mode
            if (PrefabStageUtility.GetPrefabStage(scene.gameObject) != null)
                return false;
            if (!scene.isActiveAndEnabled)
                return false;
#endif

            return !scene.IsLoaded;
        }

        public static bool IsLoaded(SubScene[] scenes)
        {
            foreach (var subScene in scenes)
            {
                if (subScene.IsLoaded)
                    return true;
            }

            return false;
        }

        public static bool CanEditScene(SubScene[] scenes)
        {
            foreach (var subScene in scenes)
            {
                if (CanEditScene(subScene))
                    return true;
            }

            return false;
        }

        public static void EditScene(params SubScene[] scenes)
        {
            foreach (var subScene in scenes)
            {
                if (CanEditScene(subScene))
                {
                    Scene scene;
                    if (Application.isPlaying)
                        scene = EditorSceneManager.LoadSceneInPlayMode(subScene.EditableScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                    else
                        scene = EditorSceneManager.OpenScene(subScene.EditableScenePath, OpenSceneMode.Additive);
                    scene.isSubScene = true;
                }
            }
        }

        public static void CloseAndAskSaveIfUserWantsTo(SubScene[] subScenes)
        {
            if (!Application.isPlaying)
            {
                var dirtyScenes = new List<Scene>();
                foreach (var scene in subScenes)
                {
                    if (scene.EditingScene.isLoaded && scene.EditingScene.isDirty)
                    {
                        dirtyScenes.Add(scene.EditingScene);
                    }
                }

                if (dirtyScenes.Count != 0)
                {
                    if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(dirtyScenes.ToArray()))
                        return;
                }

                CloseSceneWithoutSaving(subScenes);
            }
            else
            {
                foreach (var scene in subScenes)
                {
                    if (scene.EditingScene.isLoaded)
                        EditorSceneManager.UnloadSceneAsync(scene.EditingScene);
                }
            }
        }

        public static void SaveScene(SubScene[] subScenes)
        {
            foreach (var scene in subScenes)
            {
                if (scene.EditingScene.isLoaded && scene.EditingScene.isDirty)
                {
                    EditorSceneManager.SaveScene(scene.EditingScene);
                }
            }
        }

        public static bool IsDirty(SubScene[] scenes)
        {
            foreach (var scene in scenes)
            {
                if (scene.EditingScene.isLoaded && scene.EditingScene.isDirty)
                    return true;
            }

            return false;
        }

        public static MinMaxAABB GetActiveWorldMinMax(World world, UnityEngine.Object[] targets)
        {
            MinMaxAABB bounds = MinMaxAABB.Empty;

            var entities = world?.EntityManager;
            foreach (SubScene subScene in targets)
            {
                foreach (var section in GetActiveWorldSections(World.DefaultGameObjectInjectionWorld, subScene.SceneGUID))
                {
                    if (entities.HasComponent<SceneBoundingVolume>(section))
                        bounds.Encapsulate(entities.GetComponentData<SceneBoundingVolume>(section).Value);
                }
            }

            return bounds;
        }

        // Visualize SubScene using bounding volume when it is selected.
        public static void DrawSubsceneBounds(SubScene scene)
        {
            var isEditing = scene.IsLoaded;

            var entities = World.DefaultGameObjectInjectionWorld?.EntityManager;
            foreach (var section in GetActiveWorldSections(World.DefaultGameObjectInjectionWorld, scene.SceneGUID))
            {
                if (!entities.HasComponent<SceneBoundingVolume>(section))
                    continue;

                if (isEditing)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.gray;

                AABB aabb = entities.GetComponentData<SceneBoundingVolume>(section).Value;
                Gizmos.DrawWireCube(aabb.Center, aabb.Size);
            }
        }

        public static LiveLinkMode LiveLinkMode
        {
            get
            {
                if (EditorApplication.isPlaying || LiveLinkEnabledInEditMode)
                    return LiveLinkShowGameStateInSceneView ? LiveLinkMode.LiveConvertSceneView : LiveLinkMode.LiveConvertGameView;
                else
                    return LiveLinkMode.Disabled;
            }
        }

        public static bool LiveLinkEnabledInEditMode
        {
            get
            {
                return EditorPrefs.GetBool("Unity.Entities.Streaming.SubScene.LiveLinkEnabledInEditMode", false);
            }
            set
            {
                if (LiveLinkEnabledInEditMode == value)
                    return;

                EditorPrefs.SetBool("Unity.Entities.Streaming.SubScene.LiveLinkEnabledInEditMode", value);
                LiveLinkConnection.GlobalDirtyLiveLink();
                LiveLinkModeChanged();
            }
        }
        
        public static bool LiveLinkShowGameStateInSceneView
        {
            get
            {
                return EditorPrefs.GetBool("Unity.Entities.Streaming.SubScene.LiveLinkShowGameStateInSceneView", false);
            }
            set
            {
                if (LiveLinkShowGameStateInSceneView == value)
                    return;

                EditorPrefs.SetBool("Unity.Entities.Streaming.SubScene.LiveLinkShowGameStateInSceneView", value);
                LiveLinkConnection.GlobalDirtyLiveLink();
                LiveLinkModeChanged();
            }
        }

        public static event Action LiveLinkModeChanged = delegate { };
    }
}
