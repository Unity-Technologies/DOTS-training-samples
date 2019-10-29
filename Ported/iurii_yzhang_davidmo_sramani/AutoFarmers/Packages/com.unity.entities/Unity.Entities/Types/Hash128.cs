using System;
using Unity.Mathematics;

namespace Unity.Entities
{
    [Serializable]
    public struct Hash128 : IEquatable<Hash128>, IComparable<Hash128>
    {
        public uint4 Value;

        static readonly char[] k_HexToLiteral = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};
        
        public Hash128(uint4 value) => Value = value;
        public Hash128(uint x, uint y, uint z, uint w) => Value = new uint4(x, y, z, w); 

        /// <summary>
        /// Construct a hash from a 32 character hex string
        /// If the string has the incorrect length or non-hex characters the Value will be all 0 
        /// </summary>
        unsafe public Hash128(string value)
        {
            fixed(char* ptr = value)
            {
                Value = StringToHash(ptr, value.Length);
            }
        }


        public override unsafe string ToString()
        {
            var chars = stackalloc char[32]; 

            for (int i = 0; i < 4; i++)
            {
                for (int j = 7; j >= 0;j--)
                {
                    uint cur = Value[i];
                    cur >>= (j* 4);
                    cur &= 0xF;
                    chars[i * 8 + j] = k_HexToLiteral[cur];
                }
            }
            
            return new string(chars, 0, 32);            
        }
        
        static readonly sbyte[] s_LiteralToHex = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -1, -1, -1, -1, -1, -1, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        const int kGUIDStringLength = 32;

        unsafe static uint4 StringToHash(char* guidString, int length)
        {
            if (length != kGUIDStringLength)
                return default;

            // Convert every hex char into an int [0...16]
            var hex = stackalloc int[kGUIDStringLength];
            for (int i = 0; i < kGUIDStringLength; i++)
            {
                int intValue = guidString[i];
                if (intValue < 0 || intValue > 255)
                    return default;
                
                hex[i] = s_LiteralToHex[(int)guidString[i]];
            }
                
            uint4 value = default;
            for (int i = 0; i < 4; i++)
            {
                uint cur = 0;
                for (int j = 7; j >= 0 ;j--)
                {
                    int curHex = hex[i * 8 + j];
                    if (curHex == -1)
                        return default;

                    cur |= (uint)(curHex << (j * 4));
                }
                value[i] = cur;
            }
            return value;
        }
        
        public static bool operator== (Hash128 obj1, Hash128 obj2)
        {
            return obj1.Value.Equals(obj2.Value);
        }

        public static bool operator!= (Hash128 obj1, Hash128 obj2)
        {
            return !obj1.Value.Equals(obj2.Value);
        }

        public bool Equals(Hash128 obj)
        {
            return Value.Equals(obj.Value);
        }

        public override bool Equals(object obj)
        {
            throw new InvalidOperationException("Calling this function is a sign of inadvertent boxing");
        }

        public static bool operator <(Hash128 a, Hash128 b)
        {
            if (a.Value.w != b.Value.w)
                return a.Value.w < b.Value.w;
            if (a.Value.z != b.Value.z)
                return a.Value.z < b.Value.z;
            if (a.Value.y != b.Value.y)
                return a.Value.y < b.Value.y;
            return a.Value.x < b.Value.x;
        }

        public static bool operator >(Hash128 a, Hash128 b)
        {
            if (a.Value.w != b.Value.w)
                return a.Value.w > b.Value.w;
            if (a.Value.z != b.Value.z)
                return a.Value.z > b.Value.z;
            if (a.Value.y != b.Value.y)
                return a.Value.y > b.Value.y;
            return a.Value.x > b.Value.x;
        }
        
        public int CompareTo(Hash128 other)
        {
            if (Value.w != other.Value.w)
                return Value.w < other.Value.w ? -1 : 1;
            if (Value.z != other.Value.z)
                return Value.z < other.Value.z ? -1 : 1;
            if (Value.y != other.Value.y)
                return Value.y < other.Value.y ? -1 : 1;
            if (Value.x != other.Value.x)
                return Value.x < other.Value.x ? -1 : 1;
            return 0;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode (readonly fields will not get serialized by unity)
        public override int GetHashCode() => Value.GetHashCode();

        public bool IsValid => !Value.Equals(uint4.zero);
        
        #if UNITY_EDITOR
        public static unsafe implicit operator Hash128(UnityEditor.GUID guid) => *(Hash128*)&guid;
        public static unsafe implicit operator UnityEditor.GUID(Hash128 guid) => *(UnityEditor.GUID*)&guid;
        #endif

        #if UNITY_2019_1_OR_NEWER
        public static unsafe implicit operator Hash128(UnityEngine.Hash128 guid) => *(Hash128*)&guid;
        public static unsafe implicit operator UnityEngine.Hash128(Hash128 guid) => *(UnityEngine.Hash128*)&guid;
        #endif
    }
}
