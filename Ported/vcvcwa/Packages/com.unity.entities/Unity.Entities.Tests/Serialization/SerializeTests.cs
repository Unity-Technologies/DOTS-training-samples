using NUnit.Framework;
using Unity.Collections;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.Serialization;
using Unity.Jobs;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using BinaryReader = Unity.Entities.Serialization.BinaryReader;
using BinaryWriter = Unity.Entities.Serialization.BinaryWriter;

namespace Unity.Entities.Tests
{
    public unsafe struct TestBinaryReader : BinaryReader
    {
        NativeList<byte> content;
        int position;
        public TestBinaryReader(TestBinaryWriter writer)
        {
            position = 0;
            content = writer.content;
            writer.content = new NativeList<byte>();

        }

        public void Dispose()
        {
            content.Dispose();
        }

        public void ReadBytes(void* data, int bytes)
        {
            UnsafeUtility.MemCpy(data, (byte*)content.GetUnsafePtr() + position, bytes);
            position += bytes;
        }
    }

    public unsafe class TestBinaryWriter : BinaryWriter
    {
        internal NativeList<byte> content = new NativeList<byte>(Allocator.TempJob);

        public void Dispose()
        {
            content.Dispose();
        }

        public void WriteBytes(void* data, int bytes)
        {
            int length = content.Length;
            content.Resize(length + bytes, NativeArrayOptions.UninitializedMemory);
            UnsafeUtility.MemCpy((byte*)content.GetUnsafePtr() + length, data, bytes);
        }

    }

    struct DeserializeJob : IJob
    {
        public ExclusiveEntityTransaction Transaction;
        public TestBinaryReader Reader { get; set; }

        public void Execute()
        {
            SerializeUtility.DeserializeWorld(Transaction, Reader); 
        }
    }

    internal class YAMLSerializationHelpers
    {
        /// <summary>
        /// Compare two YAML file for equality, ignore CRLF mismatch
        /// </summary>
        /// <param name="fileA">Stream of the first file to compare</param>
        /// <param name="fileB">Stream of the second file to compare</param>
        /// <returns>true if both file are equal, false otherwise</returns>
        /// <remarks>
        /// This method start reading from both streams from their CURRENT position
        /// </remarks>
        public static bool EqualYAMLFiles(Stream fileA, Stream fileB)
        {
            using (var readerA = new StreamReader(fileA)) 
            using (var readerB = new StreamReader(fileB))
            {
                string lineA;
                while ((lineA = readerA.ReadLine()) != null)
                {
                    var lineB = readerB.ReadLine();
                    if (string.Compare(lineA, lineB, StringComparison.Ordinal) != 0)
                    {
                        return false;
                    }
                }
                return readerB.ReadLine() == null;
            }
        }
    }

    class SerializeTests : ECSTestsFixture
    {
        public struct TestComponentData1 : IComponentData
        {
            public int value;
            public Entity referencedEntity;
        }

        public struct TestComponentData2 : IComponentData
        {
            public int value;
            public Entity referencedEntity;
        }

        [InternalBufferCapacity(16)]
        public struct TestBufferElement : IBufferElementData
        {
            public Entity entity;
            public int value;
        }


        [Test]
        public void SerializeIntoExistingWorldThrows()
        {
            m_Manager.CreateEntity(typeof(EcsTestData));

            // disposed via reader
            var writer = new TestBinaryWriter();
            SerializeUtility.SerializeWorld(m_Manager, writer);

            var reader = new TestBinaryReader(writer);

            Assert.Throws<ArgumentException>(()=>
                SerializeUtility.DeserializeWorld(m_Manager.BeginExclusiveEntityTransaction(), reader)
            );
            reader.Dispose();
        }

        [Test]
        public unsafe void SerializeEntities()
        {
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset
            var e1 = CreateEntityWithDefaultData(1);
            var e2 = CreateEntityWithDefaultData(2);
            var e3 = CreateEntityWithDefaultData(3);
            m_Manager.AddComponentData(e1, new TestComponentData1{ value = 10, referencedEntity = e2 });
            m_Manager.AddComponentData(e2, new TestComponentData2{ value = 20, referencedEntity = e1 });
            m_Manager.AddComponentData(e3, new TestComponentData1{ value = 30, referencedEntity = Entity.Null });
            m_Manager.AddComponentData(e3, new TestComponentData2{ value = 40, referencedEntity = Entity.Null });
            m_Manager.AddBuffer<EcsIntElement>(e1);
            m_Manager.RemoveComponent<EcsTestData2>(e3);
            m_Manager.AddBuffer<EcsIntElement>(e3);

            m_Manager.GetBuffer<EcsIntElement>(e1).CopyFrom(new EcsIntElement[] { 1, 2, 3 }); // no overflow
            m_Manager.GetBuffer<EcsIntElement>(e3).CopyFrom(new EcsIntElement[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }); // overflow into heap

            var e4 = m_Manager.CreateEntity();
            m_Manager.AddBuffer<EcsComplexEntityRefElement>(e4);
            var ebuf = m_Manager.GetBuffer<EcsComplexEntityRefElement>(e4);
            ebuf.Add(new EcsComplexEntityRefElement { Entity = e1, Dummy = 1 });
            ebuf.Add(new EcsComplexEntityRefElement { Entity = e2, Dummy = 2 });
            ebuf.Add(new EcsComplexEntityRefElement { Entity = e3, Dummy = 3 });

            m_Manager.DestroyEntity(dummyEntity);
            // disposed via reader
            var writer = new TestBinaryWriter();

            SerializeUtility.SerializeWorld(m_Manager, writer);
            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntities Test World 3");
            var entityManager = deserializedWorld.EntityManager;

            SerializeUtility.DeserializeWorld(entityManager.BeginExclusiveEntityTransaction(), reader);
            entityManager.EndExclusiveEntityTransaction();

            try
            {
                var allEntities = entityManager.GetAllEntities(Allocator.Temp);
                var count = allEntities.Length;
                allEntities.Dispose();

                Assert.AreEqual(4, count);

                var group1 = entityManager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2),
                    typeof(TestComponentData1));
                var group2 = entityManager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2),
                    typeof(TestComponentData2));
                var group3 = entityManager.CreateEntityQuery(typeof(EcsTestData),
                    typeof(TestComponentData1), typeof(TestComponentData2));
                var group4 = entityManager.CreateEntityQuery(typeof(EcsComplexEntityRefElement));

                Assert.AreEqual(1, group1.CalculateEntityCount());
                Assert.AreEqual(1, group2.CalculateEntityCount());
                Assert.AreEqual(1, group3.CalculateEntityCount());
                Assert.AreEqual(1, group4.CalculateEntityCount());

                var everythingGroup = entityManager.CreateEntityQuery(Array.Empty<ComponentType>());
                var chunks = everythingGroup.CreateArchetypeChunkArray(Allocator.TempJob);
                Assert.AreEqual(4, chunks.Length);
                everythingGroup.Dispose();

                var entityType = entityManager.GetArchetypeChunkEntityType();
                Assert.AreEqual(1, chunks[0].GetNativeArray(entityType).Length);
                Assert.AreEqual(1, chunks[1].GetNativeArray(entityType).Length);
                Assert.AreEqual(1, chunks[2].GetNativeArray(entityType).Length);
                Assert.AreEqual(1, chunks[3].GetNativeArray(entityType).Length);
                chunks.Dispose();

                var entities1 = group1.ToEntityArray(Allocator.TempJob);
                var entities2 = group2.ToEntityArray(Allocator.TempJob);
                var entities3 = group3.ToEntityArray(Allocator.TempJob);
                var entities4 = group4.ToEntityArray(Allocator.TempJob);

                var new_e1 = entities1[0];
                var new_e2 = entities2[0];
                var new_e3 = entities3[0];
                var new_e4 = entities4[0];

                entities1.Dispose();
                entities2.Dispose();
                entities3.Dispose();
                entities4.Dispose();

                Assert.AreEqual(1, entityManager.GetComponentData<EcsTestData>(new_e1).value);
                Assert.AreEqual(-1, entityManager.GetComponentData<EcsTestData2>(new_e1).value0);
                Assert.AreEqual(-1, entityManager.GetComponentData<EcsTestData2>(new_e1).value1);
                Assert.AreEqual(10, entityManager.GetComponentData<TestComponentData1>(new_e1).value);

                Assert.AreEqual(2, entityManager.GetComponentData<EcsTestData>(new_e2).value);
                Assert.AreEqual(-2, entityManager.GetComponentData<EcsTestData2>(new_e2).value0);
                Assert.AreEqual(-2, entityManager.GetComponentData<EcsTestData2>(new_e2).value1);
                Assert.AreEqual(20, entityManager.GetComponentData<TestComponentData2>(new_e2).value);

                Assert.AreEqual(3, entityManager.GetComponentData<EcsTestData>(new_e3).value);
                Assert.AreEqual(30, entityManager.GetComponentData<TestComponentData1>(new_e3).value);
                Assert.AreEqual(40, entityManager.GetComponentData<TestComponentData2>(new_e3).value);

                Assert.IsTrue(entityManager.Exists(entityManager.GetComponentData<TestComponentData1>(new_e1).referencedEntity));
                Assert.IsTrue(entityManager.Exists(entityManager.GetComponentData<TestComponentData2>(new_e2).referencedEntity));
                Assert.AreEqual(new_e2 , entityManager.GetComponentData<TestComponentData1>(new_e1).referencedEntity);
                Assert.AreEqual(new_e1 , entityManager.GetComponentData<TestComponentData2>(new_e2).referencedEntity);

                var buf1 = entityManager.GetBuffer<EcsIntElement>(new_e1);
                Assert.AreEqual(3, buf1.Length);
                Assert.AreNotEqual((UIntPtr)m_Manager.GetBuffer<EcsIntElement>(e1).GetUnsafePtr(), (UIntPtr)buf1.GetUnsafePtr());

                for (int i = 0; i < 3; ++i)
                {
                    Assert.AreEqual(i + 1, buf1[i].Value);
                }

                var buf3 = entityManager.GetBuffer<EcsIntElement>(new_e3);
                Assert.AreEqual(10, buf3.Length);
                Assert.AreNotEqual((UIntPtr)m_Manager.GetBuffer<EcsIntElement>(e3).GetUnsafePtr(), (UIntPtr)buf3.GetUnsafePtr());

                for (int i = 0; i < 10; ++i)
                {
                    Assert.AreEqual(i + 1, buf3[i].Value);
                }

                var buf4 = entityManager.GetBuffer<EcsComplexEntityRefElement>(new_e4);
                Assert.AreEqual(3, buf4.Length);

                Assert.AreEqual(1, buf4[0].Dummy);
                Assert.AreEqual(new_e1, buf4[0].Entity);

                Assert.AreEqual(2, buf4[1].Dummy);
                Assert.AreEqual(new_e2, buf4[1].Entity);

                Assert.AreEqual(3, buf4[2].Dummy);
                Assert.AreEqual(new_e3, buf4[2].Entity);
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }
        }

        //测试

        public struct 测试 : IComponentData
        {
            public int value;
        }

        [Test]
        public void SerializeEntitiesSupportsNonASCIIComponentTypeNames()
        {
            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new 测试{ value = 7 });

            // disposed via reader
            var writer = new TestBinaryWriter();
            SerializeUtility.SerializeWorld(m_Manager, writer);
            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntitiesSupportsNonASCIIComponentTypeNames Test World");
            var entityManager = deserializedWorld.EntityManager;

            SerializeUtility.DeserializeWorld(entityManager.BeginExclusiveEntityTransaction(), reader);
            entityManager.EndExclusiveEntityTransaction();

            try
            {
                var allEntities = entityManager.GetAllEntities(Allocator.Temp);
                var count = allEntities.Length;
                allEntities.Dispose();

                Assert.AreEqual(1, count);

                var group1 = entityManager.CreateEntityQuery(typeof(测试));

                Assert.AreEqual(1, group1.CalculateEntityCount());

                var entities = group1.ToEntityArray(Allocator.TempJob);
                var new_e1 = entities[0];
                entities.Dispose();

                Assert.AreEqual(7, entityManager.GetComponentData<测试>(new_e1).value);
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }

        }

        [Test]
        public unsafe void SerializeEntitiesRemapsEntitiesInBuffers()
        {
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset

            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new EcsTestData(1));
            var e2 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e2, new EcsTestData2(2));

            m_Manager.AddBuffer<TestBufferElement>(e1);
            var buffer1 = m_Manager.GetBuffer<TestBufferElement>(e1);
            for(int i=0;i<1024;++i)
                buffer1.Add(new TestBufferElement {entity = e2, value = 2});

            m_Manager.AddBuffer<TestBufferElement>(e2);
            var buffer2 = m_Manager.GetBuffer<TestBufferElement>(e2);
            for(int i=0;i<8;++i)
                buffer2.Add(new TestBufferElement {entity = e1, value = 1});

            m_Manager.DestroyEntity(dummyEntity);
            // disposed via reader
            var writer = new TestBinaryWriter();

            SerializeUtility.SerializeWorld(m_Manager, writer);
            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntities Test World 3");
            var entityManager = deserializedWorld.EntityManager;

            SerializeUtility.DeserializeWorld(entityManager.BeginExclusiveEntityTransaction(), reader);
            entityManager.EndExclusiveEntityTransaction();

            try
            {

                var group1 = entityManager.CreateEntityQuery(typeof(EcsTestData), typeof(TestBufferElement));
                var group2 = entityManager.CreateEntityQuery(typeof(EcsTestData2), typeof(TestBufferElement));

                Assert.AreEqual(1, group1.CalculateEntityCount());
                Assert.AreEqual(1, group2.CalculateEntityCount());

                var entities1 = group1.ToEntityArray(Allocator.TempJob);
                var entities2 = group2.ToEntityArray(Allocator.TempJob);

                var new_e1 = entities1[0];
                var new_e2 = entities2[0];

                entities1.Dispose();
                entities2.Dispose();

                var newBuffer1 = entityManager.GetBuffer<TestBufferElement>(new_e1);
                Assert.AreEqual(1024, newBuffer1.Length);
                for (int i = 0; i < 1024; ++i)
                {
                    Assert.AreEqual(new_e2, newBuffer1[i].entity);
                    Assert.AreEqual(2, newBuffer1[i].value);
                }

                var newBuffer2 = entityManager.GetBuffer<TestBufferElement>(new_e2);
                Assert.AreEqual(8, newBuffer2.Length);
                for (int i = 0; i < 8; ++i)
                {
                    Assert.AreEqual(new_e1, newBuffer2[i].entity);
                    Assert.AreEqual(1, newBuffer2[i].value);
                }
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }
        }

        [Test]
        public unsafe void SerializeEntitiesWorksWithChunkComponents()
        {
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset

            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new EcsTestData(1));
            m_Manager.AddChunkComponentData<EcsTestData3>(e1);
            m_Manager.SetChunkComponentData(m_Manager.GetChunk(e1), new EcsTestData3(42));
            var e2 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e2, new EcsTestData2(2));
            m_Manager.AddChunkComponentData<EcsTestData3>(e2);
            m_Manager.SetChunkComponentData(m_Manager.GetChunk(e2), new EcsTestData3(57));

            m_Manager.DestroyEntity(dummyEntity);
            // disposed via reader
            var writer = new TestBinaryWriter();

            SerializeUtility.SerializeWorld(m_Manager, writer);
            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntities Test World 3");
            var entityManager = deserializedWorld.EntityManager;

            SerializeUtility.DeserializeWorld(entityManager.BeginExclusiveEntityTransaction(), reader);
            entityManager.EndExclusiveEntityTransaction();

            try
            {
                var group1 = entityManager.CreateEntityQuery(typeof(EcsTestData));
                var group2 = entityManager.CreateEntityQuery(typeof(EcsTestData2));

                Assert.AreEqual(1, group1.CalculateEntityCount());
                Assert.AreEqual(1, group2.CalculateEntityCount());

                var entities1 = group1.ToEntityArray(Allocator.TempJob);
                var entities2 = group2.ToEntityArray(Allocator.TempJob);

                var new_e1 = entities1[0];
                var new_e2 = entities2[0];

                entities1.Dispose();
                entities2.Dispose();

                Assert.AreEqual(1, entityManager.GetComponentData<EcsTestData>(new_e1).value);
                Assert.AreEqual(42, entityManager.GetChunkComponentData<EcsTestData3>(new_e1).value0);

                Assert.AreEqual(2, entityManager.GetComponentData<EcsTestData2>(new_e2).value0);
                Assert.AreEqual(57, entityManager.GetChunkComponentData<EcsTestData3>(new_e2).value0);
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }
        }

        [Test]
        public void SerializeDoesntRemapOriginalHeapBuffers()
        {
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset

            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new EcsTestData(1));
            var e2 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e2, new EcsTestData2(2));

            m_Manager.AddBuffer<TestBufferElement>(e1);
            var buffer1 = m_Manager.GetBuffer<TestBufferElement>(e1);
            for(int i=0;i<1024;++i)
                buffer1.Add(new TestBufferElement {entity = e2, value = 2});

            m_Manager.AddBuffer<TestBufferElement>(e2);
            var buffer2 = m_Manager.GetBuffer<TestBufferElement>(e2);
            for(int i=0;i<8;++i)
                buffer2.Add(new TestBufferElement {entity = e1, value = 1});

            m_Manager.DestroyEntity(dummyEntity);
            // disposed via reader
            var writer = new TestBinaryWriter();

            SerializeUtility.SerializeWorld(m_Manager, writer);

            buffer1 = m_Manager.GetBuffer<TestBufferElement>(e1);
            for (int i = 0; i < buffer1.Length; ++i)
            {
                Assert.AreEqual(e2, buffer1[i].entity);
                Assert.AreEqual(2, buffer1[i].value);
            }

            buffer2 = m_Manager.GetBuffer<TestBufferElement>(e2);
            for (int i = 0; i < buffer2.Length; ++i)
            {
                Assert.AreEqual(e1, buffer2[i].entity);
                Assert.AreEqual(1, buffer2[i].value);
            }

            writer.Dispose();
        }

        struct ExternalSharedComponentValue
        {
            public object obj;
            public int hashcode;
            public int typeIndex;
        }

        ExternalSharedComponentValue[] ExtractSharedComponentValues(int[] indices, EntityManager manager)
        {
            var values = new ExternalSharedComponentValue[indices.Length];
            for (int i = 0; i < indices.Length; ++i)
            {
                object value = manager.ManagedComponentStore.GetSharedComponentDataNonDefaultBoxed(indices[i]);
                int typeIndex = TypeManager.GetTypeIndex(value.GetType());
                int hash = TypeManager.GetHashCode(value, typeIndex);
                values[i] = new ExternalSharedComponentValue {obj = value, hashcode = hash, typeIndex = typeIndex};
            }
            return values;
        }

        void InsertSharedComponentValues(ExternalSharedComponentValue[] values, EntityManager manager)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                ExternalSharedComponentValue value = values[i];
                int index = manager.ManagedComponentStore.InsertSharedComponentAssumeNonDefault(value.typeIndex,
                    value.hashcode, value.obj);
                Assert.AreEqual(i+1, index);
            }
        }

        [Test]
        [StandaloneFixme] // Unity.Properties support required
        public unsafe void SerializeEntitiesWorksWithBlobAssetReferences()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestSharedComp), typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestSharedComp2), typeof(EcsTestData2));
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset

            var builder = new BlobBuilder(Allocator.Temp);
            ref var blobArray = ref builder.ConstructRoot<BlobArray<float>>();
            var array = builder.Allocate(ref blobArray, 5);
            array[0] = 1.7f;
            array[1] = 2.6f;
            array[2] = 3.5f;
            array[3] = 4.4f;
            array[4] = 5.3f;

            var arrayComponent = new EcsTestDataBlobAssetArray { array = builder.CreateBlobAssetReference<BlobArray<float>>(Allocator.Temp) };
            builder.Dispose();

            const int entityCount = 1000;
            var entities = new NativeArray<Entity>(entityCount, Allocator.Temp);

            m_Manager.CreateEntity(archetype1, entities);
            for (int i = 0; i < entityCount; ++i)
            {
                m_Manager.AddComponentData(entities[i], arrayComponent);
                m_Manager.SetSharedComponentData(entities[i], new EcsTestSharedComp(i % 4));
            }

            var intComponents = new NativeArray<EcsTestDataBlobAssetRef>(entityCount / 5, Allocator.Temp);
            for (int i = 0; i < intComponents.Length; ++i)
                intComponents[i] = new EcsTestDataBlobAssetRef { value = BlobAssetReference<int>.Create(i) };


            m_Manager.CreateEntity(archetype2, entities);
            for (int i = 0; i < entityCount; ++i)
            {
                var intComponent = intComponents[i % intComponents.Length];
                m_Manager.AddComponentData(entities[i], intComponent);
                m_Manager.SetComponentData(entities[i], new EcsTestData2(intComponent.value.Value));
                m_Manager.SetSharedComponentData(entities[i], new EcsTestSharedComp2(i % 3));

                m_Manager.AddBuffer<EcsTestDataBlobAssetElement>(entities[i]);
                var buffer = m_Manager.GetBuffer<EcsTestDataBlobAssetElement>(entities[i]);
                int numBlobs = i % 100;
                buffer.Reserve(numBlobs);
                for (int j = 0; j < numBlobs; ++j)
                {
                    buffer.Add(new EcsTestDataBlobAssetElement() { blobElement = BlobAssetReference<int>.Create(j) });
                }

                m_Manager.AddBuffer<EcsTestDataBlobAssetElement2>(entities[i]);
                var buffer2 = m_Manager.GetBuffer<EcsTestDataBlobAssetElement2>(entities[i]);
                buffer2.Reserve(numBlobs);
                for (int j = 0; j < numBlobs; ++j)
                {
                    buffer2.Add(new EcsTestDataBlobAssetElement2()
                    {
                        blobElement = BlobAssetReference<int>.Create(j+1),
                        blobElement2 = BlobAssetReference<int>.Create(j+2)
                    });
                }
            }

            m_Manager.DestroyEntity(dummyEntity);

            // disposed via reader
            var writer = new TestBinaryWriter();

            SerializeUtility.SerializeWorld(m_Manager, writer);

            m_Manager.DestroyEntity(m_Manager.UniversalQuery);

            arrayComponent.array.Release();
            for (int i = 0; i < intComponents.Length; ++i)
                intComponents[i].value.Release();

            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntities Test World 3");
            var entityManager = deserializedWorld.EntityManager;

            var job = new DeserializeJob {Transaction = entityManager.BeginExclusiveEntityTransaction(), Reader = reader};
            job.Schedule().Complete();
            entityManager.EndExclusiveEntityTransaction();

            entityManager.Debug.CheckInternalConsistency();
            Assert.IsTrue(entityManager.ManagedComponentStore.AllSharedComponentReferencesAreFromChunks(entityManager.EntityComponentStore));

            try
            {
                var group1 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetArray));
                var group2 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetRef));
                var group3 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetElement));
                var group4 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetElement2));

                var entities1 = group1.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities1.Length);
                var new_e1 = entities1[0];
                arrayComponent = entityManager.GetComponentData<EcsTestDataBlobAssetArray>(new_e1);
                var a = arrayComponent.array;
                Assert.AreEqual(1.7f, a.Value[0]);
                Assert.AreEqual(2.6f, a.Value[1]);
                Assert.AreEqual(3.5f, a.Value[2]);
                Assert.AreEqual(4.4f, a.Value[3]);
                Assert.AreEqual(5.3f, a.Value[4]);
                entities1.Dispose();

                var entities2 = group2.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities2.Length);
                for (int i = 0; i < entityCount; ++i)
                {
                    var val = entityManager.GetComponentData<EcsTestData2>(entities2[i]).value0;
                    Assert.AreEqual(val, entityManager.GetComponentData<EcsTestDataBlobAssetRef>(entities2[i]).value.Value);
                }
                entities2.Dispose();

                var entities3 = group3.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities3.Length);

                // We can't rely on the entity order matching how we filled the buffers so we instead ensure 
                // that we see buffers as many buffers as there are entities and we see buffers with 1 to 'entityCount'
                // elements in them with ascending values from 0 to bufferLength-1
                NativeHashMap<int, int> bufferMap = new NativeHashMap<int, int>(entityCount, Allocator.Temp);
                for (int i = 0; i < entityCount; ++i)
                {
                    var buffer = entityManager.GetBuffer<EcsTestDataBlobAssetElement>(entities3[i]);

                    for (int j = 0; j < buffer.Length; ++j)
                    {
                        Assert.AreEqual(j, buffer[j].blobElement.Value);
                    }
                    if(!bufferMap.TryGetValue(buffer.Length, out var count))
                    {
                        bufferMap.TryAdd(buffer.Length, 1);
                    }
                    else
                    {
                        bufferMap[buffer.Length] = count + 1;
                    }
                }
                for (int i = 0; i < entityCount; ++i)
                {
                    Assert.IsTrue(bufferMap[i%100] == 10);
                }
                bufferMap.Dispose();
                entities3.Dispose();


                var entities4 = group4.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities4.Length);

                NativeHashMap<int, int> bufferMap2 = new NativeHashMap<int, int>(entityCount, Allocator.Temp);
                for (int i = 0; i < entityCount; ++i)
                {
                    var buffer = entityManager.GetBuffer<EcsTestDataBlobAssetElement2>(entities4[i]);

                    for (int j = 0; j < buffer.Length; ++j)
                    {
                        Assert.AreEqual(j+1, buffer[j].blobElement.Value);
                        Assert.AreEqual(j+2, buffer[j].blobElement2.Value);
                    }
                    if (!bufferMap2.TryGetValue(buffer.Length, out var count))
                    {
                        bufferMap2.TryAdd(buffer.Length, 1);
                    }
                    else
                    {
                        bufferMap2[buffer.Length] = count + 1;
                    }
                }
                for (int i = 0; i < entityCount; ++i)
                {
                    Assert.IsTrue(bufferMap2[i % 100] == 10);
                }
                bufferMap2.Dispose();
                entities4.Dispose();
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }

#if !NET_DOTS // If memory has been unmapped this can throw exceptions other than InvalidOperation
            float f = 1.0f;
            Assert.Throws<InvalidOperationException>(() => f = arrayComponent.array.Value[0]);
#endif
        }

        // A test for DOTS Runtime to validate blob assets.
        [Test]
        public unsafe void SerializeEntitiesWorksWithBlobAssetReferencesNoSharedComponents()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData2));
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset

            var builder = new BlobBuilder(Allocator.Temp);
            ref var blobArray = ref builder.ConstructRoot<BlobArray<float>>();
            var array = builder.Allocate(ref blobArray, 5);
            array[0] = 1.7f;
            array[1] = 2.6f;
            array[2] = 3.5f;
            array[3] = 4.4f;
            array[4] = 5.3f;

            var arrayComponent = new EcsTestDataBlobAssetArray { array = builder.CreateBlobAssetReference<BlobArray<float>>(Allocator.Temp) };
            builder.Dispose();

            const int entityCount = 1000;
            var entities = new NativeArray<Entity>(entityCount, Allocator.Temp);

            m_Manager.CreateEntity(archetype1, entities);
            for (int i = 0; i < entityCount; ++i)
            {
                m_Manager.AddComponentData(entities[i], arrayComponent);
            }

            var intComponents = new NativeArray<EcsTestDataBlobAssetRef>(entityCount / 5, Allocator.Temp);
            for (int i = 0; i < intComponents.Length; ++i)
                intComponents[i] = new EcsTestDataBlobAssetRef { value = BlobAssetReference<int>.Create(i) };


            m_Manager.CreateEntity(archetype2, entities);
            for (int i = 0; i < entityCount; ++i)
            {
                var intComponent = intComponents[i % intComponents.Length];
                m_Manager.AddComponentData(entities[i], intComponent);
                m_Manager.SetComponentData(entities[i], new EcsTestData2(intComponent.value.Value));

                m_Manager.AddBuffer<EcsTestDataBlobAssetElement>(entities[i]);
                var buffer = m_Manager.GetBuffer<EcsTestDataBlobAssetElement>(entities[i]);
                int numBlobs = i % 100;
                buffer.Reserve(numBlobs);
                for (int j = 0; j < numBlobs; ++j)
                {
                    buffer.Add(new EcsTestDataBlobAssetElement() { blobElement = BlobAssetReference<int>.Create(j) });
                }

                m_Manager.AddBuffer<EcsTestDataBlobAssetElement2>(entities[i]);
                var buffer2 = m_Manager.GetBuffer<EcsTestDataBlobAssetElement2>(entities[i]);
                buffer2.Reserve(numBlobs);
                for (int j = 0; j < numBlobs; ++j)
                {
                    buffer2.Add(new EcsTestDataBlobAssetElement2()
                    {
                        blobElement = BlobAssetReference<int>.Create(j + 1),
                        blobElement2 = BlobAssetReference<int>.Create(j + 2)
                    });
                }
            }

            m_Manager.DestroyEntity(dummyEntity);

            // disposed via reader
            var writer = new TestBinaryWriter();

            SerializeUtility.SerializeWorld(m_Manager, writer);

            m_Manager.DestroyEntity(m_Manager.UniversalQuery);

            arrayComponent.array.Release();
            for (int i = 0; i < intComponents.Length; ++i)
                intComponents[i].value.Release();

            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntities Test World 3");
            var entityManager = deserializedWorld.EntityManager;

            var job = new DeserializeJob { Transaction = entityManager.BeginExclusiveEntityTransaction(), Reader = reader };
            job.Schedule().Complete();
            entityManager.EndExclusiveEntityTransaction();

            entityManager.Debug.CheckInternalConsistency();
            Assert.IsTrue(entityManager.ManagedComponentStore.AllSharedComponentReferencesAreFromChunks(entityManager.EntityComponentStore));

            try
            {
                var group1 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetArray));
                var group2 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetRef));
                var group3 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetElement));
                var group4 = entityManager.CreateEntityQuery(typeof(EcsTestDataBlobAssetElement2));

                var entities1 = group1.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities1.Length);
                var new_e1 = entities1[0];
                arrayComponent = entityManager.GetComponentData<EcsTestDataBlobAssetArray>(new_e1);
                var a = arrayComponent.array;
                Assert.AreEqual(1.7f, a.Value[0]);
                Assert.AreEqual(2.6f, a.Value[1]);
                Assert.AreEqual(3.5f, a.Value[2]);
                Assert.AreEqual(4.4f, a.Value[3]);
                Assert.AreEqual(5.3f, a.Value[4]);
                entities1.Dispose();

                var entities2 = group2.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities2.Length);
                for (int i = 0; i < entityCount; ++i)
                {
                    var val = entityManager.GetComponentData<EcsTestData2>(entities2[i]).value0;
                    Assert.AreEqual(val, entityManager.GetComponentData<EcsTestDataBlobAssetRef>(entities2[i]).value.Value);
                }
                entities2.Dispose();

                var entities3 = group3.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities3.Length);

                // We can't rely on the entity order matching how we filled the buffers so we instead ensure 
                // that we see buffers as many buffers as there are entities and we see buffers with 1 to 'entityCount'
                // elements in them with ascending values from 0 to bufferLength-1
                NativeHashMap<int, int> bufferMap = new NativeHashMap<int, int>(entityCount, Allocator.Temp);
                for (int i = 0; i < entityCount; ++i)
                {
                    var buffer = entityManager.GetBuffer<EcsTestDataBlobAssetElement>(entities3[i]);

                    for (int j = 0; j < buffer.Length; ++j)
                    {
                        Assert.AreEqual(j, buffer[j].blobElement.Value);
                    }
                    if (!bufferMap.TryGetValue(buffer.Length, out var count))
                    {
                        bufferMap.TryAdd(buffer.Length, 1);
                    }
                    else
                    {
                        bufferMap[buffer.Length] = count + 1;
                    }
                }
                for (int i = 0; i < entityCount; ++i)
                {
                    Assert.IsTrue(bufferMap[i % 100] == 10);
                }
                bufferMap.Dispose();
                entities3.Dispose();


                var entities4 = group4.ToEntityArray(Allocator.TempJob);
                Assert.AreEqual(entityCount, entities4.Length);

                NativeHashMap<int, int> bufferMap2 = new NativeHashMap<int, int>(entityCount, Allocator.Temp);
                for (int i = 0; i < entityCount; ++i)
                {
                    var buffer = entityManager.GetBuffer<EcsTestDataBlobAssetElement2>(entities4[i]);

                    for (int j = 0; j < buffer.Length; ++j)
                    {
                        Assert.AreEqual(j + 1, buffer[j].blobElement.Value);
                        Assert.AreEqual(j + 2, buffer[j].blobElement2.Value);
                    }
                    if (!bufferMap2.TryGetValue(buffer.Length, out var count))
                    {
                        bufferMap2.TryAdd(buffer.Length, 1);
                    }
                    else
                    {
                        bufferMap2[buffer.Length] = count + 1;
                    }
                }
                for (int i = 0; i < entityCount; ++i)
                {
                    Assert.IsTrue(bufferMap2[i % 100] == 10);
                }
                bufferMap2.Dispose();
                entities4.Dispose();
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }

#if !NET_DOTS // If memory has been unmapped this can throw exceptions other than InvalidOperation
            float f = 1.0f;
            Assert.Throws<InvalidOperationException>(() => f = arrayComponent.array.Value[0]);
#endif
        }

        public unsafe struct ComponentWithPointer : IComponentData
        {
            int m_Pad;
            byte* m_Data;
        }

#if !NET_DOTS // We don't have reflection to validate if a type is serializable in NET_DOTS
        [Test]
        public void SerializeComponentWithPointerField()
        {
            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new ComponentWithPointer());

            using (var writer = new TestBinaryWriter())
            {
                Assert.Throws<ArgumentException>(() =>
                    SerializeUtility.SerializeWorld(m_Manager, writer)
                );
            }
        }

        public struct ComponentWithIntPtr : IComponentData
        {
            int m_Pad;
            IntPtr m_Data;
        }

        [Test]
        public void SerializeComponentWithIntPtrField()
        {
            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new ComponentWithIntPtr());

            using (var writer = new TestBinaryWriter())
            {
                Assert.Throws<ArgumentException>(() =>
                    SerializeUtility.SerializeWorld(m_Manager, writer)
                );
            }
        }

        public unsafe struct TypeWithPointer
        {
            int m_Pad;
            byte* m_Data;
        }

        public struct TypeWithNestedPointer
        {
            int m_Pad;
            TypeWithPointer m_Data;
            int m_Pad1;
        }

        public struct ComponentWithNestedPointer : IComponentData
        {
            int m_Pad;
            TypeWithNestedPointer m_PointerField;
            int m_Pad1;
        }

        [Test]
        public void SerializeComponentWithNestedPointerField()
        {
            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new ComponentWithNestedPointer());

            using (var writer = new TestBinaryWriter())
            {
                Assert.Throws<ArgumentException>(() =>
                    SerializeUtility.SerializeWorld(m_Manager, writer)
                );
            }
        }

        public struct TypeWithNestedWhiteListType
        {
            int m_Pad;
            ChunkHeader m_Header; // whitelisted pointer type
            int m_Pad1;
        }

        public struct ComponentWithNestedPointerAndNestedWhiteListType : IComponentData
        {
            TypeWithNestedPointer m_PointerField;
            TypeWithNestedWhiteListType m_NestedWhitelistField;
        }

        [Test]
        public void EnsureSerializationWhitelistingDoesNotTrumpValidation()
        {
            var e1 = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e1, new ComponentWithNestedPointerAndNestedWhiteListType());

            using (var writer = new TestBinaryWriter())
            {
                Assert.Throws<ArgumentException>(() =>
                    SerializeUtility.SerializeWorld(m_Manager, writer)
                );
            }
        }
#endif

        [Test]
        public void DeserializedChunksAreConsideredChangedOnlyOnce()
        {
            TestBinaryReader CreateSerializedData()
            {
                var world = new World("DeserializedChunksAreConsideredChangedOnlyOnce World");
                var manager = world.EntityManager;
                var entity = manager.CreateEntity();
                manager.AddComponentData(entity, new EcsTestData(42));

                // owned by caller via reader
                var writer = new TestBinaryWriter();
                SerializeUtility.SerializeWorld(manager, writer, out var objRefs);
                world.Dispose();
                return new TestBinaryReader(writer);
            }

            var reader = CreateSerializedData();

            var deserializedWorld = new World("DeserializedChunksAreConsideredChangedOnlyOnce World 2");
            var deserializedManager = deserializedWorld.EntityManager;
            var system = deserializedWorld.GetOrCreateSystem<TestEcsChangeSystem>();

            Assert.AreEqual(0, system.NumChanged);

            SerializeUtility.DeserializeWorld(deserializedManager.BeginExclusiveEntityTransaction(), reader);
            deserializedManager.EndExclusiveEntityTransaction();
            reader.Dispose();

            system.Update();
            Assert.AreEqual(1, system.NumChanged);

            system.Update();
            Assert.AreEqual(0, system.NumChanged);

            deserializedWorld.Dispose();
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        public class ManagedComponent : IComponentData
        {
            public string String;
            public Dictionary<string, int> Map;
        }

        public class ManagedComponent2 : IComponentData
        {
            public List<string> List;
        }

        public class MyClass
        {
            public uint4 mInt4;
            public int mId;
            public int[] mArray;

            public MyClass()
            {
                mInt4 = new uint4();
                mId = 0;
                mArray = null;
            }

            public MyClass(int v)
            {
                mInt4 = new uint4(v + 1);
                mId = v;
                mArray = new int[16];
                for (int i = 0; i < 16; ++i)
                    mArray[i] = v + i;
            }

            public int GetId() { return mId; }
        }

        public class ManagedComponentCustomClass : IComponentData
        {
            public MyClass mClass;

            public ManagedComponentCustomClass()
            {
                mClass = null;
            }

            public ManagedComponentCustomClass(MyClass c)
            {
                mClass = c;
            }
        }

        [Test]
        [StandaloneFixme] // DOTS Runtime Managed Component Serialization
        public void SerializeEntities_ManagedComponents()
        {
            int expectedEntityCount = 1000;
            for (int i = 0; i < expectedEntityCount; ++i)
            {
                var e1 = m_Manager.CreateEntity();

                var expectedManagedComponent = new ManagedComponent { String = i.ToString(), Map = new Dictionary<string, int>() };
                expectedManagedComponent.Map.Add("positive", i);
                expectedManagedComponent.Map.Add("negative", -i);

                var expectedManagedComponent2 = new ManagedComponent2() { List = new List<string>() };
                expectedManagedComponent2.List.Add("one");
                expectedManagedComponent2.List.Add("two");
                expectedManagedComponent2.List.Add("three");
                expectedManagedComponent2.List.Add("four");

                m_Manager.AddComponentData(e1, expectedManagedComponent);
                m_Manager.AddComponentData(e1, expectedManagedComponent2);
            }

            // disposed via reader
            var writer = new TestBinaryWriter();
            SerializeUtility.SerializeWorld(m_Manager, writer);
            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntities_ManagedComponents Test World");
            var entityManager = deserializedWorld.EntityManager;

            SerializeUtility.DeserializeWorld(entityManager.BeginExclusiveEntityTransaction(), reader);
            entityManager.EndExclusiveEntityTransaction();

            try
            {
                var allEntities = entityManager.GetAllEntities(Allocator.Temp);
                var actualEntityCount = allEntities.Length;
                allEntities.Dispose();

                Assert.AreEqual(expectedEntityCount, actualEntityCount);

                var group1 = entityManager.CreateEntityQuery(typeof(ManagedComponent));

                Assert.AreEqual(actualEntityCount, group1.CalculateEntityCount());

                using (var entities = group1.ToEntityArray(Allocator.TempJob))
                {
                    for (int i = 0; i < entities.Length; ++i)
                    {
                        var e = entities[i];

                        var actualManagedComponent = entityManager.GetComponentData<ManagedComponent>(e);
                        Assert.AreEqual(i.ToString(), actualManagedComponent.String);
                        Assert.AreEqual(2, actualManagedComponent.Map.Count);
                        Assert.AreEqual(i, actualManagedComponent.Map["positive"]);
                        Assert.AreEqual(-i, actualManagedComponent.Map["negative"]);

                        var actualManagedComponent2 = entityManager.GetComponentData<ManagedComponent2>(e);
                        Assert.AreEqual(4, actualManagedComponent2.List.Count);
                        Assert.AreEqual("one", actualManagedComponent2.List[0]);
                        Assert.AreEqual("two", actualManagedComponent2.List[1]);
                        Assert.AreEqual("three", actualManagedComponent2.List[2]);
                        Assert.AreEqual("four", actualManagedComponent2.List[3]);
                    }
                }
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }
        }

        [Test]
        [StandaloneFixme] // DOTS Runtime Managed Component Serialization
        public void SerializeEntitiesManagedComponentWithCustomClass_ManagedComponents()
        {
            int expectedEntityCount = 1000;
            for (int i = 0; i < expectedEntityCount; ++i)
            {
                var e1 = m_Manager.CreateEntity();

                var expectedManagedComponent = new ManagedComponentCustomClass(new MyClass(i));
                m_Manager.AddComponentData(e1, expectedManagedComponent);
            }

            // disposed via reader
            var writer = new TestBinaryWriter();
            SerializeUtility.SerializeWorld(m_Manager, writer);
            var reader = new TestBinaryReader(writer);

            var deserializedWorld = new World("SerializeEntitiesManagedComponentWithCustomClass_ManagedComponents Test World");
            var entityManager = deserializedWorld.EntityManager;

            SerializeUtility.DeserializeWorld(entityManager.BeginExclusiveEntityTransaction(), reader);
            entityManager.EndExclusiveEntityTransaction();

            try
            {
                var allEntities = entityManager.GetAllEntities(Allocator.Temp);
                var actualEntityCount = allEntities.Length;
                allEntities.Dispose();

                Assert.AreEqual(expectedEntityCount, actualEntityCount);

                var group1 = entityManager.CreateEntityQuery(typeof(ManagedComponentCustomClass));

                Assert.AreEqual(actualEntityCount, group1.CalculateEntityCount());

                using (var entities = group1.ToEntityArray(Allocator.TempJob))
                {
                    NativeHashMap<int, int> idSet = new NativeHashMap<int, int>(entities.Length, Allocator.Temp);
                    for (int i = 0; i < entities.Length; ++i)
                    {
                        var e = entities[i];

                        var actualManagedComponent = entityManager.GetComponentData<ManagedComponentCustomClass>(e);
                        var myClass = actualManagedComponent.mClass;
                        Assert.NotNull(actualManagedComponent);
                        Assert.NotNull(myClass);
                        int id = actualManagedComponent.mClass.GetId();
                        Assert.IsTrue(idSet.TryAdd(id, id));

                        Assert.IsTrue(myClass.mInt4.Equals(new uint4(id + 1)));
                        for (int j = 0; j < 16; ++j)
                        {
                            Assert.IsTrue(myClass.mArray[j] == id + j);
                        }
                    }
                    idSet.Dispose();
                }
            }
            finally
            {
                deserializedWorld.Dispose();
                reader.Dispose();
            }
        }

#if !NET_DOTS
        [Test]
        public void WorldYamlSerializationTest()
        {
            var dummyEntity = CreateEntityWithDefaultData(0); //To ensure entity indices are offset
            var e1 = CreateEntityWithDefaultData(1);
            var e2 = CreateEntityWithDefaultData(2);
            var e3 = CreateEntityWithDefaultData(3);
            m_Manager.AddComponentData(e1, new TestComponentData1 { value = 10, referencedEntity = e2 });
            m_Manager.AddComponentData(e2, new TestComponentData2 { value = 20, referencedEntity = e1 });
            m_Manager.AddComponentData(e3, new TestComponentData1 { value = 30, referencedEntity = Entity.Null });
            m_Manager.AddComponentData(e3, new TestComponentData2 { value = 40, referencedEntity = Entity.Null });
            m_Manager.AddBuffer<EcsIntElement>(e1);
            m_Manager.RemoveComponent<EcsTestData2>(e3);
            m_Manager.AddBuffer<EcsIntElement>(e3);

            m_Manager.GetBuffer<EcsIntElement>(e1).CopyFrom(new EcsIntElement[] { 1, 2, 3 }); // no overflow
            m_Manager.GetBuffer<EcsIntElement>(e3).CopyFrom(new EcsIntElement[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }); // overflow into heap

            var e4 = m_Manager.CreateEntity();
            m_Manager.AddBuffer<EcsComplexEntityRefElement>(e4);
            var ebuf = m_Manager.GetBuffer<EcsComplexEntityRefElement>(e4);
            ebuf.Add(new EcsComplexEntityRefElement { Entity = e1, Dummy = 1 });
            ebuf.Add(new EcsComplexEntityRefElement { Entity = e2, Dummy = 2 });
            ebuf.Add(new EcsComplexEntityRefElement { Entity = e3, Dummy = 3 });

            m_Manager.DestroyEntity(dummyEntity);

            var refFilePathName = @"Packages\com.unity.entities\Unity.Entities.Tests\Serialization\WorldTest.yaml";
            
            // To generate the file we'll test against
            using (var sw = new StreamWriter(refFilePathName))
            {
                sw.NewLine = "\n";
                SerializeUtility.SerializeWorldIntoYAML(m_Manager, sw, false);
            }

            using (var memStream = new MemoryStream())
            {
                byte[] testContentBuffer;

                // Save the World to a memory buffer via a a Stream Writer
                using (var sw = new StreamWriter(memStream))
                {
                    sw.NewLine = "\n";
                    SerializeUtility.SerializeWorldIntoYAML(m_Manager, sw, false);
                    sw.Flush();
                    memStream.Seek(0, SeekOrigin.Begin);
                    testContentBuffer = memStream.ToArray();
                }

                // Load both reference content and the test one into strings and compare
                using (var sr = File.OpenRead(refFilePathName))
                using (var testMemoryStream = new MemoryStream(testContentBuffer))
                {
                    Assert.IsTrue(YAMLSerializationHelpers.EqualYAMLFiles(sr, testMemoryStream));
                }
            }
        }

        
        [Test]
        public void WorldYamlSerialization_UsingStreamWriterWithCRLF_ThrowsArgumentException()
        {
            using (var memStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memStream))
            {
                streamWriter.NewLine = "\r\n";
                Assert.Throws<ArgumentException>(() =>
                {
                    SerializeUtility.SerializeWorldIntoYAML(m_Manager, streamWriter, false);
                });
            }
        }
#endif // !NET_DOTS
#endif // !UNITY_DISABLE_MANAGED_COMPONENTS
    }
}
