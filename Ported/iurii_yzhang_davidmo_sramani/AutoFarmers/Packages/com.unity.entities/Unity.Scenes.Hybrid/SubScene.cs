using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using File = System.IO.File;
using Hash128 = Unity.Entities.Hash128;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
#pragma warning disable 649


namespace Unity.Scenes
{
    [ExecuteAlways]
    public class SubScene : MonoBehaviour
    {
    #if UNITY_EDITOR
        [FormerlySerializedAs("sceneAsset")]
        [SerializeField] SceneAsset _SceneAsset;
        [SerializeField] Color _HierarchyColor = Color.gray;
    
        static List<SubScene> m_AllSubScenes = new List<SubScene>();
        public static IReadOnlyCollection<SubScene> AllSubScenes { get { return m_AllSubScenes; } }
        
    #endif
        
        public bool AutoLoadScene = true;


        [SerializeField]
        [HideInInspector]
        Hash128 _SceneGUID;

        [NonSerialized]
        Hash128 _AddedSceneGUID;

    #if UNITY_EDITOR

        [NonSerialized]
        bool _IsEnabled;
        
        public SceneAsset SceneAsset
        {
            get { return _SceneAsset; }
            set
            {
                _SceneAsset = value;
                OnValidate();
            }
        }

        public string SceneName
        {
            get { return SceneAsset.name; }
        }

        public Color HierarchyColor
        {
            get { return _HierarchyColor; }
            set { _HierarchyColor = value; }
        }
    
    
        public string EditableScenePath
        {
            get 
            { 
                return _SceneAsset != null ? AssetDatabase.GetAssetPath(_SceneAsset) : "";    
            }
        }
        
        public Scene EditingScene
        {
            get
            {
                if (_SceneAsset == null)
                    return default(Scene);
                
                return EditorSceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(_SceneAsset));
            }
        }
        
        public bool IsLoaded
        {
            get { return EditingScene.isLoaded; }
        }
        
        void OnValidate()
        {
            _SceneGUID = new GUID(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_SceneAsset)));
            
            if (_IsEnabled)
            {
                if (_SceneGUID != _AddedSceneGUID)
                {
                    RemoveSceneEntities();
                    if (_SceneGUID != default)
                        AddSceneEntities();
                }
            }
        }
    
    #endif
        
        public Hash128 SceneGUID => _SceneGUID;
    
        void OnEnable()
        {
    #if UNITY_EDITOR
            _IsEnabled = true;
    
            if (_SceneGUID == default(Hash128))
                return;
            
            if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null)
                return;
            
            foreach (var subscene in m_AllSubScenes)
            {
                if (subscene.SceneAsset == SceneAsset)
                {
                    UnityEngine.Debug.LogWarning($"A sub-scene can not include the same scene ('{EditableScenePath}') multiple times.", this);
                    return;
                }
            }
                 
            m_AllSubScenes.Add(this);    
    #endif

            DefaultWorldInitialization.DefaultLazyEditModeInitialize();
            AddSceneEntities();
        }
        
        void OnDisable()
        {
    #if UNITY_EDITOR
            _IsEnabled = false;
            m_AllSubScenes.Remove(this);
    #endif
            
            RemoveSceneEntities();
        }

        void AddSceneEntities()
        {
            Assert.IsTrue(_AddedSceneGUID == default);
            Assert.IsFalse(_SceneGUID == default);

            var flags = AutoLoadScene ? SceneLoadFlags.AutoLoad : 0;
#if UNITY_EDITOR
            flags |= EditorApplication.isPlaying ? SceneLoadFlags.BlockOnImport : 0;
#else
            flags |= SceneLoadFlags.BlockOnImport;
#endif            
            foreach (var world in World.AllWorlds)
            {
                var sceneSystem = world.GetExistingSystem<SceneSystem>();
                if (sceneSystem != null)
                {
                    
                    var loadParams = new SceneSystem.LoadParameters
                    {
                        Flags = flags
                    };
                    
                    var sceneEntity = sceneSystem.LoadSceneAsync(_SceneGUID, loadParams);
                    sceneSystem.EntityManager.AddComponentObject(sceneEntity, this);
                    _AddedSceneGUID = _SceneGUID;
                }
            }
        }
        void RemoveSceneEntities()
        {
            if (_AddedSceneGUID != default)
            {
                foreach (var world in World.AllWorlds)
                {
                    var sceneSystem = world.GetExistingSystem<SceneSystem>();
                    if (sceneSystem != null)
                        sceneSystem.UnloadScene(_AddedSceneGUID, SceneSystem.UnloadParameters.DestroySceneProxyEntity | SceneSystem.UnloadParameters.DestroySectionProxyEntities);
                }
                
                _AddedSceneGUID = default;
            }
        }

        //@TODO: Move this into SceneManager
        void UnloadScene()
        {
        //@TODO: ask to save scene first???
    #if UNITY_EDITOR
            var scene = EditingScene;
            if (scene.IsValid())
            {
                // If there is only one scene left in the editor, we create a new empty scene
                // before unloading this sub scene
                if (EditorSceneManager.loadedSceneCount == 1)
                {
                    Debug.Log("Creating new scene");
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
                }
    
                EditorSceneManager.UnloadSceneAsync(scene);    
            }
    #endif
        }
    
    
        private void OnDestroy()
        {
            UnloadScene();
        }

        [Obsolete("_SceneEntities has been deprecated, please use World.GetExistingSystem<SceneSystem>().LoadAsync / Unload using SceneGUID instead. (RemovedAfter 2019-11-22)")]
        public List<Entity> _SceneEntities
        {
            get
            {
                var entities = new List<Entity>();

                var sceneSystem = World.DefaultGameObjectInjectionWorld?.GetExistingSystem<SceneSystem>();
                
                if (sceneSystem != null)
                {
                    var sceneEntity = sceneSystem.GetSceneEntity(SceneGUID);
                    if (sceneEntity != Entity.Null && sceneSystem.EntityManager.HasComponent<ResolvedSectionEntity>(sceneEntity))
                    {
                        foreach(var section in sceneSystem.EntityManager.GetBuffer<ResolvedSectionEntity>(sceneEntity))
                            entities.Add(section.SectionEntity);    
                    }
                }

                return entities;
            }
        }

        /* @TODO: Add conversion. How do we prevent duplicate from OnEnable / OnDisable
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new SceneReference { SceneGUID = SceneGUID});
            if (AutoLoadScene)
                dstManager.AddComponent<RequestSceneLoaded>(entity);
        }
        */
    }
}
