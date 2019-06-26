using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Rendering
{
    public struct FrustumPlanes
    {
        public enum IntersectResult
        {
            Out,
            In,
            Partial
        };

        static public void FromCamera(Camera camera, NativeArray<float4> planes)
        {
            Plane[] sourcePlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            for (int i = 0; i < 6; ++i)
            {
                planes[i] = new float4(sourcePlanes[i].normal.x, sourcePlanes[i].normal.y, sourcePlanes[i].normal.z,
                    sourcePlanes[i].distance);
            }
        }
        
        static public IntersectResult Intersect(NativeArray<float4> cullingPlanes, AABB a)
        {
            float3 m = a.Center;
            float3 extent = a.Extents;

            var inCount = 0;
            for (int i = 0; i < cullingPlanes.Length; i++)
            {
                float3 normal = cullingPlanes[i].xyz;
                float dist = math.dot(normal, m) + cullingPlanes[i].w;
                float radius = math.dot(extent, math.abs(normal));
                if (dist + radius <= 0)
                    return IntersectResult.Out;
                
                if (dist > radius)
                    inCount++;
                
            }
            
            return (inCount == cullingPlanes.Length) ? IntersectResult.In : IntersectResult.Partial;
        }

        public struct PlanePacket4
        {
            public float4 Xs;
            public float4 Ys;
            public float4 Zs;
            public float4 Distances;
        }

        public static NativeArray<PlanePacket4> BuildSOAPlanePackets(NativeArray<Plane> cullingPlanes, Allocator allocator)
        {
            int cullingPlaneCount = cullingPlanes.Length;
            int packetCount = (cullingPlaneCount + 3) >> 2;
            var planes = new NativeArray<PlanePacket4>(packetCount, allocator, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < cullingPlaneCount; i++)
            {
                var p = planes[i >> 2];
                p.Xs[i & 3] = cullingPlanes[i].normal.x;
                p.Ys[i & 3] = cullingPlanes[i].normal.y;
                p.Zs[i & 3] = cullingPlanes[i].normal.z;
                p.Distances[i & 3] = cullingPlanes[i].distance;
                planes[i >> 2] = p;
            }

            // Populate the remaining planes with values that are always "in"
            for (int i = cullingPlaneCount; i < 4 * packetCount; ++i)
            {
                var p = planes[i >> 2];
                p.Xs[i & 3] = 1.0f;
                p.Ys[i & 3] = 0.0f;
                p.Zs[i & 3] = 0.0f;
                p.Distances[i & 3] = 32786.0f; //float.MaxValue;
                planes[i >> 2] = p;
            }

            return planes;
        }

        static public IntersectResult Intersect2(NativeArray<PlanePacket4> cullingPlanePackets, AABB a)
        {
            float4 mx = a.Center.xxxx;
            float4 my = a.Center.yyyy;
            float4 mz = a.Center.zzzz;

            float4 ex = a.Extents.xxxx;
            float4 ey = a.Extents.yyyy;
            float4 ez = a.Extents.zzzz;

            int4 outCounts = 0;
            int4 inCounts = 0;

            for (int i = 0; i < cullingPlanePackets.Length; i++)
            {
                var p = cullingPlanePackets[i];
                float4 distances = dot4(p.Xs, p.Ys, p.Zs, mx, my, mz) + p.Distances;
                float4 radii = dot4(ex, ey, ez, math.abs(p.Xs), math.abs(p.Ys), math.abs(p.Zs));

                outCounts += (int4) (distances + radii <= 0);
                inCounts += (int4) (distances > radii);
            }

            int inCount = math.csum(inCounts);
            int outCount = math.csum(outCounts);

            if (outCount != 0)
                return IntersectResult.Out;
            else
                return (inCount == 4 * cullingPlanePackets.Length) ? IntersectResult.In : IntersectResult.Partial;
        }

        static public IntersectResult Intersect2NoPartial(NativeArray<PlanePacket4> cullingPlanePackets, AABB a)
        {
            float4 mx = a.Center.xxxx;
            float4 my = a.Center.yyyy;
            float4 mz = a.Center.zzzz;

            float4 ex = a.Extents.xxxx;
            float4 ey = a.Extents.yyyy;
            float4 ez = a.Extents.zzzz;

            int4 masks = 0;

            for (int i = 0; i < cullingPlanePackets.Length; i++)
            {
                var p = cullingPlanePackets[i];
                float4 distances = dot4(p.Xs, p.Ys, p.Zs, mx, my, mz) + p.Distances;
                float4 radii = dot4(ex, ey, ez, math.abs(p.Xs), math.abs(p.Ys), math.abs(p.Zs));

                masks += (int4) (distances + radii <= 0);
            }

            int outCount = math.csum(masks);
            return outCount > 0 ? IntersectResult.Out : IntersectResult.In;
        }

        private static float4 dot4(float4 xs, float4 ys, float4 zs, float4 mx, float4 my, float4 mz)
        {
            return xs * mx + ys * my + zs * mz;
        }

        static public IntersectResult Intersect(NativeArray<float4> planes, float3 center, float radius)
        {
            var inCount = 0;

            for (int i = 0; i < planes.Length; i++)
            {
                var d = math.dot(planes[i].xyz, center) + planes[i].w;
                if (d < -radius)
                {
                    return IntersectResult.Out;
                }

                if (d > radius)
                {
                    inCount++;
                }
            }

            return (inCount == planes.Length) ? IntersectResult.In : IntersectResult.Partial;
        }
    }

}
