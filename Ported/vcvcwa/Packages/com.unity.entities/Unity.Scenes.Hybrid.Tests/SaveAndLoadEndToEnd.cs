using System;
using System.Collections;
using NUnit.Framework;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Scenes.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Scenes.Tests
{
    public class SaveAndLoadEndToEnd : ECSTestsFixture
    {
        // Required test coverage:
        // * In playmode test so we test in both editor & player
        // * Using normal default initialized world
        // * Use a scene that is in version control on disk and was manuallly created. Let same pipeline that is used by end users generate the cached data.
        // 
        // * Load scene with multiple sections.
        //      * Validate seperate loadability.
        //      * Transform parents are correctly hooked up
        //      * Rendering data with HLOD is correctly configured seperated into sections & loaded. Validate against actual expected components from a scene
        // * Managed components with object references, Shared component with object references
        // * IComponentData, ChunkComponentData
        // * Scene with only pure entities no shared component (validate loads correctly as well as no asset bundle is created in build)

        [Test]
        [Ignore("Tests must be reimplemented to work at higher level to be truly end to end tests")]
        public void EndToEndTests()
        {
            
        }
#if false

        // Load / unload scene
        // Enter live link
        // Close live link
        // Unload scene after live link

        [UnityTest]
        public IEnumerator EndToEnd()
        {
            var guid = GUID.Generate();
            var temp = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            EditorSceneManager.SetActiveScene(temp);
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var entitySceneData = EditorEntityScenes.WriteEntityScene(temp, guid);
            Assert.AreEqual(1, entitySceneData.Length);
        
            var sceneEntity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(sceneEntity, entitySceneData[0]);

            for (int i = 0; i != 10; i++)
            {
                m_Manager.AddComponentData(sceneEntity, new RequestSceneLoaded());

                Assert.AreEqual(1, m_Manager.Debug.EntityCount);

                for (int w = 0; w != 1000; w++)
                {
                    World.GetOrCreateSystem<ResolveSceneReferenceSystem>().Update();
                    World.GetOrCreateSystem<SceneSectionStreamingSystem>().Update();
                    if (1 != m_Manager.Debug.EntityCount)
                        break;

                    yield return null;
                }

                // 1. Scene entity
                // 2. Public ref array
                // 3. Mesh Renderer 
                Assert.AreEqual(3, m_Manager.Debug.EntityCount);

                m_Manager.RemoveComponent<RequestSceneLoaded>(sceneEntity);
                World.GetOrCreateSystem<SceneSectionStreamingSystem>().Update();

                Assert.AreEqual(1, m_Manager.Debug.EntityCount);
            }
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        class ManagedObjectWithObjectReference : IComponentData
        {
            public Texture2D Texture;
        }

        [UpdateInGroup(typeof(GameObjectConversionGroup))]
        [DisableAutoCreation]
        internal class TestAddTexturePerTransformConverter : GameObjectConversionSystem
        {
            protected override void OnUpdate()
            {
                Entities.ForEach((UnityEngine.Transform transform) =>
                {
                    if (transform.gameObject.scene.name != "EndToEndTest_ManagedComponentsScene")
                        return;

                    var entity = GetPrimaryEntity(transform);

                    DstEntityManager.AddComponentData(entity, new ManagedObjectWithObjectReference()
                    {
                        Texture = new Texture2D(512, 512)
                    }); ;
                });
            }
        }

        [UnityTest]
        public IEnumerator EndToEnd_ManagedComponents()
        {
            var guid = GUID.Generate();
            var temp = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            temp.name = "EndToEndTest_ManagedComponentsScene";
            EditorSceneManager.SetActiveScene(temp);
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var entitySceneData = EditorEntityScenes.WriteEntityScene(temp, guid);
            Assert.AreEqual(1, entitySceneData.Length);

            var sceneEntity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(sceneEntity, entitySceneData[0]);

            var query = m_Manager.CreateEntityQuery(typeof(ManagedObjectWithObjectReference));
            for (int i = 0; i != 10; i++)
            {
                m_Manager.AddComponentData(sceneEntity, new RequestSceneLoaded());

                Assert.AreEqual(1, m_Manager.Debug.EntityCount);

                for (int w = 0; w != 1000; w++)
                {
                    World.GetOrCreateSystem<SceneSectionStreamingSystem>().Update();
                    if (1 != m_Manager.Debug.EntityCount)
                        break;

                    yield return null;
                }

                // 1. Scene entity
                // 2. Public ref array
                // 3. Mesh Renderer 
                Assert.AreEqual(3, m_Manager.Debug.EntityCount);

                var entities = query.ToEntityArray(Collections.Allocator.TempJob);
                Assert.AreEqual(1, entities.Length);
                var component = m_Manager.GetComponentData<ManagedObjectWithObjectReference>(entities[0]);
                Assert.NotNull(component.Texture);
                Assert.AreEqual(512, component.Texture.width);
                Assert.AreEqual(512, component.Texture.height);
                entities.Dispose();

                m_Manager.RemoveComponent<RequestSceneLoaded>(sceneEntity);
                World.GetOrCreateSystem<SceneSectionStreamingSystem>().Update();

                Assert.AreEqual(1, m_Manager.Debug.EntityCount);
            }
            query.Dispose();

        }
#endif
#endif
    }
}
