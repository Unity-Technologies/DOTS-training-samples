using System.Collections;
using NUnit.Framework;
using Unity.Entities;
using Unity.Rendering;
using Unity.Scenes.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Scenes.Tests
{
    public class EndToEndStreaming
    {
        // Load / unload scene
        // Enter live link
        // Close live link
        // Unload scene after live link

        [UnityTest]
        public IEnumerator LodSplitOverSections()
        {
            using (var world = new World("TestWorld"))
            {
                var entityManager = world.EntityManager;

                var guid = GUID.Generate();
                var temp = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                EditorSceneManager.SetActiveScene(temp);

                // Large set of dummy entities to stress the public ref array
                const int dummyCount = 100;
                for (int i = 0; i < dummyCount; ++i)
                    new GameObject($"Dummy {i}");

                var lod0Renderer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var lod1Renderer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var lod2Renderer = GameObject.CreatePrimitive(PrimitiveType.Cube);

                lod0Renderer.transform.position = new Vector3(0, 0, 0);
                lod1Renderer.transform.position = new Vector3(1, 0, 0);
                lod2Renderer.transform.position = new Vector3(2, 0, 0);

                var highLod = new GameObject("HighLOD");
                highLod.SetActive(false);
                var highLodGroup = highLod.AddComponent<LODGroup>();

                var lowLod = new GameObject("LowLOD");
                lowLod.SetActive(false);
                var lowLodGroup = lowLod.AddComponent<LODGroup>();

                var hlod = new GameObject("HLOD");
                hlod.SetActive(false);
                var hlodComponent = hlod.AddComponent<HLOD>();
                var hlodGroup = hlod.GetComponent<LODGroup>();

                lod0Renderer.transform.parent = highLod.transform;
                lod1Renderer.transform.parent = highLod.transform;
                lod2Renderer.transform.parent = lowLod.transform;

                highLod.transform.parent = hlod.transform;
                lowLod.transform.parent = hlod.transform;

                highLodGroup.SetLODs(new[]
                {
                    new LOD(0.75f, new[] {lod0Renderer.GetComponent<Renderer>()}),
                    new LOD(0.00f, new[] {lod1Renderer.GetComponent<Renderer>()}),
                });

                lowLodGroup.SetLODs(new[]
                {
                    new LOD(0.00f, new[] {lod2Renderer.GetComponent<Renderer>()}),
                });

                hlodComponent.LODParentTransforms = new[] {highLod.transform, lowLod.transform};
                hlodGroup.SetLODs(new[]
                {
                    new LOD(0.25f, new Renderer[] { }),
                    new LOD(0.00f, new Renderer[] { }),
                });

                highLod.SetActive(true);
                lowLod.SetActive(true);
                hlod.SetActive(true);

                var entitySceneData = EditorEntityScenes.WriteEntityScene(temp, guid, 0);
                Assert.AreEqual(2, entitySceneData.Length);

                var sceneEntitySection0 = entityManager.CreateEntity();
                entityManager.AddComponentData(sceneEntitySection0, entitySceneData[0]);

                var sceneEntitySection1 = entityManager.CreateEntity();
                entityManager.AddComponentData(sceneEntitySection1, entitySceneData[1]);

                // Loading one scene at a time
                for (int i = 0; i != 10; i++)
                {
                    Assert.AreEqual(2, entityManager.Debug.EntityCount);

                    entityManager.AddComponentData(sceneEntitySection0, new RequestSceneLoaded());

                    for (int w = 0; w != 1000; w++)
                    {
                        world.GetOrCreateSystem<SubSceneStreamingSystem>().Update();
                        if (2 != entityManager.Debug.EntityCount)
                            break;

                        Assert.AreNotEqual(999, w, "Streaming is stuck");

                        yield return null;
                    }

                    // 1. Scene entity section 0
                    // 2. Scene entity section 1
                    // 3. Public ref array
                    // 4. HLOD
                    // 5. LowLod group
                    // 6. LOD2 renderer
                    Assert.AreEqual(dummyCount + 6, entityManager.Debug.EntityCount);

                    // Destroying and recreating the system causes the streaming worlds to reset, otherwise oversize
                    // buffers could carry over and would not trigger the right asserts.
                    world.DestroySystem(world.GetExistingSystem<SubSceneStreamingSystem>());

                    entityManager.AddComponentData(sceneEntitySection1, new RequestSceneLoaded());

                    for (int w = 0; w != 1000; w++)
                    {
                        world.GetOrCreateSystem<SubSceneStreamingSystem>().Update();
                        if (dummyCount + 6 != entityManager.Debug.EntityCount)
                            break;

                        Assert.AreNotEqual(999, w, "Streaming is stuck");

                        yield return null;
                    }

                    // 1. Scene entity section 0
                    // 2. Scene entity section 1
                    // 3. Public ref array
                    // 4. HLOD
                    // 5. LowLod group
                    // 6. LOD2 renderer
                    // 7. External ref array
                    // 8. HighLod group
                    // 9. LOD1 renderer
                    // A. LOD0 renderer
                    Assert.AreEqual(dummyCount + 10, entityManager.Debug.EntityCount);

                    entityManager.RemoveComponent<RequestSceneLoaded>(sceneEntitySection1);
                    world.GetOrCreateSystem<SubSceneStreamingSystem>().Update();

                    Assert.AreEqual(dummyCount + 6, entityManager.Debug.EntityCount);

                    entityManager.RemoveComponent<RequestSceneLoaded>(sceneEntitySection0);
                    world.GetOrCreateSystem<SubSceneStreamingSystem>().Update();

                    Assert.AreEqual(2, entityManager.Debug.EntityCount);
                }
            }
        }
    }
}
