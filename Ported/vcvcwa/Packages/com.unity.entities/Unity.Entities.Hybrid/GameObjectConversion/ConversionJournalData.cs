using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityLogType = UnityEngine.LogType;
using UnityObject = UnityEngine.Object;
using GameObject = UnityEngine.GameObject;

namespace Unity.Entities.Conversion
{
    interface IConversionEventData { }

    struct LogEventData : IConversionEventData
    {
        public UnityLogType Type;
        public string Message;
    }

    partial struct ConversionJournalData : IDisposable
    {
        NativeHashMap<int, int> m_HeadIdIndices; // object instanceId -> front index

        // Only for UnityEngine component types to be stored in a companion GameObject
        Dictionary<GameObject, int> m_HybridHeadIdIndices; // maps to MultiList m_HybridTypes

        public Dictionary<GameObject, int> HybridHeadIdIndices { get => m_HybridHeadIdIndices; }

        public void Dispose()
        {
            m_HeadIdIndices.Dispose();
        }

        // ** keep this block in sync ** (begin)

        MultiList<Entity> m_Entities;
        MultiList<LogEventData> m_LogEvents;
        MultiList<Type> m_HybridTypes;

        public void Init()
        {
            m_HeadIdIndices = new NativeHashMap<int, int>(1000, Allocator.Persistent);
            m_HybridHeadIdIndices = new Dictionary<GameObject, int>();

            m_Entities.Init();
            m_LogEvents.Init();
            m_HybridTypes.Init();
        }

        public void RemoveForIncremental(GameObject gameObject)
        {
            if (m_HeadIdIndices.TryGetValue(gameObject.GetInstanceID(), out var headIdIndex))
            {
                m_Entities.ReleaseListKeepHead(headIdIndex);
                m_LogEvents.ReleaseList(headIdIndex);
            }

            if (m_HybridHeadIdIndices.TryGetValue(gameObject, out headIdIndex))
            {
                m_HybridTypes.ReleaseList(headIdIndex);
            }
        }

        // for debug/test
        IEnumerable<IJournalDataDebug> SelectJournalDataDebug(int objectInstanceId, int headIdIndex)
        {
            foreach (var e in SelectJournalDataDebug(objectInstanceId, headIdIndex, ref m_Entities)) yield return e;
            foreach (var e in SelectJournalDataDebug(objectInstanceId, headIdIndex, ref m_LogEvents)) yield return e;
        }

        // ** keep this block in sync ** (end)

        int GetOrAddHeadIdIndex(int objectInstanceId)
        {
            if (!m_HeadIdIndices.TryGetValue(objectInstanceId, out var headIdIndex))
            {
                headIdIndex = m_HeadIdIndices.Length;
                m_HeadIdIndices.Add(objectInstanceId, headIdIndex);

                var headIdsCapacity = headIdIndex + 1;
                if (MultiList.CalcExpandCapacity(m_Entities.HeadIds.Length, ref headIdsCapacity))
                {
                    m_Entities.SetHeadIdsCapacity(headIdsCapacity);
                    m_LogEvents.SetHeadIdsCapacity(headIdsCapacity);
                }
            }

            return headIdIndex;
        }

        int GetOrAddHybridHeadIdIndex(GameObject gameObject)
        {
            if (!m_HybridHeadIdIndices.TryGetValue(gameObject, out var headIdIndex))
            {
                headIdIndex = m_HybridHeadIdIndices.Count;
                m_HybridHeadIdIndices.Add(gameObject, headIdIndex);

                var headIdsCapacity = headIdIndex + 1;
                if (MultiList.CalcExpandCapacity(m_HybridTypes.HeadIds.Length, ref headIdsCapacity))
                {
                    m_HybridTypes.SetHeadIdsCapacity(headIdsCapacity);
                }
            }

            return headIdIndex;
        }

        // creates new head, returns false if already had one
        void AddHead<T>(int objectInstanceId, ref MultiList<T> store, in T data) =>
            store.AddHead(GetOrAddHeadIdIndex(objectInstanceId), data);

        // creates new head or adds a new entry
        void Add<T>(int objectInstanceId, ref MultiList<T> store, in T data) =>
            store.Add(GetOrAddHeadIdIndex(objectInstanceId), data);

        // requires existing sublist, walks to end and adds, returns count (can be slow with large count)
        (int id, int serial) AddTail<T>(int objectInstanceId, ref MultiList<T> store) =>
            m_HeadIdIndices.TryGetValue(objectInstanceId, out var headIdIndex)
                ? store.AddTail(headIdIndex) : (-1, 0);

        int GetHeadId<T>(int objectInstanceId, ref MultiList<T> store)
        {
            if (!m_HeadIdIndices.TryGetValue(objectInstanceId, out var headIdIndex))
                return -1;

            return store.HeadIds[headIdIndex];
        }

        bool HasHead<T>(int objectInstanceId, ref MultiList<T> store) =>
            GetHeadId(objectInstanceId, ref store) >= 0;

        bool GetHeadData<T>(int objectInstanceId, ref MultiList<T> store, ref T data)
        {
            var headId = GetHeadId(objectInstanceId, ref store);
            if (headId < 0)
                return false;

            data = store.Data[headId];
            return true;
        }

        public void RecordPrimaryEntity(int objectInstanceId, Entity entity) =>
            AddHead(objectInstanceId, ref m_Entities, entity);

        public bool HasPrimaryEntity(int objectInstanceId) =>
            HasHead(objectInstanceId, ref m_Entities);

        public bool TryGetPrimaryEntity(int objectInstanceId, out Entity entity)
        {
            entity = Entity.Null;
            return GetHeadData(objectInstanceId, ref m_Entities, ref entity);
        }

        public (int id, int serial) ReserveAdditionalEntity(int objectInstanceId) =>
            AddTail(objectInstanceId, ref m_Entities);

        public void RecordAdditionalEntityAt(int atId, Entity entity) =>
            m_Entities.Data[atId] = entity;

        // returns false if the object is unknown to the conversion system
        public bool GetEntities(int objectInstanceId, out MultiListEnumerator<Entity> iter)
        {
            var headId = GetHeadId(objectInstanceId, ref m_Entities);
            iter = m_Entities.SelectListAt(headId);
            return headId >= 0;
        }

        bool RecordEvent<T>(UnityObject context, ref MultiList<T> eventStore, in T eventData)
            where T : IConversionEventData
        {
            // ignore if no context was given
            if (context == null)
                return false;

            context.CheckObjectIsNotComponent();

            //@TODO(scobi): record unknowns to scene object

            // ignore if conversion system does not know about this
            var instanceId = context.GetInstanceID();
            if (!HasHead(instanceId, ref m_Entities))
                return false;

            Add(instanceId, ref eventStore, eventData);
            return true;
        }

        public bool RecordLogEvent(UnityObject context, UnityLogType logType, string message) =>
            RecordEvent(context, ref m_LogEvents, new LogEventData { Type = logType, Message = message });

        public bool RecordExceptionEvent(UnityObject context, Exception exception) =>
            RecordLogEvent(context, UnityLogType.Exception, $"{exception.GetType().Name}: {exception.Message}");

        MultiListEnumerator<T> SelectJournalData<T>(UnityObject context, ref MultiList<T> store)
        {
            var iter = store.SelectListAt(GetHeadId(context.GetInstanceID(), ref store));
            if (!iter.IsValid)
                context.CheckObjectIsNotComponent();

            return iter;
        }

        IEnumerable<(int objectInstanceId, T eventData)> SelectJournalData<T>(MultiList<T> store)
        {
            //@TODO: make custom enumerator for this

            using (var headIdIndices = m_HeadIdIndices.GetKeyValueArrays(Allocator.Temp))
            {
                for (var i = 0; i < headIdIndices.Keys.Length; ++i)
                {
                    var objectInstanceId = headIdIndices.Keys[i];
                    if (store.TrySelectList(headIdIndices.Values[i], out var iter))
                    {
                        foreach (var e in iter)
                            yield return (objectInstanceId, e);
                    }
                }
            }
        }

        public MultiListEnumerator<Entity> SelectEntities(UnityObject context) =>
            SelectJournalData(context, ref m_Entities);

        public MultiListEnumerator<LogEventData> SelectLogEventsFast(UnityObject context) =>
            SelectJournalData(context, ref m_LogEvents);

        public LogEventData[] SelectLogEventsOrdered(UnityObject context)
        {
            using (var iter = SelectLogEventsFast(context))
            {
                var count = iter.Count();
                if (count == 0)
                    return Array.Empty<LogEventData>();

                var events = new LogEventData[count];

                iter.Reset();
                iter.MoveNext();

                // head
                events[0] = iter.Current;

                // rest of list in reverse order
                while (iter.MoveNext())
                    events[--count] = iter.Current;

                return events;
            }
        }

        public void AddHybridComponent(GameObject gameObject, Type type)
        {
            int index = GetOrAddHybridHeadIdIndex(gameObject);
            m_HybridTypes.Add(index, type);
        }

        public MultiListEnumerator<Type> HybridTypes(int headId) =>
            m_HybridTypes.SelectListAt(headId);

        public IEnumerable<(int objectInstanceId, LogEventData eventData)> SelectLogEventsFast() =>
            SelectJournalData(m_LogEvents);

        public IEnumerable<(int objectInstanceId, LogEventData)> SelectLogEventsOrdered()
        {
            using (var headIdIndices = m_HeadIdIndices.GetKeyValueArrays(Allocator.Temp))
            {
                var events = Array.Empty<LogEventData>();

                for (var i = 0; i < headIdIndices.Keys.Length; ++i)
                {
                    var objectInstanceId = headIdIndices.Keys[i];
                    if (m_LogEvents.TrySelectList(headIdIndices.Values[i], out var iter))
                    {
                        using (iter)
                        {
                            var count = iter.Count();
                            if (count == 0)
                                continue;

                            var newCount = count;
                            if (MultiList.CalcExpandCapacity(events.Length, ref newCount))
                                events = new LogEventData[newCount];

                            iter.Reset();
                            iter.MoveNext();

                            // head
                            events[0] = iter.Current;

                            // rest of list in reverse order
                            for (var e = count - 1; iter.MoveNext(); --e)
                                events[e] = iter.Current;

                            // yield in forward order
                            for (var e = 0; e < count; ++e)
                                yield return (objectInstanceId, events[e]);
                        }
                    }
                }
            }
        }
    }
}
