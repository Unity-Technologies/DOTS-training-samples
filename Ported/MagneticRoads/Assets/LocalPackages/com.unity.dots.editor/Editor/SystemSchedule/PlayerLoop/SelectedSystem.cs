using UnityEngine;
using System;

namespace Unity.Entities.Editor
{
    unsafe struct SelectedSystem : IEquatable<SelectedSystem>
    {
        public struct UnmanagedData
        {
            public World World;
            public SystemHandleUntyped Handle;
        }

        public readonly ComponentSystemBase Managed;
        public readonly UnmanagedData Unmanaged;

        public SelectedSystem(ComponentSystemBase b)
        {
            Managed = b;
            Unmanaged = default;
        }

        public SelectedSystem(SystemHandleUntyped h, World w)
        {
            Managed = null;
            Unmanaged.Handle = h;
            Unmanaged.World = w;
        }

        public SystemState* StatePointer
        {
            get
            {
                if (Managed != null)
                    return Managed.m_StatePtr;
                else if (Unmanaged.World != null)

                    // FIX: Not available in our version of ECS
                    //return Unmanaged.World.ResolveSystemState(Unmanaged.Handle);
                    return null;
                return null;
            }
        }

        public World World
        {
            get
            {
                var ptr = StatePointer;
                if (ptr != null)
                    return ptr->World;
                return null;
            }
        }

        public Type GetSystemType()
        {
            return Managed != null ? Managed.GetType() : SystemBaseRegistry.GetStructType(StatePointer->UnmanagedMetaIndex);
        }

        public bool Valid => Managed != null || Unmanaged.World != null;

        public bool Equals(SelectedSystem other)
        {
            return ReferenceEquals(Managed, other.Managed) && Unmanaged.Handle == other.Unmanaged.Handle;
        }

        public override int GetHashCode()
        {
            return Managed != null ? Managed.GetHashCode() : Unmanaged.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SelectedSystem sel)
            {
                return Equals(sel);
            }

            return false;
        }

        public static bool operator ==(SelectedSystem lhs, SelectedSystem rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SelectedSystem lhs, SelectedSystem rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static implicit operator SelectedSystem(ComponentSystemBase arg) => new SelectedSystem(arg);

        public override string ToString()
        {
            return GetSystemType().ToString();
        }
    }
}

