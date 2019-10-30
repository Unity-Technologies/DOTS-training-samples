using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Entities.Conversion;
using UnityEngine;
using UnityEngine.TestTools;
using ConversionFlags = Unity.Entities.GameObjectConversionUtility.ConversionFlags;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Tests.Conversion
{
    class ConversionMappingTestFixtureBase : ConversionTestFixtureBase
    {
        protected World m_DstWorld { get; private set; }
        protected GameObjectConversionMappingSystem m_MappingSystem { get; private set; }

        protected int CalcConversionWorldEntityCount() => m_MappingSystem.EntityManager.UniversalQuery.CalculateEntityCount();

        [SetUp]
        public new void Setup()
        {
            m_DstWorld = new World(TestContext.CurrentContext.Test.FullName);
            var settings = new GameObjectConversionSettings(m_DstWorld, ConversionFlags.AssignName | ConversionFlags.AddEntityGUID);
            m_MappingSystem = World.CreateSystem<GameObjectConversionMappingSystem>(settings);
        }

        [TearDown]
        public new void TearDown()
        {
            m_DstWorld.Dispose();
            m_DstWorld = null;
            m_MappingSystem = null;
        }
    }

    class ConversionMappingStateTests : ConversionMappingTestFixtureBase
    {
        [Test]
        public void SetUp_HasExpectedValues()
        {
            Assert.AreSame(m_DstWorld.EntityManager, m_MappingSystem.DstEntityManager);
            Assert.IsTrue(m_MappingSystem.AddEntityGUID);
            Assert.IsFalse(m_MappingSystem.ForceStaticOptimization);
            Assert.IsTrue(m_MappingSystem.AssignName);
            Assert.IsFalse(m_MappingSystem.IsLiveLink);
            Assert.AreEqual(ConversionState.NotConverting, m_MappingSystem.ConversionState);
        }

        [Test]
        public void ConversionState_WithProgressionThroughConversionPhases_ReturnsMatchingState()
        {
            Assert.AreEqual(ConversionState.NotConverting, m_MappingSystem.ConversionState);

            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.EndConversion());
            Assert.AreEqual(ConversionState.NotConverting, m_MappingSystem.ConversionState);

            m_MappingSystem.BeginConversion();
            Assert.AreEqual(ConversionState.Discovering, m_MappingSystem.ConversionState);
            m_MappingSystem.BeginConversion();
            Assert.AreEqual(ConversionState.Discovering, m_MappingSystem.ConversionState);

            m_MappingSystem.CreatePrimaryEntities();
            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.BeginConversion());
            Assert.AreEqual(ConversionState.Converting, m_MappingSystem.ConversionState);

            m_MappingSystem.EndConversion();
            Assert.AreEqual(ConversionState.Converting, m_MappingSystem.ConversionState);

            m_MappingSystem.EndConversion();
            Assert.AreEqual(ConversionState.NotConverting, m_MappingSystem.ConversionState);
        }
    }

    class ConversionMappingTests : ConversionMappingTestFixtureBase
    {
        class TestBehaviour : MonoBehaviour { }

        [SetUp]
        public new void Setup()
        {
            m_MappingSystem.BeginConversion();
        }

        [Test]
        public void AddingConversionObjects_WithDestinationConversionStarted_Throws()
        {
            // ok to add

            m_MappingSystem.AddGameObjectOrPrefab(CreateGameObject("OkGameObject"));
            m_MappingSystem.DeclareReferencedPrefab(LoadPrefab("Prefab"));

            // start destination world conversion

            m_MappingSystem.CreatePrimaryEntities();

            // not ok to add

            void Check(Exception x) => StringAssert.Contains(nameof(GameObjectDeclareReferencedObjectsGroup), x.Message);

            Check(Assert.Throws<InvalidOperationException>(() =>
                m_MappingSystem.AddGameObjectOrPrefab(CreateGameObject("NotOkGameObject"))));
            Check(Assert.Throws<InvalidOperationException>(() =>
                m_MappingSystem.DeclareReferencedPrefab(LoadPrefab("Prefab_Hierarchy"))));
        }

        [Test]
        public void AddingConversionReferences_WithDestinationConversionStarted_DoesNotThrow()
        {
            // ok to add

            m_MappingSystem.DeclareDependency(CreateGameObject("Ok0Dependency0"), CreateGameObject("O0kDependency1"));
            m_MappingSystem.DeclareLinkedEntityGroup(CreateGameObject("Ok0LinkedEntityGroup"));

            // start destination world conversion

            m_MappingSystem.CreatePrimaryEntities();

            // still ok to add

            m_MappingSystem.DeclareDependency(CreateGameObject("Ok1Dependency0"), CreateGameObject("Ok1Dependency1"));
            m_MappingSystem.DeclareLinkedEntityGroup(CreateGameObject("Ok1LinkedEntityGroup"));
        }

        [Test]
        public void CheckObjectIsNotComponent()
        {
            //@TODO: combine with IsAsset etc. "trait" tests

            var go = CreateGameObject();
            var component = go.AddComponent<Rigidbody>();

            Assert.DoesNotThrow(() => go.CheckObjectIsNotComponent());
            Assert.Throws<InvalidOperationException>(() => component.CheckObjectIsNotComponent());
        }

        [Test]
        public void DeclareReferencedObjects_WithNull_DoesNothing()
        {
            m_MappingSystem.DeclareReferencedPrefab(null);
            m_MappingSystem.DeclareReferencedAsset(null);

            var count = CalcConversionWorldEntityCount();
            Assert.Zero(count);
        }

        [Test]
        public void DeclareReferencedPrefab_WithPrefabs_AddsEntitiesForUniquePrefabs()
        {
            var count0 = CalcConversionWorldEntityCount();
            m_MappingSystem.DeclareReferencedPrefab(LoadPrefab("Prefab"));
            var count1 = CalcConversionWorldEntityCount();
            m_MappingSystem.DeclareReferencedPrefab(LoadPrefab("Prefab_Hierarchy"));
            var count2 = CalcConversionWorldEntityCount();
            m_MappingSystem.DeclareReferencedPrefab(LoadPrefab("Prefab_Hierarchy"));
            var count3 = CalcConversionWorldEntityCount();
            m_MappingSystem.DeclareReferencedPrefab(LoadPrefab("Prefab"));
            var count4 = CalcConversionWorldEntityCount();

            Assert.AreEqual(0, count0);
            Assert.AreEqual(1, count1);
            Assert.AreEqual(3, count2);
            Assert.AreEqual(3, count3);
            Assert.AreEqual(3, count4);
        }

        [Test]
        public void CreatePrimaryEntities_WithVariousAddedObjects_MatchesPrimaryEntities()
        {
            // add conversion objects

            var go = CreateGameObject("GameObject");
            var prefab = LoadPrefab("Prefab");

            m_MappingSystem.AddGameObjectOrPrefab(go);
            m_MappingSystem.DeclareReferencedPrefab(prefab);

            // start conversion

            m_MappingSystem.CreatePrimaryEntities();

            // validate primaries match and are distinct

            Assert.IsTrue(m_MappingSystem.HasPrimaryEntity(go));
            Assert.IsTrue(m_MappingSystem.HasPrimaryEntity(prefab));

            var goEntity = m_MappingSystem.GetPrimaryEntity(go);
            var prefabEntity = m_MappingSystem.GetPrimaryEntity(prefab);

            Assert.AreNotEqual(goEntity, prefabEntity);

            Assert.AreEqual(goEntity, m_MappingSystem.TryGetPrimaryEntity(go));
            Assert.AreEqual(prefabEntity, m_MappingSystem.TryGetPrimaryEntity(prefab));
        }

        [Test]
        public void CreatePrimaryEntity_WithNull_Throws()
        {
            var x = Assert.Throws<ArgumentNullException>(() => m_MappingSystem.CreatePrimaryEntity(null));
            Assert.That(x.Message, Contains.Substring("must be called with a valid UnityEngine.Object"));
        }

        [Test]
        public void CreatePrimaryEntity_WithComponent_Throws()
        {
            var component = CreateGameObject().AddComponent<TestBehaviour>();

            var x = Assert.Throws<ArgumentException>(() => m_MappingSystem.CreatePrimaryEntity(component));
            Assert.That(x.Message, Contains.Substring("Object must be a GameObject, Prefab, or Asset"));
        }

        [Test]
        public void CreateAdditionalEntity_WithNull_Throws()
        {
            var x = Assert.Throws<ArgumentNullException>(() => m_MappingSystem.CreateAdditionalEntity(null));
            Assert.That(x.Message, Contains.Substring("must be called with a valid UnityEngine.Object"));
        }

        [Test]
        public void CreateAdditionalEntity_WithComponent_Throws()
        {
            var component = CreateGameObject().AddComponent<TestBehaviour>();

            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.CreateAdditionalEntity(component));
        }

        [Test]
        public void CreateAdditionalEntity_WithUnconvertedGameObject_Throws()
        {
            var go = CreateGameObject();

            var x = Assert.Throws<ArgumentException>(() => m_MappingSystem.CreateAdditionalEntity(go));
            StringAssert.IsMatch(".*GameObject.*was not included in the conversion.*", x.Message);
        }

        [Test]
        public void CreateAdditionalEntity_WithUndeclaredPrefab_Throws()
        {
            var prefab = LoadPrefab("Prefab");

            var x = Assert.Throws<ArgumentException>(() => m_MappingSystem.CreateAdditionalEntity(prefab));
            StringAssert.Contains("is a Prefab that was not declared for conversion", x.Message);
        }

        //@TODO: CreateAdditionalEntity_ with prefab, gameobject, asset

        [Test]
        public void GetPrimaryEntity_WithNull_ReturnsFalseAndNoLog()
        {
            var hasEntity = m_MappingSystem.HasPrimaryEntity(null);
            var tryEntity = m_MappingSystem.TryGetPrimaryEntity(null);
            var entity = m_MappingSystem.GetPrimaryEntity(null);

            Assert.False(hasEntity);
            Assert.AreEqual(Entity.Null, tryEntity);
            Assert.AreEqual(Entity.Null, entity);
        }

        [Test]
        public void GetPrimaryEntity_WithComponent_Throws()
        {
            var component = CreateGameObject().AddComponent<TestBehaviour>();

            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.HasPrimaryEntity(component));
            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.TryGetPrimaryEntity(component));
            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.GetPrimaryEntity(component));
        }

        [Test]
        public void GetPrimaryEntity_WithUnregisteredGameObject_ReturnsNullAndWarns()
        {
            var go = CreateGameObject();

            var hasEntity = m_MappingSystem.HasPrimaryEntity(go);
            var tryEntity = m_MappingSystem.TryGetPrimaryEntity(go);
            var entity = m_MappingSystem.GetPrimaryEntity(go);

            Assert.False(hasEntity);
            Assert.AreEqual(Entity.Null, tryEntity);
            Assert.AreEqual(Entity.Null, entity);
            LogAssert.Expect(LogType.Warning, new Regex(".*GameObject.*was not included in the conversion.*"));
        }

        [Test]
        public void GetPrimaryEntity_WithUnregisteredPrefab_ReturnsNullAndWarns()
        {
            var prefab = LoadPrefab("Prefab");

            var hasEntity = m_MappingSystem.HasPrimaryEntity(prefab);
            var tryEntity = m_MappingSystem.TryGetPrimaryEntity(prefab);
            var entity = m_MappingSystem.GetPrimaryEntity(prefab);

            Assert.False(hasEntity);
            Assert.AreEqual(Entity.Null, tryEntity);
            Assert.AreEqual(Entity.Null, entity);
            LogAssert.Expect(LogType.Warning, new Regex(".*is a Prefab that was not declared for conversion.*"));
        }

        [Test]
        public void GetPrimaryEntity_WithValidGo_ReturnsMatchingEntity()
        {
            var go = CreateGameObject();

            var entity = m_MappingSystem.CreatePrimaryEntity(go);
            var hasEntity = m_MappingSystem.HasPrimaryEntity(go);
            var tryEntity = m_MappingSystem.TryGetPrimaryEntity(go);
            var getEntity = m_MappingSystem.GetPrimaryEntity(go);
            var entities = m_MappingSystem.GetEntities(go);

            Assert.True(hasEntity);
            Assert.AreEqual(entity, tryEntity);
            Assert.AreEqual(entity, getEntity);

            CollectionAssert.AreEqual(entities, new[] { entity });
        }

        [Test]
        public void GetEntities_WithNull_ReturnsEmpty()
        {
            var count = m_MappingSystem.GetEntities(null).Count();
            Assert.Zero(count);
        }

        [Test]
        public void GetEntities_WithComponent_Throws()
        {
            var component = CreateGameObject().AddComponent<TestBehaviour>();

            Assert.Throws<InvalidOperationException>(() => m_MappingSystem.GetEntities(component));
        }

        [Test]
        public void GetEntities_WithNoMatch_ReturnsEmpty()
        {
            var go = CreateGameObject();

            Assert.That(m_MappingSystem.GetEntities(go), Is.Empty);
        }

        [Test]
        public void GetEntities_WithMixed_ReturnsExpected()
        {
            var (go0, go1) = (CreateGameObject("go0"), CreateGameObject("go1"));

            var entity0 = m_MappingSystem.CreatePrimaryEntity(go0);
            var additional0 = m_MappingSystem.CreateAdditionalEntity(go0);
            var additional1 = m_MappingSystem.CreateAdditionalEntity(go0);
            var entities0 = m_MappingSystem.GetEntities(go0);

            var entity1 = m_MappingSystem.CreatePrimaryEntity(go1);
            var entities1 = m_MappingSystem.GetEntities(go1);

            var allEntities = new[] { entity0, entity1, additional0, additional1 };
            CollectionAssert.AreEquivalent(allEntities, allEntities.Distinct());

            Assert.AreEqual(entity0, m_MappingSystem.GetPrimaryEntity(go0));
            Assert.AreEqual(entity1, m_MappingSystem.GetPrimaryEntity(go1));

            CollectionAssert.AreEqual(new [] { entity0, additional0, additional1 }, entities0);
            CollectionAssert.AreEqual(new [] { entity1 }, entities1);
        }

        // hi! if this test is failing for you, then inheritance support has been added to entity queries. this test
        // can be deleted (or updated), but not before scouring the codebase for places we are doing workarounds and
        // fixing them too. for example grep for:
        //    "Remove this again once KevinM adds support for inheritance in queries"
        //    "Revert this again once we add support for inheritance in queries"
        //    "inherited classes should probably be supported by queries, so we can delete this loop"
        // ...or anything where we have a specific reference to RectTransform anywhere in Entities
        [Test]
        public void Inheritance_NotCurrentlySupported()
        {
            var tf = CreateGameObject("transform");
            var rt = CreateGameObject("rect_transform", typeof(RectTransform));

            m_MappingSystem.AddGameObjectOrPrefab(tf);
            m_MappingSystem.AddGameObjectOrPrefab(rt);

            var ctf = 0;
            Entities.ForEach((Transform transform) => ++ctf);
            var crt = 0;
            Entities.ForEach((RectTransform transform) => ++crt);

            Assert.That(ctf, Is.EqualTo(1)); // when fixed, this should be == 2
            Assert.That(crt, Is.EqualTo(1));
        }
    }
}
