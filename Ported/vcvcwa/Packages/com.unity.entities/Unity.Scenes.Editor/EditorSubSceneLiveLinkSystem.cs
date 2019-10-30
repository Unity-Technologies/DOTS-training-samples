using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes.Editor
{
    [ExecuteAlways]
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(LiveLinkEditorSystemGroup))]
    class EditorSubSceneLiveLinkSystem : ComponentSystem
    {
        LiveLinkConnection         _EditorLiveLink;
        LiveLinkPatcher            _Patcher;
        LiveLinkSceneChangeTracker _SceneChangeTracker;

        // Temp data cached to reduce gc allocations
        List<LiveLinkChangeSet>    _ChangeSets;
        NativeList<Hash128>        _UnloadScenes;
        NativeList<Hash128>        _LoadScenes;

#if UNITY_2020_1_OR_NEWER
        ulong m_GizmoSceneCullingMask = 1UL << 59;
#endif

        protected override void OnUpdate()
        {
            // We can't initialize live link in OnCreate because other systems might configure BuildSettingsGUID from OnCreate
            if (_EditorLiveLink == null)
                _EditorLiveLink = new LiveLinkConnection(World.GetExistingSystem<SceneSystem>().BuildSettingsGUID);
            
            try
            {
                if (_SceneChangeTracker.GetSceneMessage(out var msg))
                {
                    _EditorLiveLink.ApplyLiveLinkSceneMsg(msg);   
                    msg.Dispose();
                }

                _EditorLiveLink.Update(_ChangeSets, _LoadScenes, _UnloadScenes, SubSceneInspectorUtility.LiveLinkMode);

                // Unload scenes that are no longer being edited / need to be reloaded etc
                foreach (var change in _UnloadScenes)
                {
                    _Patcher.UnloadScene(change);
                }
                
                // Apply changes to scenes that are being edited
                foreach (var change in _ChangeSets)
                {
                    _Patcher.ApplyPatch(change);
                    change.Dispose();
                }
            }
            finally
            {
                _LoadScenes.Clear();
                _ChangeSets.Clear();
                _UnloadScenes.Clear();
            }


            // Configure scene culling masks so that game objects & entities are rendered exlusively to each other
            var liveLinkEnabled = SubSceneInspectorUtility.LiveLinkMode != LiveLinkMode.Disabled;
            for (int i = 0; i != EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);

                var sceneGUID = new GUID(AssetDatabase.AssetPathToGUID(scene.path));

                if (_EditorLiveLink.HasScene(sceneGUID))
                {
#if UNITY_2020_1_OR_NEWER
                    if (SubSceneInspectorUtility.LiveLinkMode == LiveLinkMode.LiveConvertGameView)
                        EditorSceneManager.SetSceneCullingMask(scene, SceneCullingMasks.MainStageSceneViewObjects);
                    else if (SubSceneInspectorUtility.LiveLinkMode == LiveLinkMode.LiveConvertSceneView)
                        EditorSceneManager.SetSceneCullingMask(scene, m_GizmoSceneCullingMask);
                    else
                        EditorSceneManager.SetSceneCullingMask(scene, EditorSceneManager.DefaultSceneCullingMask);
#else
                    if (liveLinkEnabled)
                        EditorSceneManager.SetSceneCullingMask(scene, EditorRenderData.LiveLinkEditSceneViewMask);
                    else
                        EditorSceneManager.SetSceneCullingMask(scene, EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditGameViewMask);
#endif
                }
            }
        }

        protected override void OnCreate()
        {
            Camera.onPreCull += OnPreCull;
            RenderPipelineManager.beginCameraRendering += OnPreCull;
            SceneView.duringSceneGui += SceneViewOnBeforeSceneGui;

            _SceneChangeTracker = new LiveLinkSceneChangeTracker(EntityManager);

            _Patcher = new LiveLinkPatcher(World);
            _UnloadScenes = new NativeList<Hash128>(Allocator.Persistent);
            _LoadScenes = new NativeList<Hash128>(Allocator.Persistent);
            _ChangeSets = new List<LiveLinkChangeSet>();
        }
        
        protected override void OnDestroy()
        {
            Camera.onPreCull -= OnPreCull;
            RenderPipelineManager.beginCameraRendering -= OnPreCull;
            SceneView.duringSceneGui -= SceneViewOnBeforeSceneGui;

            if (_EditorLiveLink != null)
                _EditorLiveLink.Dispose();
            _SceneChangeTracker.Dispose();
            _Patcher.Dispose();
            _UnloadScenes.Dispose();
            _LoadScenes.Dispose();
        }

#if UNITY_2020_1_OR_NEWER
        //@TODO:
        // * This is a gross hack to show the Transform gizmo even though the game objects used for editing are hidden and thus the tool gizmo is not shown
        // * Also we are not rendering the selection (Selection must be drawn around live linked object, not the editing object)
        void SceneViewOnBeforeSceneGui(SceneView sceneView)
        {
            if (SubSceneInspectorUtility.LiveLinkMode == LiveLinkMode.LiveConvertSceneView)
            {
                Camera camera = sceneView.camera;
                bool sceneViewIsRenderingCustomScene = camera.scene.IsValid();
                if (!sceneViewIsRenderingCustomScene)
                {
                    // Add our gizmo hack bit before gizmo rendering so the SubScene GameObjects are considered visible
                    ulong newmask = camera.overrideSceneCullingMask | m_GizmoSceneCullingMask;
                    camera.overrideSceneCullingMask = newmask;
                }
            }
        }

        void OnPreCull(ScriptableRenderContext _, Camera camera) => OnPreCull(camera);

        void OnPreCull(Camera camera)
        {
            if (camera.cameraType == CameraType.SceneView)
            {
                // Ensure to remove our gizmo hack bit before rendering
                ulong newmask = camera.overrideSceneCullingMask & ~m_GizmoSceneCullingMask;
                camera.overrideSceneCullingMask = newmask; 
            }
        }

#else

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
                camera.overrideSceneCullingMask = EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditGameViewMask;
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
                        camera.overrideSceneCullingMask = EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditGameViewMask;
                    else
                        camera.overrideSceneCullingMask = EditorSceneManager.DefaultSceneCullingMask | EditorRenderData.LiveLinkEditSceneViewMask;
                }
            }
        }
#endif
    }
}
