using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Serialization
{
    public static class SerializeUtilityHybrid
    {
        public static void Serialize(EntityManager manager, BinaryWriter writer, out ReferencedUnityObjects objRefs)
        {
            object[] referencedObjects;
            SerializeUtility.SerializeWorld(manager, writer, out referencedObjects);
            SerializeObjectReferences(manager, writer, (UnityEngine.Object[]) referencedObjects, out objRefs);
        }

        public static void Serialize(EntityManager manager, BinaryWriter writer, out ReferencedUnityObjects objRefs, NativeArray<EntityRemapUtility.EntityRemapInfo> entityRemapInfos)
        {
            object[] referencedObjects;
            SerializeUtility.SerializeWorld(manager, writer, out referencedObjects, entityRemapInfos);
            SerializeObjectReferences(manager, writer, (UnityEngine.Object[]) referencedObjects, out objRefs);
        }

        public static void Deserialize(EntityManager manager, BinaryReader reader, ReferencedUnityObjects objRefs)
        {
            DeserializeObjectReferences(manager, objRefs, "", out var objectReferences);
            var transaction = manager.BeginExclusiveEntityTransaction();
            SerializeUtility.DeserializeWorld(transaction, reader, objectReferences);
            manager.EndExclusiveEntityTransaction();
        }

        public static void SerializeObjectReferences(EntityManager manager, BinaryWriter writer, UnityEngine.Object[] referencedObjects, out ReferencedUnityObjects objRefs)
        {
            objRefs = null;

            if (referencedObjects != null && referencedObjects.Length > 0)
            {
                objRefs = ScriptableObject.CreateInstance<ReferencedUnityObjects>();
                objRefs.Array = referencedObjects;
            }
        }

        public static void DeserializeObjectReferences(EntityManager manager, ReferencedUnityObjects objRefs, string debugSceneName, out UnityEngine.Object[] objectReferences)
        {
            objectReferences = objRefs?.Array;
        }
    }
}
