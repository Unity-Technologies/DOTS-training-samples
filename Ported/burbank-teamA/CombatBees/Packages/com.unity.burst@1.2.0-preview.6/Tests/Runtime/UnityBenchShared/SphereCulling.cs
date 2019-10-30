using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

[assembly: InternalsVisibleTo("Burst.Benchmarks")]

namespace UnityBenchShared
{
    internal struct Sphere
    {
        private float x, y, z, r;

        public Sphere(float x, float y, float z, float r)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
        }

        public bool Intersects(Sphere other)
        {
            float dx = x - other.x;
            float dy = y - other.y;
            float dz = z - other.z;
            float rs = r + other.r;
            return dx * dx + dy * dy + dz * dz < (rs * rs);
        }
    }

    /// <summary>
    /// TODO: Rename to SphereCollision instead? (as the code is more an intersection than a culling)
    /// </summary>
    internal static class SphereCulling
    {
        public static int BenchCount = 256 * 1024 + 3;
    }

    internal interface IJob<T> : IJob
    {
        T Result { get; set; }
    }


    /// <summary>
    /// Simple AOS with a Sphere struct using plain floats
    /// </summary>
    internal struct SphereCullingAOSJob : IJob<bool>, IDisposable
    {
        public Sphere Against;

        [ReadOnly] public NativeArray<Sphere> Spheres;

        [MarshalAs(UnmanagedType.U1)]
        private bool result;

        public bool Result
        {
            get => result;
            set => result = value;
        }

        public void Execute()
        {
            bool result = false;
            for (int i = 0; i < Spheres.Length; ++i)
            {
                result |= Spheres[i].Intersects(Against);
            }

            Result = result;
        }

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    int length = SphereCulling.BenchCount;
                    var job = new SphereCullingAOSJob()
                    {
                        Spheres = new NativeArray<Sphere>(length, Allocator.Persistent),
                        Against = new Sphere(0, 0, 0, 1)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < job.Spheres.Length; i++)
                    {
                        // Most don't intersects
                        var x = random.Next(100) + 3.0f;
                        if (i == job.Spheres.Length / 2)
                        {
                            // only this one
                            x = 0.5f;
                        }
                        job.Spheres[i] = new Sphere(x, x, x, 1.0f);
                    }

                    return job;
                }
            }
        }

        public void Dispose()
        {
            Spheres.Dispose();
        }
    }

    /// <summary>
    /// Simple AOS with a float4 for the struct and using
    /// </summary>
    internal struct SphereCullingFloat4AOSJob : IJob<bool>, IDisposable
    {
        public float4 Against;

        [ReadOnly]
        public NativeArray<float4> Spheres;

        [MarshalAs(UnmanagedType.U1)]
        private bool result;

        public bool Result
        {
            get => result;
            set => result = value;
        }

        public static bool Intersects(float4 value, float4 other)
        {
            float rs = value.w + other.w;
            return math.dot(value.xyz, other.xyz) < (rs * rs);
        }

        public void Execute()
        {
            bool result = false;
            for (int i = 0; i < Spheres.Length; ++i)
            {
                result |= Intersects(Spheres[i], Against);
            }

            Result = result;
        }

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    int length = SphereCulling.BenchCount;
                    var job = new SphereCullingFloat4AOSJob()
                    {
                        Spheres = new NativeArray<float4>(length, Allocator.Persistent),
                        Against = new float4(0, 0, 0, 1)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < job.Spheres.Length; i++)
                    {
                        // Most don't intersects
                        var x = random.Next(100) + 3.0f;
                        if (i == job.Spheres.Length / 2)
                        {
                            // only this one
                            x = 0.5f;
                        }
                        job.Spheres[i] = new float4(x, x, x, 1.0f);
                    }

                    return job;
                }
            }
        }

        public void Dispose()
        {
            Spheres.Dispose();
        }
    }

    /// <summary>
    /// Simple SOA with 4 NativeArray for X,Y,Z,R
    /// </summary>
    internal struct SphereCullingSOAJob : IJob<bool>, IDisposable
    {
        [ReadOnly] public NativeArray<float> X;
        [ReadOnly] public NativeArray<float> Y;
        [ReadOnly] public NativeArray<float> Z;
        [ReadOnly] public NativeArray<float> R;

        public float4 Against;

        [MarshalAs(UnmanagedType.U1)]
        private bool result;

        public bool Result
        {
            get => result;
            set => result = value;
        }

        public void Execute()
        {
            bool result = false;

            for (int i = 0; i < X.Length; ++i)
            {
                float dx = X[i] - Against.x;
                float dy = Y[i] - Against.y;
                float dz = Z[i] - Against.z;
                float rs = R[i] + Against.w;
                result |= dx * dx + dy * dy + dz * dz < (rs * rs);
            }

            Result = result;
        }

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    int length = SphereCulling.BenchCount;
                    var job = new SphereCullingSOAJob()
                    {
                        X = new NativeArray<float>(length, Allocator.Persistent),
                        Y = new NativeArray<float>(length, Allocator.Persistent),
                        Z = new NativeArray<float>(length, Allocator.Persistent),
                        R = new NativeArray<float>(length, Allocator.Persistent),
                        Against = new float4(0, 0, 0, 1)
                    };


                    var random = new System.Random(0);
                    for (int i = 0; i < job.X.Length; i++)
                    {
                        // Most don't intersects
                        var x = random.Next(100) + 3.0f;
                        if (i == job.X.Length / 2)
                        {
                            // only this one
                            x = 0.5f;
                        }
                        job.X[i] = x;
                        job.Y[i] = x;
                        job.Z[i] = x;
                        job.R[i] = 1;
                    }

                    return job;
                }
            }
        }

        public void Dispose()
        {
            X.Dispose();
            Y.Dispose();
            Z.Dispose();
            R.Dispose();
        }
    }

    /// <summary>
    /// SOA with chunks of x,y,z,r using `float4`
    /// </summary>
    internal struct SphereCullingChunkSOAJob : IJob<bool>, IDisposable
    {
        public struct Chunk
        {
            public float4 X;
            public float4 Y;
            public float4 Z;
            public float4 R;
        }

        [ReadOnly] public NativeArray<Chunk> Chunks;

        public float4 Against;

        [MarshalAs(UnmanagedType.U1)]
        private bool result;

        public bool Result
        {
            get => result;
            set => result = value;
        }

        public void Execute()
        {
            bool result = false;

            for (int i = 0; i < Chunks.Length; ++i)
            {
                var chunk = Chunks[i];
                for (int j = 0; j < 4; j++)
                {
                    float dx = chunk.X[j] - Against.x;
                    float dy = chunk.Y[j] - Against.y;
                    float dz = chunk.Z[j] - Against.z;
                    float rs = chunk.R[j] + Against.w;
                    result |= dx * dx + dy * dy + dz * dz < (rs * rs);
                }
            }

            Result = result;
        }

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    // Approximate a similar batch
                    int length = (SphereCulling.BenchCount + 4) / 4;
                    var job = new SphereCullingChunkSOAJob
                    {
                        Chunks = new NativeArray<SphereCullingChunkSOAJob.Chunk>(length, Allocator.Persistent),
                        Against = new float4(0, 0, 0, 1)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < job.Chunks.Length; i++)
                    {
                        var chunk = job.Chunks[i];
                        for (int j = 0; j < 4; j++)
                        {
                            // Most don't intersects
                            var x = random.Next(100) + 3.0f;
                            if (i == job.Chunks.Length / 2)
                            {
                                // only this one
                                x = 0.5f;
                            }

                            chunk.X[j] = x;
                            chunk.Y[j] = x;
                            chunk.Z[j] = x;
                            chunk.R[j] = 1;
                        }
                        job.Chunks[i] = chunk;
                    }

                    return job;
                }
            }
        }

        public void Dispose()
        {
            Chunks.Dispose();
        }
    }

    /// <summary>
    /// SOA with chunks of x,y,z,r using `fixed float x[4]`
    /// </summary>
    internal struct SphereCullingChunkFixedSOAJob : IJob<bool>, IDisposable
    {
        public unsafe struct Chunk
        {
            public fixed float X[4];
            public fixed float Y[4];
            public fixed float Z[4];
            public fixed float R[4];
        }

        [ReadOnly] public NativeArray<Chunk> Chunks;

        public float4 Against;

        [MarshalAs(UnmanagedType.U1)]
        private bool result;

        public bool Result
        {
            get => result;
            set => result = value;
        }

        public unsafe void Execute()
        {
            bool result = false;

            for (int i = 0; i < Chunks.Length; ++i)
            {
                var chunk = Chunks[i];
                for (int j = 0; j < 4; j++)
                {
                    float dx = chunk.X[j] - Against.x;
                    float dy = chunk.Y[j] - Against.y;
                    float dz = chunk.Z[j] - Against.z;
                    float rs = chunk.R[j] + Against.w;
                    result |= dx * dx + dy * dy + dz * dz < (rs * rs);
                }
            }

            Result = result;
        }

        public struct Provider : IArgumentProvider
        {
            public unsafe object Value
            {
                get
                {
                    // Approximate a similar batch
                    int length = (SphereCulling.BenchCount + 4) / 4;
                    var job = new SphereCullingChunkFixedSOAJob
                    {
                        Chunks = new NativeArray<Chunk>(length, Allocator.Persistent),
                        Against = new float4(0, 0, 0, 1)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < job.Chunks.Length; i++)
                    {
                        var chunk = job.Chunks[i];
                        for (int j = 0; j < 4; j++)
                        {
                            // Most don't intersects
                            var x = random.Next(100) + 3.0f;
                            if (i == job.Chunks.Length / 2)
                            {
                                // only this one
                                x = 0.5f;
                            }
                            chunk.X[j] = x;
                            chunk.Y[j] = x;
                            chunk.Z[j] = x;
                            chunk.R[j] = 1;
                        }
                        job.Chunks[i] = chunk;
                    }

                    return job;
                }
            }
        }

        public void Dispose()
        {
            Chunks.Dispose();
        }
    }

    /// <summary>
    /// </summary>
    internal struct SphereCullingChunkSOAManualJob : IJob<bool>, IDisposable
    {
        public struct Chunk
        {
            public float4 X;
            public float4 Y;
            public float4 Z;
            public float4 R;
        }

        [ReadOnly] public NativeArray<Chunk> Chunks;

        public float4 Against;

        [MarshalAs(UnmanagedType.U1)]
        private bool result;

        public bool Result
        {
            get => result;
            set => result = value;
        }

        public void Execute()
        {
            bool4 result = false;

            for (int i = 0; i < Chunks.Length; ++i)
            {
                var chunk = Chunks[i];
                float4 dx = chunk.X - Against.x;
                float4 dy = chunk.Y - Against.y;
                float4 dz = chunk.Z - Against.z;
                float4 rs = chunk.R + Against.w;
                result |= dx * dx + dy * dy + dz * dz < (rs * rs);
            }

            Result = math.any(result);
        }

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    // Approximate a similar batch
                    int length = (SphereCulling.BenchCount + 4) / 4;
                    var job = new SphereCullingChunkSOAManualJob
                    {
                        Chunks = new NativeArray<SphereCullingChunkSOAManualJob.Chunk>(length, Allocator.Persistent),
                        Against = new float4(0, 0, 0, 1)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < job.Chunks.Length; i++)
                    {
                        var chunk = job.Chunks[i];
                        for (int j = 0; j < 4; j++)
                        {
                            // Most don't intersects
                            var x = random.Next(100) + 3.0f;
                            if (i == job.Chunks.Length / 2)
                            {
                                // only this one
                                x = 0.5f;
                            }

                            chunk.X[j] = x;
                            chunk.Y[j] = x;
                            chunk.Z[j] = x;
                            chunk.R[j] = 1;
                        }
                        job.Chunks[i] = chunk;
                    }

                    return job;
                }
            }
        }

        public void Dispose()
        {
            Chunks.Dispose();
        }
    }
}
