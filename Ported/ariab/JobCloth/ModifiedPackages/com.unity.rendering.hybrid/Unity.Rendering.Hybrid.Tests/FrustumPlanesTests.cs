
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;
using static Unity.Mathematics.math;
using System;

namespace Unity.Entities.Tests
{
    public class FrustumPlanesTests
    {
        static readonly Plane[] Planes = {
            new Plane(new Vector3( 1.0f, 0.0f, 0.0f), -1.0f),
            new Plane(new Vector3(-1.0f, 0.0f, 0.0f),  1.0f),
            new Plane(new Vector3( 0.0f, 1.0f, 0.0f), -1.0f),
            new Plane(new Vector3( 0.0f,-1.0f, 0.0f),  1.0f),
            new Plane(new Vector3( 0.0f,-1.0f, 1.0f),  1.0f),
            new Plane(new Vector3( 0.0f,-2.0f, 0.0f),  12.0f),
            new Plane(new Vector3( 1.0f,-1.0f,-7.0f), -12.0f),
            new Plane(new Vector3( 0.0f, 183.0f,-7.0f), -12.0f),
            new Plane(new Vector3(0.9933293f, 0.01911314f, 0.113717f), -409.9551f),
        };

        static readonly AABB[] boxes = {
            new AABB { Center = new float3( 0.0f, 0.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(-1.0f, 0.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(-2.0f, 0.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f,-2.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f,-1.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f, 1.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f, 2.0f, 0.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f,0.0f,-2.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f,0.0f,-1.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f,0.0f, 1.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f,0.0f, 2.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(1.0f,-1.0f, 1.0f), Extents = new float3(0.5f, 0.5f, 0.5f) },
            new AABB { Center = new float3(0.0f, 0.0f, 0.0f), Extents = new float3(16384.0f, 16384.0f, 16384.0f) },
            new AABB { Center = new float3(-325.303f, 391.993f, 1053.86f), Extents = new float3(22.32453f, 18.56214f, 23.49754f) },

        };

        static NativeArray<Plane> CreatePlanes(int n)
        {
            var result = new NativeArray<Plane>(n, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < n; ++i)
            {
                result[i] = Planes[i];
            }
            return result;
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        public void MultiPlaneTest(int planeCount)
        {
            using (var par = CreatePlanes(planeCount))
            using (var soap = FrustumPlanes.BuildSOAPlanePackets(par, Allocator.Temp))
            {
                foreach (var box in boxes)
                {
                    Assert.AreEqual(ReferenceTest(par, box), FrustumPlanes.Intersect2(soap, box));
                }
            }
        }

        private FrustumPlanes.IntersectResult ReferenceTest(NativeArray<Plane> par, AABB box)
        {
            FrustumPlanes.IntersectResult result;
            var temp = new NativeArray<float4>(par.Length, Allocator.Temp);

            for (int i = 0; i < par.Length; ++i)
            {
                temp[i] = new float4(par[i].normal, par[i].distance);
            }

            result = FrustumPlanes.Intersect(temp, box);

            temp.Dispose();
            return result;
        }
    }
}

