#if !UNITY_DOTSPLAYER
using UnityEngine;

#pragma warning disable 0660, 0661

namespace Unity.Mathematics
{
    public partial struct float2
    {
        public static implicit operator Vector2(float2 v)     { return new Vector2(v.x, v.y); }
        public static implicit operator float2(Vector2 v)     { return new float2(v.x, v.y); }
    }
    
    public partial struct float3
    {
        public static implicit operator Vector3(float3 v)     { return new Vector3(v.x, v.y, v.z); }
        public static implicit operator float3(Vector3 v)     { return new float3(v.x, v.y, v.z); }
    }

    public partial struct float4
    {
        public static implicit operator float4(Vector4 v)     { return new float4(v.x, v.y, v.z, v.w); }
        public static implicit operator Vector4(float4 v)     { return new Vector4(v.x, v.y, v.z, v.w); }
    }

    public partial struct quaternion
    {
        public static implicit operator Quaternion(quaternion q)  { return new Quaternion(q.value.x, q.value.y, q.value.z, q.value.w); }
        public static implicit operator quaternion(Quaternion q)  { return new quaternion(q.x, q.y, q.z, q.w); }
    }
    
    public partial struct float4x4
    {
        public static implicit operator float4x4(Matrix4x4 m) { return new float4x4(m.GetColumn(0), m.GetColumn(1), m.GetColumn(2), m.GetColumn(3)); }
        public static implicit operator Matrix4x4(float4x4 m) { return new Matrix4x4(m.c0, m.c1, m.c2, m.c3); }
    }
}
#endif
