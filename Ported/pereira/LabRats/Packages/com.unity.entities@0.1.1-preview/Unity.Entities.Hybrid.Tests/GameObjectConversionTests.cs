using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Entities;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using static Unity.Entities.GameObjectConversionUtility;

namespace UnityEngine.Entities.Tests
{
    class GameObjectConversionTests : ECSTestsFixture
    {
        GameObject LoadPrefab(string name)
            => AssetDatabase.LoadAssetAtPath<GameObject>($"Packages/com.unity.entities/Unity.Entities.Hybrid.Tests/{name}.prefab");
        
        [Test]
        public void ConversionIgnoresMissingMonoBehaviour()
        {
            TestTools.LogAssert.Expect(LogType.Warning, new Regex("missing"));
            
            var entity = ConvertGameObjectHierarchy(LoadPrefab("Conversion_Prefab_MissingMB"), World);

            Assert.IsTrue(m_Manager.Exists(entity));
        }
        
        [Test]
        public void ConversionOfGameObject()
        {
            var gameObject = new GameObject();
            var entity = ConvertGameObjectHierarchy(gameObject, World);

            Assert.IsFalse(m_Manager.HasComponent<Prefab>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Static>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Disabled>(entity));

            Object.DestroyImmediate(gameObject);
        }
        
        [Test]
        public void ConversionOfStatic()
        {
            var gameObject = new GameObject("", typeof(StaticOptimizeEntity));
            var entity = ConvertGameObjectHierarchy(gameObject, World);

            Assert.IsTrue(m_Manager.HasComponent<Static>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Translation>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Rotation>(entity));
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ConversionOfComponentDataProxy()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<EcsTestProxy>().Value = new EcsTestData(5);
            
            var entity = ConvertGameObjectHierarchy(gameObject, World);

            Assert.AreEqual(5, m_Manager.GetComponentData<EcsTestData>(entity).value);
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ConversionOfRectTransform()
        {
            var gameObject = new GameObject("blah", typeof(RectTransform));
            gameObject.AddComponent<EntityRefTestDataAuthoring>();
            gameObject.AddComponent<MockDataProxy>();

            var entity = ConvertGameObjectHierarchy(gameObject, World);

            Assert.IsTrue(m_Manager.HasComponent<MockData>(entity));
            Assert.IsTrue(m_Manager.HasComponent<EntityRefTestData>(entity));

            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ConversionOfComponentWithNullGameObjectPtr()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<EntityRefTestDataAuthoring>();

            // should generate no errors about null reference
            var entity = ConvertGameObjectHierarchy(gameObject, World);

            Assert.IsFalse(m_Manager.HasComponent<Prefab>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Static>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Disabled>(entity));

            var count = World.EntityManager.UniversalQuery.CalculateEntityCount();
            Assert.AreEqual(1, count);

            Object.DestroyImmediate(gameObject);
        }
 
        [Test]
        public void ConversionOfComponentWithDeletedGameObjectPtr()
        {
            var deletedGameObject = new GameObject("deleted");
            Object.DestroyImmediate(deletedGameObject);

            var gameObject = new GameObject();
            gameObject.AddComponent<EntityRefTestDataAuthoring>().Value = deletedGameObject;

            // should properly detect a deleted gameobject as null
            var entity = ConvertGameObjectHierarchy(gameObject, World);

            // and not convert anything extra
            var count = World.EntityManager.UniversalQuery.CalculateEntityCount();
            Assert.AreEqual(1, count);

            Object.DestroyImmediate(gameObject);
        }
 
        [Test]
        public void ConversionOfPrefabIsEntityPrefab()
        {
            var entity = ConvertGameObjectHierarchy(LoadPrefab("Conversion_Prefab"), World);
            Assert.IsTrue(m_Manager.HasComponent<Prefab>(entity));
            Assert.IsFalse(m_Manager.HasComponent<Disabled>(entity));
        }

        [Test]
        public void ConversionOfNullReference()
        {
            var go = new GameObject();
            go.AddComponent<EntityRefTestDataAuthoring>();
            
            var entity = ConvertGameObjectHierarchy(go, World);
            Assert.AreEqual(Entity.Null, m_Manager.GetComponentData<EntityRefTestData>(entity).Value);
            
            Object.DestroyImmediate(go);        
        }

        [Test]
        public void ConversionOfPrefabReferenceOtherPrefab()
        {
            var go = new GameObject();
            go.AddComponent<EntityRefTestDataAuthoring>().Value = LoadPrefab("Conversion_Prefab_Reference_Prefab");
            
            var entity = ConvertGameObjectHierarchy(go, World);
            Assert.IsFalse(m_Manager.HasComponent<Prefab>(entity));
            var referenced = m_Manager.GetComponentData<EntityRefTestData>(entity).Value;
            
            // Conversion_Prefab_Reference_Prefab.prefab
            Assert.IsTrue(m_Manager.HasComponent<Prefab>(referenced));
            Assert.AreEqual(1, m_Manager.GetComponentData<MockData>(referenced).Value);
            
            // Conversion_Prefab.prefab
            var referenced2 = m_Manager.GetComponentData<EntityRefTestData>(referenced).Value;
            Assert.IsTrue(m_Manager.HasComponent<Prefab>(referenced2));
            Assert.AreEqual(0, m_Manager.GetComponentData<MockData>(referenced2).Value);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void ConversionOfPrefabSelfReference()
        {
            var go = new GameObject();
            go.AddComponent<EntityRefTestDataAuthoring>().Value = LoadPrefab("Conversion_Prefab_Reference_Self");

            var entity = ConvertGameObjectHierarchy(go, World);
            var referenced = m_Manager.GetComponentData<EntityRefTestData>(entity).Value;
            Assert.IsTrue(m_Manager.HasComponent<Prefab>(referenced));
            Assert.AreEqual(referenced, m_Manager.GetComponentData<EntityRefTestData>(referenced).Value);
                        
            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void ReferenceOutsideConvertedGroupWarning()
        {
            TestTools.LogAssert.Expect(LogType.Warning, new Regex("not included in the conversion"));
            var go = new GameObject();
            
            var notIncluded = new GameObject();
            go.AddComponent<EntityRefTestDataAuthoring>().Value = notIncluded;

            var entity = ConvertGameObjectHierarchy(go, World);
            
            Assert.AreEqual(1, m_Manager.Debug.EntityCount);
            Assert.AreEqual(Entity.Null, m_Manager.GetComponentData<EntityRefTestData>(entity).Value);
                        
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(notIncluded);
        }

        [Test]
        public void SetEnabledOnPrefabOnCompleteSet()
        {
            var entity = ConvertGameObjectHierarchy(LoadPrefab("Conversion_Prefab_Hierarchy"), World);

            var mockQuery = m_Manager.CreateEntityQuery(typeof(MockData));
            var instance = m_Manager.Instantiate(entity);
            Assert.AreEqual(2, mockQuery.CalculateEntityCount());
            
            m_Manager.SetEnabled(instance, false);
            Assert.AreEqual(0, mockQuery.CalculateEntityCount());
            
            m_Manager.SetEnabled(instance, true);
            Assert.AreEqual(2, mockQuery.CalculateEntityCount());
        }

        
        [Test]
        public void InactiveHierarchyBecomesPartOfLinkedEntityGroupSet()
        {
            var go = new GameObject();
            var child = new GameObject();
            var childChild = new GameObject();

            child.SetActive(false);
            go.AddComponent<EntityRefTestDataAuthoring>().Value = child;
            child.transform.parent = go.transform;
            childChild.transform.parent = child.transform;
            
            var query = m_Manager.CreateEntityQuery(new EntityQueryDesc());
            
            var entity = ConvertGameObjectHierarchy(go, World);
            
            Assert.AreEqual(1, query.CalculateEntityCount());
            // Conversion will automatically add a LinkedEntityGroup to all inactive children
            // so that when enabling them, the whole hierarchy will get enabled
            m_Manager.SetEnabled(m_Manager.GetComponentData<EntityRefTestData>(entity).Value, true);
            Assert.AreEqual(3, query.CalculateEntityCount());

            Object.DestroyImmediate(go);
        }
        
        [Test]
        public void InactiveConversion()
        {
            var gameObject = new GameObject();
            var child = new GameObject();
            child.transform.parent = gameObject.transform;
            gameObject.gameObject.SetActive(false);
            
            ConvertGameObjectHierarchy(gameObject, World);

            Assert.AreEqual(0, m_Manager.CreateEntityQuery(typeof(Translation)).CalculateEntityCount());
            Assert.AreEqual(2, m_Manager.UniversalQuery.CalculateEntityCount());
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void DisabledBehaviourStripping()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<MockDataProxy>().enabled = false;
            gameObject.AddComponent<EntityRefTestDataAuthoring>().enabled = false;

            var entity = ConvertGameObjectHierarchy(gameObject, World);
            Object.DestroyImmediate(gameObject);

            Assert.AreEqual(1, m_Manager.Debug.EntityCount);
            Assert.IsFalse(m_Manager.HasComponent<EntityRefTestData>(entity));
            Assert.IsFalse(m_Manager.HasComponent<MockData>(entity));
        }
    }

    class SceneConversionTests : ECSTestsFixture
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
            var go = new GameObject("Test Conversion");
            go.transform.localPosition = new Vector3(1, 2, 3);
            
            ConvertScene(scene, m_Manager.World);
            
            // Check
            var entities = m_Manager.GetAllEntities();
            Assert.AreEqual(1, entities.Length);
            var entity = entities[0];

            Assert.AreEqual(3, m_Manager.GetComponentCount(entity));
            Assert.IsTrue(m_Manager.HasComponent<Translation>(entity));
            Assert.IsTrue(m_Manager.HasComponent<Rotation>(entity));

            Assert.AreEqual(new float3(1, 2, 3), m_Manager.GetComponentData<Translation>(entity).Value);
            Assert.AreEqual(quaternion.identity, m_Manager.GetComponentData<Rotation>(entity).Value);
            var localToWorld = m_Manager.GetComponentData<LocalToWorld>(entity).Value;
            Assert.IsTrue(localToWorld.Equals(go.transform.localToWorldMatrix));
            
            Object.DestroyImmediate(go);
        }
        

        [Test]
        public void IncrementalConversionLinkedGroup()
        {
            var conversionFlags = ConversionFlags.GameViewLiveLink | ConversionFlags.AssignName;
            // Parent (LinkedEntityGroup) (2 additional entities)
            // - Child (2 additional entities)
            // All reference parent game object
            
            var parent = new GameObject().AddComponent<EntityRefTestDataAuthoring>();
            var child = new GameObject().AddComponent<EntityRefTestDataAuthoring>();
            
            child.transform.parent = parent.transform;
            
            child.name = "child";
            child.AdditionalEntityCount = 2;
            child.DeclareLinkedEntityGroup = false;
            child.Value = parent.gameObject;

            parent.name = "parent";
            parent.AdditionalEntityCount = 2;
            parent.DeclareLinkedEntityGroup = true;
            parent.Value = parent.gameObject;

            var conversionWorld = ConvertIncrementalInitialize(SceneManager.GetActiveScene(), new GameObjectConversionSettings(World, conversionFlags));
            
            Entities.ForEach((ref EntityRefTestData data) =>
            {
                StringAssert.StartsWith("parent", m_Manager.GetName(data.Value));
            });
            var entity = EmptySystem.GetSingletonEntity<LinkedEntityGroup>();

            
            // Parent (LinkedEntityGroup) (2 additional entities)
            // - Child (1 additional entities)
            // All reference child game object
            child.Value = child.gameObject;
            child.AdditionalEntityCount = 1;
            parent.Value = child.gameObject;
            ConvertIncremental(conversionWorld, new [] { child.gameObject }, conversionFlags);

            Assert.AreEqual(5, m_Manager.Debug.EntityCount);
            Assert.AreEqual(5, m_Manager.GetBuffer<LinkedEntityGroup>(entity).Length);
            // We expect there to still only be one linked entity group and it should be the same entity as before
            // since it is attached to the primary entity which is not getting destroyed.
            Assert.AreEqual(entity, EmptySystem.GetSingletonEntity<LinkedEntityGroup>());

            
            Entities.ForEach((ref EntityRefTestData data) =>
            {
                StringAssert.StartsWith("child", m_Manager.GetName(data.Value));
            });

            
            foreach(var e in m_Manager.GetBuffer<LinkedEntityGroup>(entity).AsNativeArray())
                Assert.IsTrue(m_Manager.Exists(e.Value));
            
            Object.DestroyImmediate(parent.gameObject);
            
            conversionWorld.Dispose();
        }
        
        //@TODO: Test GetEntities
        
        //@TODO: Changed prefab reference results in thrown exception and thus rebuild.
    }
}
