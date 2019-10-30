using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities.CodeGeneratedJobForEach
{
    public struct LambdaParameterValueProvider_Entity
    {
        private ArchetypeChunkEntityType _type;

        public struct Runtime
        {
            [NativeDisableUnsafePtrRestriction]
            public unsafe Entity* arrayPtr;

            public unsafe ref Entity For(int i)
            {
                return ref *(arrayPtr + i);
            }
        }

        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
            _type = jobComponentSystem.GetArchetypeChunkEntityType();
        }

        public unsafe Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk)
        {
            var ptr = (Entity*) chunk.GetNativeArray(_type).GetUnsafeReadOnlyPtr();
            return new Runtime()
            {
                arrayPtr = ptr
            };
        }
    }
    
    public struct LambdaParameterValueProvider_DynamicBuffer<T> where T : struct, IBufferElementData
    {
        ArchetypeChunkBufferType<T> _type;

        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
            _type = jobComponentSystem.GetArchetypeChunkBufferType<T>(isReadOnly);
        }

        public struct Runtime
        {
            public BufferAccessor<T> bufferAccessor;

            public DynamicBuffer<T> For(int i)
            {
                return bufferAccessor[i];
            }
        }

        public Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk)
        {
            return new Runtime()
            {
                bufferAccessor = chunk.GetBufferAccessor(_type)
            };
        }
    }
    
    public struct LambdaParameterValueProvider_IComponentData<T>
        where T : struct, IComponentData
    {
        ArchetypeChunkComponentType<T> _type;

        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
            _type = jobComponentSystem.GetArchetypeChunkComponentType<T>(isReadOnly);
        }

        public struct Runtime
        {
            public unsafe byte* ptr;

            public unsafe ref T For(int i)
            {
                return ref UnsafeUtilityEx.ArrayElementAsRef<T>(ptr, i);
            }
        }

        public unsafe Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk)
        {
            var componentDatas = chunk.GetNativeArray(_type);
            return new Runtime()
            {
                ptr = (byte*) (_type.IsReadOnly
                    ? componentDatas.GetUnsafeReadOnlyPtr()
                    : componentDatas.GetUnsafePtr()),
            };
        }
    }
    
    public struct LambdaParameterValueProvider_ManagedComponentData<T> where T : class
    {
        private EntityManager _entityManager;
        private ArchetypeChunkComponentType<T> _type;

        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
            _entityManager = jobComponentSystem.EntityManager;
            _type = _entityManager.GetArchetypeChunkComponentType<T>(isReadOnly);
        }

        public struct Runtime
        {
            internal ArchetypeChunkComponentObjects<T> _objects;
            public T For(int i) => _objects[i];
        }

        public Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk)
        {
            return new Runtime()
            {
                _objects = chunk.GetComponentObjects(_type, _entityManager)
            };
        }
    }

    public struct LambdaParameterValueProvider_ISharedComponentData<T> where T : struct, ISharedComponentData
    {
        ArchetypeChunkSharedComponentType<T> _type;
        EntityManager _entityManager;

        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
            _type = jobComponentSystem.GetArchetypeChunkSharedComponentType<T>();
            _entityManager = jobComponentSystem.EntityManager;
        }

        public struct Runtime
        {
            public T _data;
            public T For(int i) => _data;
        }

        public Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk) =>
            new Runtime()
            {
                _data = chunk.GetSharedComponentData(_type, _entityManager)
            };
    }
    
    public struct LambdaParameterValueProvider_EntityInQueryIndex
    {
        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
        }

        public struct Runtime
        {
            internal int entityInQueryIndexOfFirstEntityInChunk;
            public int For(int i) => entityInQueryIndexOfFirstEntityInChunk + i;
        }

        public Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk, int chunkIndex, int entityInQueryIndexOfFirstEntity)
        {
            return new Runtime() {entityInQueryIndexOfFirstEntityInChunk = entityInQueryIndexOfFirstEntity};
        }
    }
    
    public struct LambdaParameterValueProvider_NativeThreadIndex
    {
        [NativeSetThreadIndexAttribute] internal int _nativeThreadIndex;
        
        public void ScheduleTimeInitialize(ComponentSystemBase jobComponentSystem, bool isReadOnly)
        {
        }

        public struct Runtime
        {
            internal int _nativeThreadIndex;
            public int For(int i) => _nativeThreadIndex;
        }

        public Runtime PrepareToExecuteOnEntitiesIn(ref ArchetypeChunk chunk)
        {
            return new Runtime() {_nativeThreadIndex = _nativeThreadIndex};
        }
    }
}