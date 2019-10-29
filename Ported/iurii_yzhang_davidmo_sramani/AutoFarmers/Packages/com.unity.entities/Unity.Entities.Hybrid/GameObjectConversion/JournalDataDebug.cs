using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Conversion
{
    interface IJournalDataDebug { }

    static class JournalDataDebug
    {
        public static JournalDataDebug<T> Create<T>(int objectInstanceId, in T eventData) =>
            new JournalDataDebug<T>(objectInstanceId, eventData);
    }

    class JournalDataDebug<T> : IJournalDataDebug
    {
        public readonly int ObjectInstanceId;
        public readonly T Data;

        public JournalDataDebug(int objectInstanceId, in T data)
        {
            ObjectInstanceId = objectInstanceId;
            Data = data;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is JournalDataDebug<T> typed))
                return false;

            return
                ObjectInstanceId == typed.ObjectInstanceId &&
                Data.Equals(typed.Data);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ObjectInstanceId * 397) ^ EqualityComparer<T>.Default.GetHashCode(Data);
            }
        }
    }

    partial struct ConversionJournalData
    {
        static IEnumerable<IJournalDataDebug> SelectJournalDataDebug<T>(int objectInstanceId, int headIdIndex, ref MultiList<T> store) =>
            store
                .SelectListAt(store.HeadIds[headIdIndex])
                .Select(e => new JournalDataDebug<T>(objectInstanceId, e));

        public IEnumerable<IJournalDataDebug> SelectJournalDataDebug()
        {
            using (var keysValues = m_HeadIdIndices.GetKeyValueArrays(Allocator.Temp))
            {
                for (var i = 0; i < keysValues.Keys.Length; ++i)
                {
                    foreach (var e in SelectJournalDataDebug(keysValues.Keys[i], keysValues.Values[i]))
                        yield return e;
                }
            }
        }
        public IEnumerable<IJournalDataDebug> SelectJournalDataDebug(int objectInstanceId) =>
            m_HeadIdIndices.TryGetValue(objectInstanceId, out var headIdIndex)
                ? SelectJournalDataDebug(objectInstanceId, headIdIndex)
                : Enumerable.Empty<IJournalDataDebug>();
    }

    static class JournalDataDebugExtensions
    {
        public static IEnumerable<JournalDataDebug<T>> EventsOfType<T>(this IEnumerable<IJournalDataDebug> @this)
        {
            foreach (var i in @this)
            {
                if (i is JournalDataDebug<T> typed)
                    yield return typed;
            }
        }
    }
}
