using NUnit.Framework;
using Unity.Build;
using Unity.Entities.Conversion;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable Unity.InefficientPropertyAccess

namespace Unity.Entities.Tests.Conversion
{
    class SceneConversionTests : ConversionTestFixtureBase
    {
        [OneTimeSetUp]
        public void ClassInit()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            SceneManager.SetActiveScene(scene);
        }

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }

        [Test]
        public void ConvertGameObject_HasOnlyTransform_ProducesEntityWithPositionAndRotation()
        {
            var scene = SceneManager.GetActiveScene();
            var go = CreateGameObject("Test Conversion");
            go.transform.localPosition = new Vector3(1, 2, 3);

            GameObjectConversionUtility.ConvertScene(scene, World);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(
                    new Translation { Value = new float3(1, 2, 3) },
                    new Rotation { Value = quaternion.identity },
                    new LocalToWorld { Value = go.transform.localToWorldMatrix }));
        }

        [Test]
        public void IncrementalConversionLinkedGroup()
        {
            var conversionFlags = GameObjectConversionUtility.ConversionFlags.GameViewLiveLink | GameObjectConversionUtility.ConversionFlags.AssignName;
            // Parent (LinkedEntityGroup) (2 additional entities)
            // - Child (2 additional entities)
            // All reference parent game object

            var parent = CreateGameObject().AddComponent<EntityRefTestDataAuthoring>();
            var child = CreateGameObject().AddComponent<EntityRefTestDataAuthoring>();

            child.transform.parent = parent.transform;

            child.name = "child";
            child.AdditionalEntityCount = 2;
            child.DeclareLinkedEntityGroup = false;
            child.Value = parent.gameObject;

            parent.name = "parent";
            parent.AdditionalEntityCount = 2;
            parent.DeclareLinkedEntityGroup = true;
            parent.Value = parent.gameObject;

            using (var conversionWorld = GameObjectConversionUtility.ConvertIncrementalInitialize(SceneManager.GetActiveScene(), new GameObjectConversionSettings(World, conversionFlags)))
            {
                Entities.ForEach((ref EntityRefTestData data) =>
                    StringAssert.StartsWith("parent", m_Manager.GetName(data.Value)));

                var entity = EmptySystem.GetSingletonEntity<LinkedEntityGroup>();

                // Parent (LinkedEntityGroup) (2 additional entities)
                // - Child (1 additional entities)
                // All reference child game object
                child.Value = child.gameObject;
                child.AdditionalEntityCount = 1;
                parent.Value = child.gameObject;
                GameObjectConversionUtility.ConvertIncremental(conversionWorld, new[] { child.gameObject }, conversionFlags);

                EntitiesAssert.ContainsOnly(m_Manager,
                    EntityMatch.Exact<EntityRefTestData>(entity, k_CommonComponents,
                        EntityMatch.Component((LinkedEntityGroup[] group) => group.Length == 5)),
                    EntityMatch.Exact<EntityRefTestData>(k_ChildComponents),
                    EntityMatch.Exact<EntityRefTestData>(),
                    EntityMatch.Exact<EntityRefTestData>(),
                    EntityMatch.Exact<EntityRefTestData>());

                // We expect there to still only be one linked entity group and it should be the same entity as before
                // since it is attached to the primary entity which is not getting destroyed.
                Assert.AreEqual(entity, EmptySystem.GetSingletonEntity<LinkedEntityGroup>());

                Entities.ForEach((ref EntityRefTestData data) =>
                    StringAssert.StartsWith("child", m_Manager.GetName(data.Value)));

                foreach (var e in m_Manager.GetBuffer<LinkedEntityGroup>(entity).AsNativeArray())
                    Assert.IsTrue(m_Manager.Exists(e.Value));
            }
        }

        //@TODO: Test GetEntities

        //@TODO: Changed prefab reference results in thrown exception and thus rebuild.

        [Test]
        public void ConvertGameObject_WithFilteredBuildSettings_ConversionDoesntRun()
        {
            var scene = SceneManager.GetActiveScene();
            var go = CreateGameObject("Test Conversion");
            go.transform.localPosition = new Vector3(1, 2, 3);

            var bs = ScriptableObject.CreateInstance<BuildSettings>();
            bs.hideFlags = HideFlags.HideAndDontSave;

            bs.SetComponent(new ConversionSystemFilterSettings("Unity.Transforms.Hybrid"));

            var settings = new GameObjectConversionSettings
            {
                DestinationWorld = World,
                BuildSettings = bs
            };

            GameObjectConversionUtility.ConvertScene(scene, settings);

            // We still expect to find an Entity, just with nothing on it, because
            // entities are eagerly created for every GameObject even if no components
            // get converted on them.
            EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact());
        }
    }
}
