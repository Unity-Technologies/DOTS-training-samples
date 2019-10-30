using System;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Tests
{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    class EcsTestMonoBehaviourComponent : MonoBehaviour
    {
        public int SomeValue;
    }
#pragma warning restore CS0649
}

#if !UNITY_DISABLE_MANAGED_COMPONENTS
namespace Unity.Entities.Tests.Conversion
{
    class ConversionHybridTests : ConversionTestFixtureBase
    {
        public class MonoBehaviourComponentConversionSystem : GameObjectConversionSystem
        {
            protected override void OnUpdate()
            {
                Entities.ForEach((EcsTestMonoBehaviourComponent component) =>
                {
                    AddHybridComponent(component);
                });
            }
        }

        [Test]
        public void ManagedComponentSimple()
        {
            var gameObject = CreateGameObject();
            gameObject.AddComponent<EcsTestMonoBehaviourComponent>().SomeValue = 123;

            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, MakeDefaultSettings().WithExtraSystem<MonoBehaviourComponentConversionSystem>());

            gameObject.GetComponent<EcsTestMonoBehaviourComponent>().SomeValue = 234;
            Assert.AreEqual(123, m_Manager.GetComponentObject<EcsTestMonoBehaviourComponent>(entity).SomeValue);

            var instance = m_Manager.Instantiate(entity);

            m_Manager.GetComponentObject<EcsTestMonoBehaviourComponent>(entity).SomeValue = 345;
            Assert.AreEqual(123, m_Manager.GetComponentObject<EcsTestMonoBehaviourComponent>(instance).SomeValue);

            var instances = new NativeArray<Entity>(2, Allocator.Temp);
            m_Manager.Instantiate(entity, instances);

            Assert.AreEqual(345, m_Manager.GetComponentObject<EcsTestMonoBehaviourComponent>(instances[0]).SomeValue);
            Assert.AreEqual(345, m_Manager.GetComponentObject<EcsTestMonoBehaviourComponent>(instances[1]).SomeValue);
        }
    }
}
#endif
