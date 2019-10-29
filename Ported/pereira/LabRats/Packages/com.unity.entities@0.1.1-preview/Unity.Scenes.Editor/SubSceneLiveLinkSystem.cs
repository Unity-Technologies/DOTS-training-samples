using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Scene = UnityEngine.SceneManagement.Scene;


namespace Unity.Scenes.Editor
{
    [ExecuteAlways]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SubSceneStreamingSystem))]
    class SubSceneLiveLinkSystem : ComponentSystem
    {
        class GameObjectPrefabLiveLinkSceneTracker : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                foreach (var asset in importedAssets)
                {
                    if (asset.EndsWith(".prefab", true, System.Globalization.CultureInfo.InvariantCulture))
                        GlobalDirtyLiveLink();
                }
            }
        }

        static int GlobalDirtyID = 0;
        static int PreviousGlobalDirtyID = 0;

        readonly MethodInfo m_GetDirtyIDMethod = typeof(Scene)
            .GetProperty("dirtyID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetMethod;

        HashSet<SceneAsset> m_EditingSceneAssets = new HashSet<SceneAsset>();

        static void AddUnique(ref List<SubScene> list, SubScene scene)
        {
            if (list == null)
                list = new List<SubScene>(10);
            if (!list.Contains(scene))
                list.Add(scene);
        }

        protected override void OnUpdate()
        {
            List<SubScene> needLiveLinkSync = null;
            List<SubScene> cleanupScene = null;
            List<SubScene> markSceneLoadedFromLiveLink = null;
            List<SubScene> removeSceneLoadedFromLiveLink = null;
            m_EditingSceneAssets.Clear();

            var liveLinkEnabled = SubSceneInspectorUtility.LiveLinkMode != LiveLinkMode.Disabled;

            // By default all scenes need to have m_GameObjectSceneCullingMask, otherwise they won't show up in game view
            for (int i = 0; i != EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene.isSubScene)
                {
                    if (liveLinkEnabled)
                        EditorSceneManager.SetSceneCullingMask(scene, EditorRenderData.LiveLinkEditSceneViewMask);
                    else
                        EditorSceneManager.SetSceneCullingMask(scene,
                            EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditGameViewMask);

                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                    if (scene.isLoaded && sceneAsset != null)
                        m_EditingSceneAssets.Add(sceneAsset);
                }
            }

            if (PreviousGlobalDirtyID != GlobalDirtyID)
            {
                Entities.ForEach((SubScene subScene) =>
                {
                    if (subScene.LiveLinkData != null)
                        subScene.LiveLinkData.RequestCleanConversion();
                });
                PreviousGlobalDirtyID = GlobalDirtyID;
            }

            Entities.ForEach((SubScene subScene) =>
            {
                var isLoaded = m_EditingSceneAssets.Contains(subScene.SceneAsset);
                // We are editing with live link. Ensure it is active & up to date
                if (isLoaded && liveLinkEnabled)
                {
                    if (subScene.LiveLinkData == null)
                        AddUnique(ref needLiveLinkSync, subScene);
                    else
                    {
                        if (subScene.LiveLinkData.LiveLinkDirtyID != GetSceneDirtyID(subScene.LoadedScene) ||
                            subScene.LiveLinkData.DidRequestUpdate())
                            AddUnique(ref needLiveLinkSync, subScene);
                    }
                }
                // We are editing without live link.
                // We should have no entity representation loaded for the scene.
                else if (isLoaded && !liveLinkEnabled)
                {
                    var hasAnythingLoaded = false;
                    foreach (var s in subScene._SceneEntities)
                        hasAnythingLoaded |= EntityManager.HasComponent<SubSceneStreamingSystem.StreamingState>(s) ||
                                             !EntityManager.HasComponent<SubSceneStreamingSystem.IgnoreTag>(s);

                    if (hasAnythingLoaded)
                    {
                        AddUnique(ref cleanupScene, subScene);
                        AddUnique(ref markSceneLoadedFromLiveLink, subScene);
                    }
                }
                // Scene is not being edited, thus should not be live linked.
                else
                {
                    var isDrivenByLiveLink = false;
                    foreach (var s in subScene._SceneEntities)
                        isDrivenByLiveLink |= EntityManager.HasComponent<SubSceneStreamingSystem.IgnoreTag>(s);

                    if (isDrivenByLiveLink || subScene.LiveLinkData != null)
                    {
                        AddUnique(ref cleanupScene, subScene);
                        AddUnique(ref removeSceneLoadedFromLiveLink, subScene);
                    }
                }
            });

            if (needLiveLinkSync != null)
            {
                var shouldDelayLiveLink = SubSceneInspectorUtility.LiveLinkMode == LiveLinkMode.ConvertWithoutDiff;
                // Live link changes to entity world
                foreach (var scene in needLiveLinkSync)
                {
                    if (shouldDelayLiveLink)
                    {
                        EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
                    }
                    else
                    {
                        // The current behaviour is that we do incremental conversion until we release the hot control
                        // This is to avoid any unexpected stalls
                        // Optimally the undo system would tell us if only properties have changed, but currently we don't have such an event stream.
                        var sceneDirtyID = GetSceneDirtyID(scene.LoadedScene);
                        if (IsHotControlActive())
                        {
                            if (scene.LiveLinkData != null)
                                LiveLinkScene.ApplyLiveLink(scene, World, scene.LiveLinkData.LiveLinkDirtyID,
                                    SubSceneInspectorUtility.LiveLinkMode);
                            else
                                EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
                        }
                        else
                        {
                            if (scene.LiveLinkData != null && scene.LiveLinkData.LiveLinkDirtyID != sceneDirtyID)
                                scene.LiveLinkData.RequestCleanConversion();
                            LiveLinkScene.ApplyLiveLink(scene, World, sceneDirtyID,
                                SubSceneInspectorUtility.LiveLinkMode);
                        }
                    }
                }
            }

            if (cleanupScene != null)
            {
                // Live link changes to entity world
                foreach (var scene in cleanupScene)
                {
                    CleanupScene(scene);
                }
            }

            if (markSceneLoadedFromLiveLink != null)
            {
                foreach (var scene in markSceneLoadedFromLiveLink)
                {
                    foreach (var sceneEntity in scene._SceneEntities)
                    {
                        if (!EntityManager.HasComponent<SubSceneStreamingSystem.IgnoreTag>(sceneEntity))
                            EntityManager.AddComponentData(sceneEntity, new SubSceneStreamingSystem.IgnoreTag());
                    }
                }
            }

            if (removeSceneLoadedFromLiveLink != null)
            {
                foreach (var scene in removeSceneLoadedFromLiveLink)
                {
                    foreach (var sceneEntity in scene._SceneEntities)
                    {
                        EntityManager.RemoveComponent<SubSceneStreamingSystem.IgnoreTag>(sceneEntity);
                    }
                }
            }
        }

        void CleanupScene(SubScene scene)
        {
            // Debug.Log("CleanupLiveLinkScene: " + scene.SceneName);
            scene.CleanupLiveLink();

            var streamingSystem = World.GetExistingSystem<SubSceneStreamingSystem>();

            foreach (var sceneEntity in scene._SceneEntities)
            {
                streamingSystem.UnloadSceneImmediate(sceneEntity);
                EntityManager.DestroyEntity(sceneEntity);
            }

            scene._SceneEntities = new List<Entity>();

            scene.UpdateSceneEntities(false);
        }

        static GameObject GetGameObjectFromAny(Object target)
        {
            Component component = target as Component;
            if (component != null)
                return component.gameObject;
            return target as GameObject;
        }

        internal static bool IsHotControlActive()
        {
            return GUIUtility.hotControl != 0;
        }


        UndoPropertyModification[] PostprocessModifications(UndoPropertyModification[] modifications)
        {
            foreach (var mod in modifications)
            {
                var target = GetGameObjectFromAny(mod.currentValue.target);
                if (target)
                {
                    var targetScene = target.scene;
                    Entities.ForEach((SubScene scene) =>
                    {
                        if (scene.LiveLinkData != null && scene.LoadedScene == targetScene)
                        {
                            scene.LiveLinkData.AddChanged(target);
                            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
                        }
                    });
                }
            }

            return modifications;
        }

        int GetSceneDirtyID(Scene scene)
        {
            if (scene.IsValid())
            {
                return (int) m_GetDirtyIDMethod.Invoke(scene, null);
            }
            else
                return -1;
        }

        public static void GlobalDirtyLiveLink()
        {
            GlobalDirtyID++;
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }

        protected override void OnCreate()
        {
            Undo.postprocessModifications += PostprocessModifications;
            Undo.undoRedoPerformed += GlobalDirtyLiveLink;

            Camera.onPreCull += OnPreCull;
            RenderPipelineManager.beginCameraRendering += OnPreCull;

            SceneView.duringSceneGui += SceneViewOnBeforeSceneGui;
        }

        //@TODO:
        // * This is a gross hack to show the Transform gizmo even though the game objects used for editing are hidden and thus the tool gizmo is not shown
        // * Also we are not rendering the selection (Selection must be drawn around live linked object, not the editing object)
        void SceneViewOnBeforeSceneGui(SceneView obj)
        {
            if (SubSceneInspectorUtility.LiveLinkMode == LiveLinkMode.LiveConvertSceneView)
            {
                ConfigureCamera(obj.camera, false);
            }
        }

        void OnPreCull(ScriptableRenderContext _, Camera camera) => OnPreCull(camera);

        void OnPreCull(Camera camera)
        {
            ConfigureCamera(camera, SubSceneInspectorUtility.LiveLinkMode == LiveLinkMode.LiveConvertSceneView);
        }

        void ConfigureCamera(Camera camera, bool sceneViewLiveLink)
        {
            if (camera.cameraType == CameraType.Game)
            {
                //Debug.Log("Configure game view");
                camera.overrideSceneCullingMask = EditorSceneManager.DefaultSceneCullingMask |
                                                  EditorRenderData.LiveLinkEditGameViewMask;
            }
            else if (camera.cameraType == CameraType.SceneView)
            {
                if (camera.scene.IsValid())
                {
                    // Debug.Log("Prefab view" + camera.GetInstanceID());
                    camera.overrideSceneCullingMask = 0;
                }
                else
                {
                    // Debug.Log("Scene view" + camera.GetInstanceID());
                    if (sceneViewLiveLink)
                        camera.overrideSceneCullingMask =
                            EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditGameViewMask;
                    else
                        camera.overrideSceneCullingMask =
                            EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditSceneViewMask;
                }
            }
        }

        protected override void OnDestroy()
        {
            Undo.postprocessModifications -= PostprocessModifications;
            Undo.undoRedoPerformed -= GlobalDirtyLiveLink;

            Camera.onPreCull -= OnPreCull;
            RenderPipelineManager.beginCameraRendering -= OnPreCull;
        }
    }
}
