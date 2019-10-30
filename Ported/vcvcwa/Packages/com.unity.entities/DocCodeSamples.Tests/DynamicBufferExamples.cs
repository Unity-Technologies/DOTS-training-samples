using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

public class DynamicBuffersManual : MonoBehaviour
{
}

#region declare-element

public struct IntBufferElement : IBufferElementData
{
    public int Value;
}

#endregion

#region declare-element-full

// InternalBufferCapacity specifies how many elements a buffer can have before
// the buffer storage is moved outside the chunk.
[InternalBufferCapacity(8)]
public struct MyBufferElement : IBufferElementData
{
    // Actual value each buffer element will store.
    public int Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator int(MyBufferElement e)
    {
        return e.Value;
    }

    public static implicit operator MyBufferElement(int e)
    {
        return new MyBufferElement {Value = e};
    }
}

#endregion

public struct DataToSpawn : IComponentData
{
    public int EntityCount;
    public int ElementCount;
}

public class AddBufferSnippets : JobComponentSystem
{
    protected override void OnCreate()
    {
        var entity = EntityManager.CreateEntity();

        #region add-with-manager

        EntityManager.AddBuffer<MyBufferElement>(entity);

        #endregion

        #region add-with-archetype

        Entity e = EntityManager.CreateEntity(typeof(MyBufferElement));

        #endregion

        #region reinterpret-snippet

        DynamicBuffer<int> intBuffer
            = EntityManager.GetBuffer<MyBufferElement>(entity).Reinterpret<int>();

        #endregion

        #region access-manager

        DynamicBuffer<MyBufferElement> dynamicBuffer
            = EntityManager.GetBuffer<MyBufferElement>(entity);

        #endregion

        #region lookup-snippet

        BufferFromEntity<MyBufferElement> lookup = GetBufferFromEntity<MyBufferElement>();
        var buffer = lookup[entity];
        buffer.Add(17);
        buffer.RemoveAt(0);

        #endregion
    }

    #region add-in-job

    struct DataSpawnJob : IJobForEachWithEntity<DataToSpawn>
    {
        // A command buffer marshals structural changes to the data
        public EntityCommandBuffer.Concurrent CommandBuffer;

        //The DataToSpawn component tells us how many entities with buffers to create
        public void Execute(Entity spawnEntity, int index, [ReadOnly] ref DataToSpawn data)
        {
            for (int e = 0; e < data.EntityCount; e++)
            {
                //Create a new entity for the command buffer
                Entity newEntity = CommandBuffer.CreateEntity(index);

                //Create the dynamic buffer and add it to the new entity
                DynamicBuffer<MyBufferElement> buffer =
                    CommandBuffer.AddBuffer<MyBufferElement>(index, newEntity);

                //Reinterpret to plain int buffer
                DynamicBuffer<int> intBuffer = buffer.Reinterpret<int>();

                //Optionally, populate the dynamic buffer
                for (int j = 0; j < data.ElementCount; j++)
                {
                    intBuffer.Add(j);
                }
            }

            //Destroy the DataToSpawn entity since it has done its job
            CommandBuffer.DestroyEntity(index, spawnEntity);
        }
    }

    #endregion

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

#region access-buffer-system

public class DynamicBufferSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var sum = 0;

        Entities.ForEach((DynamicBuffer<MyBufferElement> buffer) =>
        {
            foreach (var integer in buffer.Reinterpret<int>())
            {
                sum += integer;
            }
        });

        Debug.Log("Sum of all buffers: " + sum);
    }
}

#endregion

#region declare-ijfe-with-buffer

public struct BuffersByEntity : IJobForEachWithEntity_EB<MyBufferElement>

    #endregion

{
    #region declare-ijfe-execute-method

    public void Execute(Entity entity, int index, DynamicBuffer<MyBufferElement> buffer)

        #endregion

    {
        //...
    }
}

#region access-ijfe

public class DynamicBufferForEachSystem : JobComponentSystem
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        EntityQueryDesc queryDescription = new EntityQueryDesc();
        queryDescription.All = new[] {ComponentType.ReadOnly<MyBufferElement>()};
        query = GetEntityQuery(queryDescription);
    }

    //Sums the elements of individual buffers of each entity
    public struct BuffersByEntity : IJobForEachWithEntity_EB<MyBufferElement>
    {
        public NativeArray<int> sums;

        public void Execute(Entity entity,
            int index,
            DynamicBuffer<MyBufferElement> buffer)
        {
            foreach (int integer in buffer.Reinterpret<int>())
            {
                sums[index] += integer;
            }
        }
    }

    //Sums the intermediate results into the final total
    public struct SumResult : IJob
    {
        [DeallocateOnJobCompletion] public NativeArray<int> sums;

        public void Execute()
        {
            int sum = 0;
            foreach (int integer in sums)
            {
                sum += integer;
            }

            //Note: Debug.Log is not burst-compatible
            Debug.Log("Sum of all buffers: " + sum);
        }
    }

    //Schedules the two jobs with a dependency between them
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Create a native array to hold the intermediate sums
        int entitiesInQuery = query.CalculateEntityCount();
        NativeArray<int> intermediateSums
            = new NativeArray<int>(entitiesInQuery, Allocator.TempJob);

        //Schedule the first job to add all the buffer elements
        BuffersByEntity bufferJob = new BuffersByEntity();
        bufferJob.sums = intermediateSums;
        JobHandle intermediateJob = bufferJob.Schedule(this, inputDeps);

        //Schedule the second job, which depends on the first
        SumResult finalSumJob = new SumResult();
        finalSumJob.sums = intermediateSums;
        return finalSumJob.Schedule(intermediateJob);
    }
}

#endregion

#region access-chunk-job

public class DynamicBufferJobSystem : JobComponentSystem
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        //Create a query to find all entities with a dynamic buffer
        // containing MyBufferElement
        EntityQueryDesc queryDescription = new EntityQueryDesc();
        queryDescription.All = new[] {ComponentType.ReadOnly<MyBufferElement>()};
        query = GetEntityQuery(queryDescription);
    }

    public struct BuffersInChunks : IJobChunk
    {
        //The data type and safety object
        public ArchetypeChunkBufferType<MyBufferElement> BufferType;

        //An array to hold the output, intermediate sums
        public NativeArray<int> sums;

        public void Execute(ArchetypeChunk chunk,
            int chunkIndex,
            int firstEntityIndex)
        {
            //A buffer accessor is a list of all the buffers in the chunk
            BufferAccessor<MyBufferElement> buffers
                = chunk.GetBufferAccessor(BufferType);

            for (int c = 0; c < chunk.Count; c++)
            {
                //An individual dynamic buffer for a specific entity
                DynamicBuffer<MyBufferElement> buffer = buffers[c];
                foreach (MyBufferElement element in buffer)
                {
                    sums[chunkIndex] += element.Value;
                }
            }
        }
    }

    //Sums the intermediate results into the final total
    public struct SumResult : IJob
    {
        [DeallocateOnJobCompletion] public NativeArray<int> sums;

        public void Execute()
        {
            int sum = 0;
            foreach (int integer in sums)
            {
                sum += integer;
            }

            //Note: Debug.Log is not burst-compatible
            Debug.Log("Sum of all buffers: " + sum);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Create a native array to hold the intermediate sums
        int chunksInQuery = query.CalculateChunkCount();
        NativeArray<int> intermediateSums
            = new NativeArray<int>(chunksInQuery, Allocator.TempJob);

        //Schedule the first job to add all the buffer elements
        BuffersInChunks bufferJob = new BuffersInChunks();
        bufferJob.BufferType = GetArchetypeChunkBufferType<MyBufferElement>();
        bufferJob.sums = intermediateSums;
        JobHandle intermediateJob = bufferJob.Schedule(query, inputDeps);

        //Schedule the second job, which depends on the first
        SumResult finalSumJob = new SumResult();
        finalSumJob.sums = intermediateSums;
        return finalSumJob.Schedule(intermediateJob);
    }
}

#endregion

#region dynamicbuffer.class

[InternalBufferCapacity(8)]
public struct FloatBufferElement : IBufferElementData
{
    // Actual value each buffer element will store.
    public float Value;

    // The following implicit conversions are optional, but can be convenient.
    public static implicit operator float(FloatBufferElement e)
    {
        return e.Value;
    }

    public static implicit operator FloatBufferElement(float e)
    {
        return new FloatBufferElement {Value = e};
    }
}

public class DynamicBufferExample : ComponentSystem
{
    protected override void OnUpdate()
    {
        float sum = 0;

        Entities.ForEach((DynamicBuffer<FloatBufferElement> buffer) =>
        {
            foreach (var element in buffer.Reinterpret<float>())
            {
                sum += element;
            }
        });

        Debug.Log("Sum of all buffers: " + sum);
    }
}

#endregion

class DynamicBufferSnippets
{
    private void ShowAdd()
    {
        DynamicBuffer<int> buffer = new DynamicBuffer<int>();
        DynamicBuffer<int> secondBuffer = new DynamicBuffer<int>();

        #region dynamicbuffer.add

        buffer.Add(5);

        #endregion

        #region dynamicbuffer.addrange

        int[] source = {1, 2, 3, 4, 5};
        NativeArray<int> newElements = new NativeArray<int>(source, Allocator.Persistent);
        buffer.AddRange(newElements);

        #endregion

        #region dynamicbuffer.asnativearray

        int[] intArray = {1, 2, 3, 4, 5};
        NativeArray<int>.Copy(intArray, buffer.AsNativeArray());

        #endregion

        #region dynamicbuffer.capacity

        #endregion

        #region dynamicbuffer.clear

        buffer.Clear();

        #endregion

        #region dynamicbuffer.copyfrom.dynamicbuffer

        buffer.CopyFrom(secondBuffer);

        #endregion

        #region dynamicbuffer.copyfrom.nativearray

        int[] sourceArray = {1, 2, 3, 4, 5};
        NativeArray<int> nativeArray = new NativeArray<int>(source, Allocator.Persistent);
        buffer.CopyFrom(nativeArray);

        #endregion

        #region dynamicbuffer.copyfrom.array

        int[] integerArray = {1, 2, 3, 4, 5};
        buffer.CopyFrom(integerArray);

        #endregion

        #region dynamicbuffer.getenumerator

        foreach (var element in buffer)
        {
            //Use element...
        }

        #endregion

        #region dynamicbuffer.getunsafeptr

        #endregion

        int insertionIndex = 2;

        #region dynamicbuffer.insert

        if (insertionIndex < buffer.Length)
            buffer.Insert(insertionIndex, 6);

        #endregion

        #region dynamicbuffer.iscreated

        #endregion

        #region dynamicbuffer.length

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = i * i;
        }

        #endregion

        #region dynamicbuffer.removeat

        if (insertionIndex < buffer.Length)
            buffer.RemoveAt(insertionIndex);

        #endregion

        int start = 1;

        #region dynamicbuffer.removerange

        buffer.RemoveRange(start, 5);

        #endregion

        #region dynamicbuffer.reserve

        buffer.Reserve(buffer.Capacity + 10);

        #endregion

        #region dynamicbuffer.resizeuninitialized

        buffer.ResizeUninitialized(buffer.Length + 10);

        #endregion

        #region dynamicbuffer.indexoperator

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = i * i;
        }

        #endregion

        #region dynamicbuffer.tonativearray

        NativeArray<int> copy = buffer.ToNativeArray(Allocator.Persistent);

        #endregion

        #region dynamicbuffer.trimexcess

        if (buffer.Capacity > buffer.Length)
            buffer.TrimExcess();

        #endregion
    }
}

public class ReinterpretExample : ComponentSystem
{
    protected override void OnUpdate()
    {
        #region dynamicbuffer.reinterpret

        Entities.ForEach((DynamicBuffer<FloatBufferElement> buffer) =>
        {
            DynamicBuffer<float> floatBuffer = buffer.Reinterpret<float>();
            for (int i = 0; i < floatBuffer.Length; i++)
            {
                floatBuffer[i] = i * 1.2f;
            }
        });

        #endregion
    }
}
