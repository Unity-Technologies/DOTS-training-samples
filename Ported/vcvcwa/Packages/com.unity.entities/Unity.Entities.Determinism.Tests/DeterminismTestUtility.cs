using System;
using System.Text;
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace Unity.Entities.Determinism.Tests
{
    internal static class DeterminismTestUtility
    {
        public static NativeArray<byte> GenerateData(int length, Func<int, byte> generator)
        {
            var data = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < length; i++)
            {
                data[i] = generator(i);
            }
            return data;
        }

        const byte kMaskedByte = 0xff;
        public static NativeArray<byte> GenerateMask(int length, Func<int, bool> generator)
        {
            var data = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < length; i++)
            {
                data[i] = generator(i) ? kMaskedByte : default;
            }
            return data;
        }

        public static NativeArray<byte> GenerateIdentity(int length) => GenerateData(length, i => (byte)i);

        public static World CreateTestWorld_WithLocalToWorldComponents(string name, int entityCount)
        {
            var world = new World(name);

            var manager = world.EntityManager;
            var archetype = manager.CreateArchetype(ComponentType.ReadWrite<LocalToWorld>());

            // might exceed size of temp/local allocator, thus using persistent here
            using (var entities = new NativeArray<Entity>(entityCount, Allocator.Persistent))
            {
                manager.CreateEntity(archetype, entities);

                var ltw = new LocalToWorld { Value = float4x4.identity };
                foreach (var entity in entities)
                {
                    manager.SetComponentData(entity, ltw);
                }
            }

            return world;
        }
        
        public static string PrintAll<T>(NativeArray<T> actual, NativeArray<T> expected) where T : struct
        {
            var sb = new StringBuilder();

            void Print(NativeArray<T> array, string q )
            {
                sb.Append($"{q} [Length={expected.Length.ToString().PadLeft(3)}] with < ");
                foreach (var entry in array)
                {
                    sb.Append($"{entry} ");
                }
                sb.AppendLine(">");
            }

            Print(expected, "Expected:");
            Print(actual,   "But was:  ");
            return sb.ToString();
        }
    }

    [TestFixture]
    public class TestGenerator_Tests
    {
        [Test]
        public void TestUtility_GenerateIdentityArray_HoldsExceptedValues()
        {
            using (var array = DeterminismTestUtility.GenerateIdentity(3))
            {
                Assert.AreEqual(3, array.Length);

                Assert.AreEqual(0, array[0]);
                Assert.AreEqual(1, array[1]);
                Assert.AreEqual(2, array[2]);
            }
        }
    }
}
