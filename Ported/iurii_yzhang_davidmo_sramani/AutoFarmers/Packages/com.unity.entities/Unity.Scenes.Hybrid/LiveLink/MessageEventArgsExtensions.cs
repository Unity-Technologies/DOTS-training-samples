using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Networking.PlayerConnection;

namespace Unity.Scenes
{
    internal static class MessageEventArgsExtensions
    {
        unsafe static byte[] SerializeUnmanagedArray<T>(NativeArray<T> value) where T: unmanaged
        {
            var bytes = new byte[UnsafeUtility.SizeOf<T>() * value.Length + sizeof(int)];
            fixed (byte* ptr = bytes)
            {
                var buf = new UnsafeAppendBuffer(ptr, bytes.Length);
                buf.Add(value);
                Assert.AreEqual(buf.Size, bytes.Length);
            }

            return bytes;
        }
        
        unsafe static NativeArray<T> DeserializeUnmanagedArray<T>(byte[] buffer) where T : unmanaged
        {
            fixed (byte* ptr = buffer)
            {
                var buf = new UnsafeAppendBuffer.Reader(ptr, buffer.Length);
                buf.ReadNext<T>(out var array, Allocator.Temp);
                return array;
            }
        }

        unsafe static byte[] SerializeUnmanaged<T>(ref T value) where T: unmanaged
        {
            var bytes = new byte[UnsafeUtility.SizeOf<T>()];
            fixed (byte* ptr = bytes)
            {
                UnsafeUtility.CopyStructureToPtr(ref value, ptr);                                                
            }

            return bytes;
        }
        
        unsafe static T DeserializeUnmanaged<T>(byte[] buffer) where T : unmanaged
        {
            fixed (byte* ptr = buffer)
            {
                UnsafeUtility.CopyPtrToStructure<T>(ptr, out var value);
                return value;
            }
        }
        
        static public T Receive<T>(this MessageEventArgs args) where T : unmanaged
        {
            return DeserializeUnmanaged<T>(args.data);
        }
        
        static public NativeArray<T> ReceiveArray<T>(this MessageEventArgs args) where T : unmanaged
        {
            return DeserializeUnmanagedArray<T>(args.data);
        }
        
        static public void Send<T>(this PlayerConnection connection, Guid msgGuid, T data) where T : unmanaged
        {
            connection.Send(msgGuid, SerializeUnmanaged(ref data));
        }

        static public void SendArray<T>(this PlayerConnection connection, Guid msgGuid, NativeArray<T> data) where T : unmanaged
        {
            connection.Send(msgGuid, SerializeUnmanagedArray(data));
        }

#if UNITY_EDITOR        
        static public void Send<T>(this UnityEditor.Networking.PlayerConnection.EditorConnection connection, Guid msgGuid, T data, int playerId = 0) where T : unmanaged
        {
            connection.Send(msgGuid, SerializeUnmanaged(ref data), playerId);
        }

        static public void SendArray<T>(this UnityEditor.Networking.PlayerConnection.EditorConnection connection, Guid msgGuid, NativeArray<T> data, int playerId = 0) where T : unmanaged
        {
            connection.Send(msgGuid, SerializeUnmanagedArray(data), playerId);
        }
#endif
    }
}