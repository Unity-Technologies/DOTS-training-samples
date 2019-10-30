using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using System;

namespace Unity.Scenes
{
    struct LiveLinkSceneMsg
    {
        public NativeArray<Hash128> LoadedScenes;
        public NativeArray<Hash128> RemovedScenes;

        public void Dispose()
        {
            LoadedScenes.Dispose();
            RemovedScenes.Dispose();
        }

        unsafe public byte[] ToMsg()
        {
            var buffer = new UnsafeAppendBuffer(0, 16, Allocator.TempJob);
            Serialize(ref buffer);
            var bytes = buffer.ToBytes();
            buffer.Dispose();
            return bytes;
        }

        unsafe public static LiveLinkSceneMsg FromMsg(byte[] buffer, Allocator allocator)
        {
            fixed (byte* ptr = buffer)
            {
                var reader = new UnsafeAppendBuffer.Reader(ptr, buffer.Length);
                LiveLinkSceneMsg msg = default;
                msg.Deserialize(ref reader, allocator);
                return msg;
            }
        }
        
        void Serialize(ref UnsafeAppendBuffer buffer)
        {
            buffer.Add(LoadedScenes);
            buffer.Add(RemovedScenes);
        }
        
        void Deserialize(ref UnsafeAppendBuffer.Reader buffer, Allocator allocator)
        {
            buffer.ReadNext(out LoadedScenes, allocator);
            buffer.ReadNext(out RemovedScenes, allocator);
        }
    }

    class LiveLinkSceneChangeTracker
    {
        private EntityQuery _LoadedScenesQuery;
        private EntityQuery _UnloadedScenesQuery;
        private NativeList<SceneReference>  m_PreviousScenes;
        private NativeList<LiveLinkPatcher.LiveLinkedSceneState> m_PreviousRemovedScenes;

        public LiveLinkSceneChangeTracker(EntityManager manager)
        {
            _LoadedScenesQuery = manager.CreateEntityQuery(typeof(SceneReference));
            _UnloadedScenesQuery = manager.CreateEntityQuery(ComponentType.Exclude<SceneReference>(), ComponentType.ReadOnly<LiveLinkPatcher.LiveLinkedSceneState>());
            m_PreviousScenes = new NativeList<SceneReference>(Allocator.Persistent);
            m_PreviousRemovedScenes = new NativeList<LiveLinkPatcher.LiveLinkedSceneState>(Allocator.Persistent);
        }

        public void Dispose()
        {
            _LoadedScenesQuery.Dispose();
            _UnloadedScenesQuery.Dispose();
            m_PreviousScenes.Dispose();
            m_PreviousRemovedScenes.Dispose();
        }

        public void Reset()
        {
            m_PreviousScenes.Clear();
            m_PreviousRemovedScenes.Clear();
        }
        
        public bool GetSceneMessage(out LiveLinkSceneMsg msg)
        {
            var loadedScenes = _LoadedScenesQuery.ToComponentDataArray<SceneReference>(Allocator.TempJob);
            var removedScenes = _UnloadedScenesQuery.ToComponentDataArray<LiveLinkPatcher.LiveLinkedSceneState>(Allocator.TempJob);
            var newRemovedScenes = SubtractArrays(removedScenes, m_PreviousRemovedScenes);

            var noNewUnloads = newRemovedScenes.Length == 0;
            var sameLoadState = loadedScenes.ArraysEqual(m_PreviousScenes.AsArray());
            if (noNewUnloads && sameLoadState)
            {
                loadedScenes.Dispose();
                removedScenes.Dispose();
                newRemovedScenes.Dispose();
                msg = default;
                return false;
            }

            msg.LoadedScenes = loadedScenes.Reinterpret<Hash128>();
            msg.RemovedScenes = newRemovedScenes.Reinterpret<Hash128>();

            m_PreviousScenes.Clear();
            m_PreviousRemovedScenes.Clear();
            m_PreviousScenes.AddRange(loadedScenes);
            m_PreviousRemovedScenes.AddRange(removedScenes);
            removedScenes.Dispose();

            return true;
        }

        internal static NativeArray<T> SubtractArrays<T>(NativeArray<T> minuend, NativeArray<T> subtrahend) where T : struct, IEquatable<T>
        {
            if (minuend.Length == 0 || minuend.ArraysEqual(subtrahend))
                return new NativeArray<T>(new T[0], Allocator.Persistent);

            if (subtrahend.Length == 0)
                return new NativeArray<T>(minuend, Allocator.Persistent);

            var result = new NativeList<T>(Allocator.Temp);
            result.AddRange(minuend);

            foreach (var sub in subtrahend)
            {
                var i = result.IndexOf(sub);
                if (i != -1)
                    result.RemoveAtSwapBack(i);
            }
            var asArray = new NativeArray<T>(result.AsArray(), Allocator.Persistent);
            result.Dispose();
            return asArray;
        }
    }
}