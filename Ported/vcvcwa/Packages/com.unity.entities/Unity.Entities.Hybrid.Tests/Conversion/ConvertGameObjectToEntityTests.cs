using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Entities.Tests.Conversion
{
    static class Extensions
    {
        public static GameObject AddConvertAndDestroy(this GameObject go)
        {
            go.AddComponent<ConvertToEntity>().ConversionMode = ConvertToEntity.Mode.ConvertAndDestroy;
            return go;
        }

        public static GameObject AddConvertAndInject(this GameObject go)
        {
            go.AddComponent<ConvertToEntity>().ConversionMode = ConvertToEntity.Mode.ConvertAndInjectGameObject;
            return go;
        }

        public static GameObject AddStopConvert(this GameObject go)
        {
            go.AddComponent<StopConvertToEntity>();
            return go;
        }

        public static GameObject ParentTo(this GameObject child, GameObject parent)
        {
            child.transform.parent = parent.transform;
            return child;
        }
    }

    class ConvertGameObjectToEntityTests : ConversionTestFixtureBase
    {
        static void AwakeConversion(Transform root, MethodInfo methodInfo)
        {
            foreach(Transform child in root)
                AwakeConversion(child, methodInfo);

            var convert = root.GetComponent<ConvertToEntity>();
            if (convert != null)
                methodInfo.Invoke(convert, null);
        }

        ConvertToEntitySystem BeginAwakeConversion(params GameObject[] gameObjects)
        {
            var methodInfo = typeof(ConvertToEntity).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);

            foreach(var go in gameObjects)
                AwakeConversion(go.transform, methodInfo);

            return World.GetOrCreateSystem<ConvertToEntitySystem>();
        }

        void EndAwakeConversion()
        {
            World.GetExistingSystem<ConvertToEntitySystem>().Update();
        }

        void AwakeConversion(params GameObject[] gameObjects)
        {
            BeginAwakeConversion(gameObjects);
            EndAwakeConversion();
        }

        [Test]
        public void EntitiesCanBeReferencedAcrossConversionRoots()
        {
            var a = CreateGameObject("a", DestructionBy.Test);
            var b = CreateGameObject("b", DestructionBy.Test);

            a.AddConvertAndDestroy();
            b.AddConvertAndDestroy();

            a.AddComponent<EntityRefTestDataAuthoring>().Value = b;
            b.AddComponent<EntityRefTestDataAuthoring>().Value = a;

            AwakeConversion(a, b);

            Entity entityX;
            Entity entityY;

            using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
            {
                entityX = entities[0];
                entityY = entities[1];
            }

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(new EntityRefTestData { Value = entityX }, k_CommonComponents),
                EntityMatch.Exact(new EntityRefTestData { Value = entityY }, k_CommonComponents));
        }

        [Test]
        public void RedundantGameObjectToEntityConversionAreIgnored()
        {
            var go0 = CreateGameObject("level0", DestructionBy.Test);
            var go1 = CreateGameObject("level1", DestructionBy.Test).ParentTo(go0);

            go0.AddConvertAndDestroy();

            AwakeConversion(go0);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(k_CommonComponents),
                EntityMatch.Exact(k_ChildComponents));
        }

        [Test]
        public void StopGameObjectToEntityConversion()
        {
            var go0 = CreateGameObject("level0", DestructionBy.Test);
            var go1 = CreateGameObject("level1").ParentTo(go0);
            var go2 = CreateGameObject("level2").ParentTo(go1);

            go0.AddConvertAndDestroy();
            go1.AddStopConvert();
            go2.AddConvertAndDestroy();

            AwakeConversion(go0);

            LogAssert.Expect(LogType.Warning, new Regex("ConvertToEntity will be ignored because of a StopConvertToEntity higher in the hierarchy"));

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(k_CommonComponents));
        }

        [Test]
        public void ConvertAndInject_Simple()
        {
            var go0 = CreateGameObject("level0", DestructionBy.Test);
            var go1 = CreateGameObject("level1", DestructionBy.Test).ParentTo(go0);
            var go2 = CreateGameObject("level2", DestructionBy.Test).ParentTo(go1);

            go0.AddConvertAndInject();

            AwakeConversion(go0);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact(k_CommonComponents, typeof(Transform)));
        }

        [Test]
        public void ConvertAndInject_TwoWorlds()
        {
            var go = CreateGameObject(DestructionBy.Test);
            go.AddConvertAndInject();

            using (var world2 = new World("Test World 2"))
            {
                var convertToEntitySystem = BeginAwakeConversion(go);
                convertToEntitySystem.AddToBeConverted(world2, go.GetComponent<ConvertToEntity>());
                EndAwakeConversion();

                EntitiesAssert.ContainsOnly(m_Manager,
                    EntityMatch.Exact(k_CommonComponents, typeof(Transform)));
                EntitiesAssert.ContainsOnly(world2.EntityManager,
                    EntityMatch.Exact(k_CommonComponents, typeof(Transform)));
            }
        }

        [Test]
        public void ConvertAndInject_InDestroyHierarchy()
        {
            var a = CreateGameObject("a", DestructionBy.Test);
            var aa = CreateGameObject("aa").ParentTo(a);
            var aaa = CreateGameObject("aaa").ParentTo(aa);
            var aab = CreateGameObject("aab").ParentTo(aa);
            var ab = CreateGameObject("ab").ParentTo(a);
            var aba = CreateGameObject("aba").ParentTo(ab);
            var abb = CreateGameObject("abb").ParentTo(ab);

            a.AddConvertAndDestroy();
            aa.AddConvertAndInject();
            ab.AddConvertAndInject();

            AwakeConversion(a);

            EntitiesAssert.ContainsOnly(m_Manager,
                EntityMatch.Exact<Transform>(k_ChildComponents),
                EntityMatch.Exact<Transform>(k_ChildComponents),
                EntityMatch.Exact(k_CommonComponents));
        }
        
        //@TODO: ConvertToEntity w/ multiple Worlds registered via ConvertToEntitySystem.AddToBeConverted()
    }
}
