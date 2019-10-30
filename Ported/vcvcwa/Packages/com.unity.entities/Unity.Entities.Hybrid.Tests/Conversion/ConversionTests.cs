using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TestTools;
using static Unity.Entities.GameObjectConversionUtility;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Tests.Conversion
{
    class ConversionTests : ConversionTestFixtureBase
    {
        [Test]
        public void ConversionIgnoresMissingMonoBehaviour()
        {
            LogAssert.Expect(LogType.Warning, new Regex("missing"));

            var entity = ConvertGameObjectHierarchy(LoadPrefab("Prefab_MissingMB"), MakeDefaultSettings());

            EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Any(entity));
        }

        [Test]
        public void ConversionOfGameObject()
        {
            var gameObject = CreateGameObject();
            var entity = ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings());

            EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(entity, k_RootComponents));
        }

        [Test]
        public void ConversionOfStaticGameObject()
        {
            var gameObject = CreateGameObject("", typeof(StaticOptimizeEntity));
            var entity = ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings());

            EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<Static, LocalToWorld, LinkedEntityGroup>(entity));
        }

        [Test]
        public void ConversionOfComponentDataProxy()
        {
            var gameObject = CreateGameObject();
            gameObject.AddComponent<EcsTestProxy>().Value = new EcsTestData(5);

            var entity = ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings());

            EntitiesAssert.ContainsOnly(m_Manager,
                // this is the converted gameobject we created above 
                EntityMatch.Exact(entity, new EcsTestData(5), k_RootComponents),
                // ComponentDataProxyBase requires GameObjectEntity which creates this redundant Entity into the destination world from its OnEnable
                // TODO: is this ^^ behavior right?
                EntityMatch.Exact<EcsTestData, Transform>());
        }

        [Test]
        public void ConversionOfPrefabIsEntityPrefab()
        {
            var entity = ConvertGameObjectHierarchy(LoadPrefab("Prefab"), MakeDefaultSettings());
            
            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<Prefab, MockData>(entity, k_RootComponents));
        }

        [Test]
        public void ConversionOfNullGameObjectReference()
        {
            var go = CreateGameObject();
            go.AddComponent<EntityRefTestDataAuthoring>();

            var entity = ConvertGameObjectHierarchy(go, MakeDefaultSettings());
            
            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<LocalToWorld, Translation, Rotation, LinkedEntityGroup>(entity, new EntityRefTestData()));
        }

        [Test]
        public void ConversionOfPrefabReferenceOtherPrefab()
        {
            var go = CreateGameObject();
            go.AddComponent<EntityRefTestDataAuthoring>().Value = LoadPrefab("Prefab_Reference_Prefab");

            var entity = ConvertGameObjectHierarchy(go, MakeDefaultSettings());
            var referenced = m_Manager.GetComponentData<EntityRefTestData>(entity).Value;
            var referenced2 = m_Manager.GetComponentData<EntityRefTestData>(referenced).Value;
            
            EntitiesAssert.ContainsOnly(m_Manager,
                // gameobject created above
                EntityMatch.Exact<EntityRefTestData>(entity, k_RootComponents),
                // Prefab_Reference_Prefab.prefab
                EntityMatch.Exact<EntityRefTestData, Prefab>(referenced, new MockData(1), k_RootComponents),
                // Prefab.prefab
                EntityMatch.Exact<Prefab>(referenced2, new MockData(), k_RootComponents));
        }

        [Test, Ignore("Not implemented")]
        public void ConversionOfScriptableObjectReferenceOtherScriptableObject()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ConversionOfPrefabSelfReference()
        {
            var go = CreateGameObject();
            go.AddComponent<EntityRefTestDataAuthoring>().Value = LoadPrefab("Prefab_Reference_Self");

            var entity = ConvertGameObjectHierarchy(go, MakeDefaultSettings());
            var referenced = m_Manager.GetComponentData<EntityRefTestData>(entity).Value;

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<EntityRefTestData>(entity, k_RootComponents),
                EntityMatch.Exact<Prefab, MockData>(referenced, new EntityRefTestData { Value = referenced }, k_RootComponents));
        }

        [Test, Ignore("Not implemented")]
        public void ConversionOfScriptableObjectSelfReference()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GameObjectReferenceOutsideConvertedGroupWarning()
        {
            LogAssert.Expect(LogType.Warning, new Regex("not included in the conversion"));
            var go = CreateGameObject();

            var notIncluded = CreateGameObject();
            go.AddComponent<EntityRefTestDataAuthoring>().Value = notIncluded;

            var entity = ConvertGameObjectHierarchy(go, MakeDefaultSettings());

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(entity, new EntityRefTestData(), k_RootComponents));
        }

        [Test, Ignore("Not implemented")]
        public void AssetReferenceOutsideConvertedGroupWarning()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void SetEnabledOnPrefabOnCompleteSet()
        {
            var entity = ConvertGameObjectHierarchy(LoadPrefab("Prefab_Hierarchy"), MakeDefaultSettings());
            var instance = m_Manager.Instantiate(entity);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<Prefab>(new MockData(100), k_RootComponents, entity),
                EntityMatch.Exact<Prefab>(new MockData(101), k_ChildComponents),
                EntityMatch.Exact        (new MockData(100), k_RootComponents, instance),
                EntityMatch.Exact        (new MockData(101), k_ChildComponents));
            
            m_Manager.SetEnabled(instance, false);
            
            EntitiesAssert.Contains(m_Manager,
                EntityMatch.Exact<Disabled, MockData>(k_RootComponents, instance),
                EntityMatch.Exact<Disabled, MockData>(k_ChildComponents));

            m_Manager.SetEnabled(instance, true);
            
            EntitiesAssert.Contains(m_Manager,
                EntityMatch.Exact<MockData>(k_RootComponents, instance),
                EntityMatch.Exact<MockData>(k_ChildComponents));
        }

        [Test]
        public void InactiveHierarchyBecomesPartOfLinkedEntityGroupSet()
        {
            var go = CreateGameObject();
            var child = CreateGameObject();
            var childChild = CreateGameObject();

            child.SetActive(false);
            go.AddComponent<EntityRefTestDataAuthoring>().Value = child;
            child.transform.parent = go.transform;
            childChild.transform.parent = child.transform;

            var entity = ConvertGameObjectHierarchy(go, MakeDefaultSettings());
            var childEntity = m_Manager.GetComponentData<EntityRefTestData>(entity).Value;

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<EntityRefTestData, LinkedEntityGroup>(k_CommonComponents, entity),
                EntityMatch.Exact<Disabled,          LinkedEntityGroup>(k_ChildComponents, childEntity),
                EntityMatch.Exact<Disabled>                            (k_ChildComponents));

            // Conversion will automatically add a LinkedEntityGroup to all inactive children
            // so that when enabling them, the whole hierarchy will get enabled
            m_Manager.SetEnabled(m_Manager.GetComponentData<EntityRefTestData>(entity).Value, true);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<EntityRefTestData, LinkedEntityGroup>(k_CommonComponents, entity),
                EntityMatch.Exact<                   LinkedEntityGroup>(k_ChildComponents, childEntity),
                EntityMatch.Exact                                      (k_ChildComponents));
        }

        [Test]
        public void InactiveConversion()
        {
            var gameObject = CreateGameObject();
            var child = CreateGameObject();
            child.transform.parent = gameObject.transform;
            gameObject.gameObject.SetActive(false);

            var entity = ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings());

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<Disabled>(entity, k_RootComponents),
                EntityMatch.Exact<Disabled>(k_ChildComponents));
            
            Assert.That(Entities.WithAll<Translation>().ToEntityQuery().CalculateEntityCount(), Is.Zero);
        }

        [Test]
        public void DisabledBehaviourStripping()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<MockDataProxy>().enabled = false;
            gameObject.AddComponent<EntityRefTestDataAuthoring>().enabled = false;

            var strippedEntity = ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings());
            
            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(strippedEntity, k_RootComponents),
                EntityMatch.Exact<Transform>());
            
            UnityObject.DestroyImmediate(gameObject);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(strippedEntity, k_RootComponents));
        }

        [Test]
        public void DuplicateComponentOnRootGameObject()
        {
            var gameObject = new GameObject();

            gameObject.AddComponent<EntityRefTestDataAuthoring>();
            gameObject.AddComponent<EntityRefTestDataAuthoring>();

            Assert.DoesNotThrow(() => ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings()));

            LogAssert.Expect(LogType.Warning, new Regex(@"Missing entity for root GameObject"));

            EntitiesAssert.IsEmpty(m_Manager);
        }
    }
}
