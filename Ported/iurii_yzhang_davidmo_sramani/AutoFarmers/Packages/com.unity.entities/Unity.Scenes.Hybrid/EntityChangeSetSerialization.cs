using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEditor;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes
{
    static unsafe class EntityChangeSetSerialization
    {
        public struct ResourcePacket : IDisposable
        {
            public NativeArray<RuntimeGlobalObjectId> GlobalObjectIds;
            public UnsafeAppendBuffer                 ChangeSet;

            public ResourcePacket(byte[] buffer)
            {
                fixed (byte* ptr = buffer)
                {
                    var bufferReader = new UnsafeAppendBuffer.Reader(ptr, buffer.Length);

                    bufferReader.ReadNext(out GlobalObjectIds, Allocator.Persistent);

                    var entityChangeSetSourcePtr = bufferReader.Ptr + bufferReader.Offset;
                    var entityChangeSetSourceSize = bufferReader.Size - bufferReader.Offset;
                    ChangeSet = new UnsafeAppendBuffer(entityChangeSetSourceSize, 16, Allocator.Persistent);
                    ChangeSet.Add(entityChangeSetSourcePtr, entityChangeSetSourceSize);
                }
            }

            public void Dispose()
            {
                GlobalObjectIds.Dispose();
                ChangeSet.Dispose();
            }

#if UNITY_EDITOR
            unsafe public static void SerializeResourcePacket(EntityChangeSet entityChangeSet, ref UnsafeAppendBuffer buffer)
            {
                var changeSetBuffer = new UnsafeAppendBuffer(1024, 16, Allocator.TempJob);
                Serialize(entityChangeSet, &changeSetBuffer, out var globalObjectIds);
                
                buffer.Add(globalObjectIds);
                buffer.Add(changeSetBuffer.Ptr, changeSetBuffer.Size);
                
                changeSetBuffer.Dispose();
                globalObjectIds.Dispose();
            }
#endif
        }

        public static EntityChangeSet Deserialize(UnsafeAppendBuffer.Reader* bufferReader, NativeArray<RuntimeGlobalObjectId> globalObjectIDs, GlobalAssetObjectResolver resolver)
        {
            bufferReader->ReadNext<ComponentTypeHash>(out var typeHashes, Allocator.Persistent);

            for (int i = 0; i != typeHashes.Length; i++)
            {
                var stableTypeHash = typeHashes[i].StableTypeHash;
                var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(stableTypeHash);
                if (typeIndex == -1)
                {
                    typeHashes.Dispose();
                    throw new ArgumentException("The LiveLink Patch Type Layout doesn't match the Data Layout of the Components. Please Rebuild the Player.");
                }
            }
            
            var createdEntityCount = bufferReader->ReadNext<int>();
            var destroyedEntityCount = bufferReader->ReadNext<int>();
            bufferReader->ReadNext<EntityGuid>(out var entities, Allocator.Persistent);
            bufferReader->ReadNext<NativeString64>(out var names, Allocator.Persistent);
            bufferReader->ReadNext<PackedComponent>(out var addComponents, Allocator.Persistent);
            bufferReader->ReadNext<PackedComponent>(out var removeComponents, Allocator.Persistent);
            bufferReader->ReadNext<PackedComponentDataChange>(out var setComponents, Allocator.Persistent);
            bufferReader->ReadNext<byte>(out var componentData, Allocator.Persistent);
            bufferReader->ReadNext<EntityReferenceChange>(out var entityReferenceChanges, Allocator.Persistent);
            bufferReader->ReadNext<BlobAssetReferenceChange>(out var blobAssetReferenceChanges, Allocator.Persistent);
            bufferReader->ReadNext<LinkedEntityGroupChange>(out var linkedEntityGroupAdditions, Allocator.Persistent);
            bufferReader->ReadNext<LinkedEntityGroupChange>(out var linkedEntityGroupRemovals, Allocator.Persistent);
            bufferReader->ReadNext<BlobAssetChange>(out var createdBlobAssets, Allocator.Persistent);
            bufferReader->ReadNext<ulong>(out var destroyedBlobAssets, Allocator.Persistent);
            bufferReader->ReadNext<byte>(out var blobAssetData, Allocator.Persistent);
            bufferReader->ReadNext<PackedComponent>(out var setSharedComponentPackedComponents, Allocator.Persistent);


            var resolvedObjects = new UnityEngine.Object[globalObjectIDs.Length];
            resolver.ResolveObjects(globalObjectIDs, resolvedObjects);
            var reader = new PropertiesBinaryReader(bufferReader, resolvedObjects);
            var setSharedComponents = new PackedSharedComponentDataChange[setSharedComponentPackedComponents.Length];
            for (int i = 0; i < setSharedComponentPackedComponents.Length; i++)
            {
                object componentValue;
                if (bufferReader->ReadNext<int>() == 1)
                {
                    var packedTypeIndex = setSharedComponentPackedComponents[i].PackedTypeIndex;
                    var stableTypeHash = typeHashes[packedTypeIndex].StableTypeHash;
                    var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(stableTypeHash);
                    var type = TypeManager.GetType(typeIndex);
                    componentValue = BoxedProperties.ReadBoxedStruct(type, reader);
                }
                else
                    componentValue = null;

                setSharedComponents[i].Component = setSharedComponentPackedComponents[i];
                setSharedComponents[i].BoxedSharedValue = componentValue;
            }
            setSharedComponentPackedComponents.Dispose();

            //@TODO: Support managed components
            // -----@TODO: SUPPORT THIS
            var setManagedComponents = new PackedManagedComponentDataChange[0];

            //if (!bufferReader->EndOfBuffer)
            //    throw new Exception("Underflow in EntityChangeSet buffer");

            return new EntityChangeSet(
                createdEntityCount, 
                destroyedEntityCount, 
                entities,
                typeHashes, 
                names,
                addComponents, 
                removeComponents, 
                setComponents, 
                componentData, 
                entityReferenceChanges, 
                blobAssetReferenceChanges,
                setManagedComponents, 
                setSharedComponents,
                linkedEntityGroupAdditions, 
                linkedEntityGroupRemovals,
                createdBlobAssets,
                destroyedBlobAssets,
                blobAssetData);
        }

#if UNITY_EDITOR

        public static void Serialize(EntityChangeSet entityChangeSet, UnsafeAppendBuffer* buffer, out NativeArray<RuntimeGlobalObjectId> outAssets)
        {

            // Write EntityChangeSet
            buffer->Add(entityChangeSet.TypeHashes);
            buffer->Add(entityChangeSet.CreatedEntityCount);
            buffer->Add(entityChangeSet.DestroyedEntityCount);
            buffer->Add(entityChangeSet.Entities);
            buffer->Add(entityChangeSet.Names);
            buffer->Add(entityChangeSet.AddComponents);
            buffer->Add(entityChangeSet.RemoveComponents);
            buffer->Add(entityChangeSet.SetComponents);
            buffer->Add(entityChangeSet.ComponentData);
            buffer->Add(entityChangeSet.EntityReferenceChanges);
            buffer->Add(entityChangeSet.BlobAssetReferenceChanges);
            buffer->Add(entityChangeSet.LinkedEntityGroupAdditions);
            buffer->Add(entityChangeSet.LinkedEntityGroupRemovals);
            buffer->Add(entityChangeSet.CreatedBlobAssets);
            buffer->Add(entityChangeSet.DestroyedBlobAssets);
            buffer->Add(entityChangeSet.BlobAssetData);

            var setSharedComponentCount = entityChangeSet.SetSharedComponents.Length;
            buffer->Add(setSharedComponentCount);
            for (int i = 0; i < setSharedComponentCount; i++)
                buffer->Add(entityChangeSet.SetSharedComponents[i].Component);

            // Write shared components
            var assets = new HashSet<Hash128>();
            var writer = new PropertiesBinaryWriter(buffer);
            for (int i = 0; i < setSharedComponentCount; i++)
            {
                var srcData = entityChangeSet.SetSharedComponents[i].BoxedSharedValue;

                if (srcData != null)
                {
                    buffer->Add(1);
                    BoxedProperties.WriteBoxedType(srcData, writer);
                }
                else
                {
                    buffer->Add(0);
                }
            }
            
            var objectTable = writer.GetObjectTable();
            var globalObjectIds = new GlobalObjectId[objectTable.Length];  
            GlobalObjectId.GetGlobalObjectIdsSlow(objectTable, globalObjectIds);
            
            outAssets = new NativeArray<RuntimeGlobalObjectId>(globalObjectIds.Length, Allocator.Persistent);
            for (int i = 0; i != globalObjectIds.Length;i++)
            {
                var globalObjectId = globalObjectIds[i];

                //@TODO: HACK (Object is a scene object)
                if (globalObjectId.identifierType == 2)
                {
                    Debug.LogWarning($"{objectTable[i]} is part of a scene, LiveLink can't transfer scene objects. (Note: LiveConvertSceneView currently triggers this)");
                    globalObjectId = new GlobalObjectId();
                }

                if (globalObjectId.assetGUID == new GUID())
                {
                    //@TODO: How do we handle this
                    Debug.LogWarning($"{objectTable[i]} has no valid GUID. LiveLink currently does not support built-in assets.");
                    globalObjectId = new GlobalObjectId();
                }

                outAssets[i] = Unsafe.AsRef<RuntimeGlobalObjectId>(&globalObjectId);
            }
        }
#endif

    }
}