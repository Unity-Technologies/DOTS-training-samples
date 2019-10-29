using System.Collections.Generic;
using System.Reflection;
using Unity.Build;
using Unity.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hash128 = Unity.Entities.Hash128;
using Object = UnityEngine.Object;

namespace Unity.Scenes.Editor
{
    //@TODO: LiveLinkConnection is starting to be a relatively complex statemachine. Lets have unit tests for it in isolation... 
    
    // A connection to a Player with a specific build setting.
    // Each destination world in each player/editor, has it's own LiveLinkConnection so we can generate different data for different worlds.
    // For example server world vs client world.
    class LiveLinkConnection
    {
        static int                                 GlobalDirtyID = 0;

        static readonly MethodInfo                 _GetDirtyIDMethod = typeof(Scene).GetProperty("dirtyID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetMethod;

        HashSet<Hash128>                           _LoadedScenes = new HashSet<Hash128>();
        HashSet<Hash128>                           _SentLoadScenes = new HashSet<Hash128>();
        NativeList<Hash128>                        _RemovedScenes;
        Dictionary<Hash128, LiveLinkDiffGenerator> _SceneGUIDToLiveLink = new Dictionary<Hash128, LiveLinkDiffGenerator>();
        int                                        _PreviousGlobalDirtyID;
        Dictionary<Hash128, Scene>                 _GUIDToEditScene = new Dictionary<Hash128, Scene>();

        BuildSettings                              _BuildSettings;
        Hash128                                    _BuildSettingsGUID;
        UnityEngine.Hash128                        _BuildSettingsArtifactHash;
        
        public LiveLinkConnection(Hash128 buildSettingsGuid)
        {
            _BuildSettingsGUID = buildSettingsGuid;
            if (buildSettingsGuid != default)
            {
                _BuildSettings = BuildSettings.LoadBuildSettings(buildSettingsGuid);
                if (_BuildSettings == null)
                    Debug.LogError($"Unable to load build settings asset from guid {buildSettingsGuid}.");
            }

            Undo.postprocessModifications += PostprocessModifications;
            Undo.undoRedoPerformed += GlobalDirtyLiveLink;
            
            _RemovedScenes = new NativeList<Hash128>(Allocator.Persistent);
        }
        
        public void Dispose()
        {
            Undo.postprocessModifications -= PostprocessModifications;
            Undo.undoRedoPerformed -= GlobalDirtyLiveLink;

            foreach(var livelink in _SceneGUIDToLiveLink.Values)
                livelink.Dispose();
            _SceneGUIDToLiveLink.Clear();
            _SceneGUIDToLiveLink = null;
            _RemovedScenes.Dispose();
        }
        
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
        
        public static void GlobalDirtyLiveLink()
        {
            GlobalDirtyID++;
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
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
                    var liveLink = GetLiveLink(target.scene);
                    if (liveLink != null)
                    {
                        liveLink.AddChanged(target);
                        EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
                    }
                }
            }

            return modifications;
        }

        int GetSceneDirtyID(Scene scene)
        {
            if (scene.IsValid())
            {
                return (int) _GetDirtyIDMethod.Invoke(scene, null);
            }
            else
                return -1;
        }
        
        static GameObject GetGameObjectFromAny(Object target)
        {
            Component component = target as Component;
            if (component != null)
                return component.gameObject;
            return target as GameObject;
        }
        
        LiveLinkDiffGenerator GetLiveLink(Hash128 sceneGUID)
        {
            _SceneGUIDToLiveLink.TryGetValue(sceneGUID, out var liveLink);
            return liveLink;
        }
        
        LiveLinkDiffGenerator GetLiveLink(Scene scene)
        {
            //@TODO: Cache _SceneToLiveLink ???
            var guid = new GUID(AssetDatabase.AssetPathToGUID(scene.path));
            return GetLiveLink(guid);
        }

        public void ApplyLiveLinkSceneMsg(LiveLinkSceneMsg msg)
        {
            SetLoadedScenes(msg.LoadedScenes);
            QueueRemovedScenes(msg.RemovedScenes);
        }
        //@TODO: Privatize following two API's

        public void SetLoadedScenes(NativeArray<Hash128> loadedScenes)
        {
            _LoadedScenes.Clear();
            foreach (var scene in loadedScenes)
            {
                if (scene != default)
                    _LoadedScenes.Add(scene);
            }
        }
        public void QueueRemovedScenes(NativeArray<Hash128> removedScenes)
        {
            _RemovedScenes.AddRange(removedScenes);
        }
        
        public bool HasScene(Hash128 sceneGuid)
        {
            return _LoadedScenes.Contains(sceneGuid);
        }

        void RequestCleanConversion()
        {
            foreach(var liveLink in _SceneGUIDToLiveLink.Values)
                liveLink.RequestCleanConversion();
        }
        
        public void Update(List<LiveLinkChangeSet> changeSets, NativeList<Hash128> loadScenes, NativeList<Hash128> unloadScenes, LiveLinkMode mode)
        {
            // If build settings have changed, we need to trigger a full conversion
            if (_BuildSettingsGUID != default)
            {
                var buildSettingsDependencyHash = AssetDatabase.GetAssetDependencyHash(AssetDatabase.GUIDToAssetPath(_BuildSettingsGUID.ToString()));
                if (_BuildSettingsArtifactHash != buildSettingsDependencyHash)
                {
                    _BuildSettingsArtifactHash = buildSettingsDependencyHash;
                    RequestCleanConversion();
                }
            }

            if (_PreviousGlobalDirtyID != GlobalDirtyID)
            {
                RequestCleanConversion();
                _PreviousGlobalDirtyID = GlobalDirtyID;
            }

            // By default all scenes need to have m_GameObjectSceneCullingMask, otherwise they won't show up in game view
            _GUIDToEditScene.Clear();
            for (int i = 0; i != EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                var sceneGUID = new GUID(AssetDatabase.AssetPathToGUID(scene.path));

                if (_LoadedScenes.Contains(sceneGUID))
                {
                    if (scene.isLoaded && sceneGUID != default(GUID))
                        _GUIDToEditScene.Add(sceneGUID, scene);
                }
            }

            foreach (var scene in _SceneGUIDToLiveLink)
            {
                if (!_GUIDToEditScene.ContainsKey(scene.Key))
                    unloadScenes.Add(scene.Key);
            }
            
            // Process scenes that are no longer loaded
            foreach (var scene in unloadScenes)
            {
                var liveLink = _SceneGUIDToLiveLink[scene];
                liveLink.Dispose();
                _SceneGUIDToLiveLink.Remove(scene);
                _SentLoadScenes.Remove(scene);
            }
            foreach (var scene in _RemovedScenes)
            {
                if (_SceneGUIDToLiveLink.TryGetValue(scene, out var liveLink))
                {
                    liveLink.Dispose();
                    _SceneGUIDToLiveLink.Remove(scene);
                }
                
                unloadScenes.Add(scene);
                _SentLoadScenes.Remove(scene);
            }
            _RemovedScenes.Clear();

            _SentLoadScenes.RemoveWhere(scene => !_LoadedScenes.Contains(scene));
            
            // Process all scenes that the player needs
            foreach(var sceneGuid in _LoadedScenes)
            {
                var isLoaded = _GUIDToEditScene.TryGetValue(sceneGuid, out var scene);

                // We are editing with live link. Ensure it is active & up to date
                if (isLoaded)
                {
                    var liveLink = GetLiveLink(sceneGuid);
                    if (liveLink == null)
                        AddLiveLinkChangeSet(sceneGuid, changeSets, mode);
                    else
                    {
                        if (liveLink.LiveLinkDirtyID != GetSceneDirtyID(scene) || liveLink.DidRequestUpdate())
                            AddLiveLinkChangeSet(sceneGuid, changeSets, mode);
                    }
                }
                else
                {
                    if (_SentLoadScenes.Add(sceneGuid))
                        loadScenes.Add(sceneGuid);
                }
            }
        }
	    
        void AddLiveLinkChangeSet(Hash128 sceneGUID, List<LiveLinkChangeSet> changeSets, LiveLinkMode mode)
        {
            var liveLink = GetLiveLink(sceneGUID);
            var editScene = _GUIDToEditScene[sceneGUID];
            
            // The current behaviour is that we do incremental conversion until we release the hot control
            // This is to avoid any unexpected stalls
            // Optimally the undo system would tell us if only properties have changed, but currently we don't have such an event stream.
            var sceneDirtyID = GetSceneDirtyID(editScene);
            var updateLiveLink = true;
            if (IsHotControlActive())
            {
                if (liveLink != null)
                {
                    sceneDirtyID = liveLink.LiveLinkDirtyID;
                }
                else
                {
                    updateLiveLink = false;
                    EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();    
                }
            }
            else
            {
                if (liveLink != null && liveLink.LiveLinkDirtyID != sceneDirtyID)
                    liveLink.RequestCleanConversion();
            }

            if (updateLiveLink)
            {
                //@TODO: need one place that LiveLinkDiffGenerators are managed. UpdateLiveLink does a Dispose()
                // but this must be paired with membership in _SceneGUIDToLiveLink. not good to have multiple places
                // doing ownership management.
                //
                // also: when implementing an improvement to this, be sure to deal with exceptions, which can occur
                // during conversion.

                if (liveLink != null)
                    _SceneGUIDToLiveLink.Remove(sceneGUID);

                try
                {
                    changeSets.Add(LiveLinkDiffGenerator.UpdateLiveLink(editScene, sceneGUID, ref liveLink, sceneDirtyID, mode, _BuildSettings));
                }
                finally
                {
                    if (liveLink != null)
                        _SceneGUIDToLiveLink.Add(sceneGUID, liveLink);
                }
            }
        }
    }
}
