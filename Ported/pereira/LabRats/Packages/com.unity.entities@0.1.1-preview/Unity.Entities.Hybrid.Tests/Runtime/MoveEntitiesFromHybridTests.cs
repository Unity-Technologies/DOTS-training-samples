using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace Unity.Entities.Tests
{
    class MoveEntitiesFromHybridTests : ECSTestsFixture
    {
        private interface ICastToInt
        {
            int ToInt();
        }

        internal class TestClassComponentA : MonoBehaviour, ICastToInt
        {
            public int Value;
            public int ToInt() => Value;
        }

        internal class TestClassComponentB : MonoBehaviour, ICastToInt
        {
            public int Value;
            public int ToInt() => Value;
        }

        internal class TestClassComponentC : MonoBehaviour, ICastToInt
        {
            public int Value;
            public int ToInt() => Value;
        }

        protected class TestComponentSystem : ComponentSystem
        {
            protected override void OnUpdate()
            {
            }
        }

        private static EntityQueryBuilder QueryBuilder(World world) => world.GetOrCreateSystem<TestComponentSystem>().Entities;

        private List<GameObject> gameObjects = new List<GameObject>();

        [TearDown]
        public override void TearDown()
        {
            foreach (var gameObject in gameObjects)
            {
                GameObject.DestroyImmediate(gameObject);
            }
            gameObjects.Clear();

            base.TearDown();
        }

        private Entity CreateHybrid(EntityManager manager, int valueA, int valueB, int valueC)
        {
            var obj = new GameObject();
            var entity = manager.CreateEntity();

            if (valueA != 0)
            {
                var comp = obj.AddComponent<TestClassComponentA>();
                comp.Value = valueA;
                manager.AddComponentObject(entity, comp);
            }

            if (valueB != 0)
            {
                var comp = obj.AddComponent<TestClassComponentB>();
                comp.Value = valueB;
                manager.AddComponentObject(entity, comp);
            }

            if (valueC != 0)
            {
                var comp = obj.AddComponent<TestClassComponentC>();
                comp.Value = valueC;
                manager.AddComponentObject(entity, comp);
            }

            gameObjects.Add(obj);
            return entity;
        }

        private int[] GetValueArray<T>(World world) where T : ICastToInt
        {
            using (var entities = QueryBuilder(world).WithAll<T>().ToEntityQuery().ToEntityArray(Allocator.TempJob))
            {
                var result = new int[entities.Length];

                for (int i = 0; i < entities.Length; ++i)
                {
                    result[i] = world.EntityManager.GetComponentObject<T>(entities[i]).ToInt();
                }

                return result;
            }
        }

        [Test]
        public void MoveEntitiesWithComponentObjects()
        {
            var entityAC = CreateHybrid(m_Manager, 123, 0, 345);

            using (var sourceWorld = new World("source"))
            {
                CreateHybrid(sourceWorld.EntityManager, 1230, 2340, 3450);
                m_Manager.MoveEntitiesFrom(sourceWorld.EntityManager);
            }

            CollectionAssert.AreEquivalent(new[] {123, 1230}, GetValueArray<TestClassComponentA>(World));
            CollectionAssert.AreEquivalent(new[] {2340}, GetValueArray<TestClassComponentB>(World));
            CollectionAssert.AreEquivalent(new[] {345, 3450}, GetValueArray<TestClassComponentC>(World));

            var query = QueryBuilder(World)
                .WithAll<TestClassComponentA, TestClassComponentC>()
                .WithNone<TestClassComponentB>()
                .ToEntityQuery();

            using (var dstWorld = new World("destination"))
            {
                using (var remap = m_Manager.CreateEntityRemapArray(Allocator.TempJob))
                {
                    dstWorld.EntityManager.MoveEntitiesFrom(m_Manager, query, remap);

                    CollectionAssert.AreEquivalent(new[] {123}, GetValueArray<TestClassComponentA>(dstWorld));
                    CollectionAssert.AreEquivalent(new int[] {}, GetValueArray<TestClassComponentB>(dstWorld));
                    CollectionAssert.AreEquivalent(new[] {345}, GetValueArray<TestClassComponentC>(dstWorld));
                }
            }

            CollectionAssert.AreEquivalent(new[] {1230}, GetValueArray<TestClassComponentA>(World));
            CollectionAssert.AreEquivalent(new[] {2340}, GetValueArray<TestClassComponentB>(World));
            CollectionAssert.AreEquivalent(new[] {3450}, GetValueArray<TestClassComponentC>(World));
        }
    }
}