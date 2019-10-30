using NUnit.Framework;
using System;
using static Unity.Mathematics.math;
using Burst.Compiler.IL.Tests;

namespace Unity.Mathematics.Tests
{
    [TestFixture]
    class TestRandom
    {
        // Kolmogorov–Smirnov test on lambda assuming the ideal distribution is uniform [0, 1]
        private static void ks_test(Func<double> func, int num_buckets = 256)
        {
            const int N = 8192;
            var histogram = new int[num_buckets];

            for (int i = 0; i < N; i++)
            {
                double x = func();
                Assert.GreaterOrEqual(x, 0.0);
                Assert.LessOrEqual(x, 1.0);
                int bucket = min((int)(x * num_buckets), num_buckets - 1);

                histogram[bucket]++;
            }

            double largest_delta = 0.0f;
            int accum = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                accum += histogram[i];
                double current = accum / (double)N;
                double target = (double)(i + 1) / histogram.Length;
                largest_delta = math.max(largest_delta, math.abs(current - target));
            }
            double d = 1.62762 / math.sqrt((double)N);   // significance: 0.01
            Assert.Less(largest_delta, d);
        }

        private static void ks_test(Func<double2> func)
        {
            ks_test(() => func().x);
            ks_test(() => func().y);
        }

        // Pearson's product-moment coefficient
        private static void r_test(Func<double2> func)
        {
            const int N = 4096;

            double2 sum = 0.0;
            var values = new double2[N]; 
            for(int i = 0; i < N; i++)
            {
                values[i] = func();
                sum += values[i];
            }

            double2 avg = sum / N;
            double var_a = 0.0;
            double var_b = 0.0;
            double cov = 0.0;
            for (int i = 0; i < N; i++)
            {
                double2 delta = values[i] - avg;
                var_a += delta.x * delta.x;
                var_b += delta.y * delta.y;
                cov += delta.x * delta.y;
            }

            double r = cov / sqrt(var_a * var_b);
            Assert.Less(abs(r), 0.05);
        }

        private static float range_check01(float x)
        {
            Assert.GreaterOrEqual(x, 0.0f);
            Assert.Less(x, 1.0f);
            return x;
        }

        private static double range_check01(double x)
        {
            Assert.GreaterOrEqual(x, 0.0);
            Assert.Less(x, 1.0);
            return x;
        }

        private static int range_check(int x, int min, int max)
        {
            Assert.GreaterOrEqual(x, min);
            Assert.Less(x, max);
            return x;
        }

        private static uint range_check(uint x, uint min, uint max)
        {
            Assert.GreaterOrEqual(x, min);
            Assert.Less(x, max);
            return x;
        }

        private static float range_check(float x, float min, float max)
        {
            Assert.GreaterOrEqual(x, min);
            Assert.Less(x, max);
            return x;
        }

        private static double range_check(double x, double min, double max)
        {
            Assert.GreaterOrEqual(x, min);
            Assert.Less(x, max);
            return x;
        }


        [TestCompiler]
        public static void bool_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => rnd.NextBool() ? 0.75 : 0.25), 2);
        }

        [TestCompiler]
        public static void bool2_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => rnd.NextBool2().x ? 0.75 : 0.25), 2);
            ks_test((() => rnd.NextBool2().y ? 0.75 : 0.25), 2);
        }

        [TestCompiler]
        public static void bool2_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool2().xy)));
        }
        
        [TestCompiler]
        public static void bool3_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => rnd.NextBool3().x ? 0.75 : 0.25), 2);
            ks_test((() => rnd.NextBool3().y ? 0.75 : 0.25), 2);
            ks_test((() => rnd.NextBool3().z ? 0.75 : 0.25), 2);
        }

        [TestCompiler]
        public static void bool3_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool3().xy)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool3().xz)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool3().yz)));
        }

        [TestCompiler]
        public static void bool4_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => rnd.NextBool4().x ? 0.75 : 0.25), 2);
            ks_test((() => rnd.NextBool4().y ? 0.75 : 0.25), 2);
            ks_test((() => rnd.NextBool4().z ? 0.75 : 0.25), 2);
            ks_test((() => rnd.NextBool4().w ? 0.75 : 0.25), 2);
        }

        [TestCompiler]
        public static void bool4_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool4().xy)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool4().xz)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool4().xw)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool4().yz)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool4().yw)));
            r_test((() => select(double2(0.25), double2(0.75), rnd.NextBool4().zw)));
        }

        [TestCompiler]
        public static void int_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextInt() & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => (((uint)rnd.NextInt() >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int max = 17;
            ks_test((() => (range_check(rnd.NextInt(max), 0, max) + 0.5) / max), max);
        }

        [TestCompiler]
        public static void int_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int max = 2147483647;
            ks_test((() => (range_check(rnd.NextInt(max), 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void int_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int min = -7;
            int max = 17;
            int range = max - min;
            ks_test((() => (range_check(rnd.NextInt(min, max), min, max) + 0.5 - min) / range), range);
        }

        [TestCompiler]
        public static void int_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int min = -2147483648;
            int max = 2147483647;
            long range = (long)max - (long)min;
            ks_test((() => (range_check(rnd.NextInt(min, max), min, max) + 0.5 - min) / range), 256);
        }

        [TestCompiler]
        public static void int2_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextInt2().x & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextInt2().y & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int2_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => (((uint)rnd.NextInt2().x >> 24) + 0.5) / 256.0), 256);
            ks_test((() => (((uint)rnd.NextInt2().y >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int2_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int max = 2147483647;
            ks_test((() => (range_check(rnd.NextInt2(max).x, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextInt2(max).y, 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void int2_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int2 min = int2(-7, 3);
            int2 max = int2(17, 14);
            int2 range = max - min;
            ks_test((() => (range_check(rnd.NextInt2(min, max).x, min.x, max.x) + 0.5 - min.x) / range.x), range.x);
            ks_test((() => (range_check(rnd.NextInt2(min, max).y, min.y, max.y) + 0.5 - min.y) / range.y), range.y);
        }

        [TestCompiler]
        public static void int2_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int min = -2147483648;
            int max = 2147483647;
            long range = (long)max - (long)min;
            ks_test((() => (range_check(rnd.NextInt2(min, max).x, min, max) + 0.5 - min) / range), 256);
            ks_test((() => (range_check(rnd.NextInt2(min, max).y, min, max) + 0.5 - min) / range), 256);
        }

        [TestCompiler]
        public static void int2_independent_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextInt2().xy & 255));
        }

        [TestCompiler]
        public static void int2_independent_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => ((uint2)rnd.NextInt2().xy >> 24));
        }

        [TestCompiler]
        public static void int3_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextInt3().x & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextInt3().y & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextInt3().z & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int3_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => (((uint)rnd.NextInt3().x >> 24) + 0.5) / 256.0), 256);
            ks_test((() => (((uint)rnd.NextInt3().y >> 24) + 0.5) / 256.0), 256);
            ks_test((() => (((uint)rnd.NextInt3().z >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int3_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int3 max = int3(13, 17, 19);
            ks_test((() => ((uint)range_check(rnd.NextInt3(max).x, 0, max.x) + 0.5) / max.x), max.x);
            ks_test((() => ((uint)range_check(rnd.NextInt3(max).y, 0, max.y) + 0.5) / max.y), max.y);
            ks_test((() => ((uint)range_check(rnd.NextInt3(max).z, 0, max.z) + 0.5) / max.z), max.z);
        }

        [TestCompiler]
        public static void int3_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int max = 2147483647;
            ks_test((() => ((uint)range_check(rnd.NextInt3(max).x, 0, max) + 0.5) / max), 256);
            ks_test((() => ((uint)range_check(rnd.NextInt3(max).y, 0, max) + 0.5) / max), 256);
            ks_test((() => ((uint)range_check(rnd.NextInt3(max).z, 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void int3_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int3 min = int3(-7, 3, -10);
            int3 max = int3(17, 14, -3);
            int3 range = max - min;
            ks_test((() => (range_check(rnd.NextInt3(min, max).x, min.x, max.x) + 0.5 - min.x) / range.x), range.x);
            ks_test((() => (range_check(rnd.NextInt3(min, max).y, min.y, max.y) + 0.5 - min.y) / range.y), range.y);
            ks_test((() => (range_check(rnd.NextInt3(min, max).z, min.z, max.z) + 0.5 - min.z) / range.z), range.z);
        }

        [TestCompiler]
        public static void int3_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int min = -2147483648;
            int max = 2147483647;
            long range = (long)max - (long)min;
            ks_test((() => (range_check(rnd.NextInt3(min, max).x, min, max) + 0.5 - min) / range), 256);
            ks_test((() => (range_check(rnd.NextInt3(min, max).y, min, max) + 0.5 - min) / range), 256);
            ks_test((() => (range_check(rnd.NextInt3(min, max).z, min, max) + 0.5 - min) / range), 256);
        }

        [TestCompiler]
        public static void int3_independent_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextInt3().xy & 255));
            r_test(() => (rnd.NextInt3().xz & 255));
            r_test(() => (rnd.NextInt3().yz & 255));
        }

        [TestCompiler]
        public static void int3_independent_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => ((uint2)rnd.NextInt3().xy >> 24));
            r_test(() => ((uint2)rnd.NextInt3().xz >> 24));
            r_test(() => ((uint2)rnd.NextInt3().yz >> 24));
        }

        [TestCompiler]
        public static void int4_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextInt4().x & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextInt4().y & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextInt4().z & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextInt4().w & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int4_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => (((uint)rnd.NextInt4().x >> 24) + 0.5) / 256.0), 256);
            ks_test((() => (((uint)rnd.NextInt4().y >> 24) + 0.5) / 256.0), 256);
            ks_test((() => (((uint)rnd.NextInt4().z >> 24) + 0.5) / 256.0), 256);
            ks_test((() => (((uint)rnd.NextInt4().w >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void int4_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int4 max = int4(13, 17, 19, 23);
            ks_test((() => (range_check(rnd.NextInt4(max).x, 0, max.x) + 0.5) / max.x), max.x);
            ks_test((() => (range_check(rnd.NextInt4(max).y, 0, max.y) + 0.5) / max.y), max.y);
            ks_test((() => (range_check(rnd.NextInt4(max).z, 0, max.z) + 0.5) / max.z), max.z);
            ks_test((() => (range_check(rnd.NextInt4(max).w, 0, max.w) + 0.5) / max.w), max.w);
        }

        [TestCompiler]
        public static void int4_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int max = 2147483647;
            ks_test((() => (range_check(rnd.NextInt4(max).x, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextInt4(max).y, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextInt4(max).z, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextInt4(max).w, 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void int4_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            int4 min = int4(-7, 3, -10, 1);
            int4 max = int4(17, 14, -3, 111);
            int4 range = max - min;
            ks_test((() => (range_check(rnd.NextInt4(min, max).x, min.x, max.x) + 0.5 - min.x) / range.x), range.x);
            ks_test((() => (range_check(rnd.NextInt4(min, max).y, min.y, max.y) + 0.5 - min.y) / range.y), range.y);
            ks_test((() => (range_check(rnd.NextInt4(min, max).z, min.z, max.z) + 0.5 - min.z) / range.z), range.z);
            ks_test((() => (range_check(rnd.NextInt4(min, max).w, min.w, max.w) + 0.5 - min.w) / range.w), range.w);
        }

        [TestCompiler]
        public static void int4_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            int min = -2147483648;
            int max = 2147483647;
            long range = (long)max - (long)min;
            ks_test((() => (range_check(rnd.NextInt4(min, max).x, min, max) + 0.5 - min) / range), 256);
            ks_test((() => (range_check(rnd.NextInt4(min, max).y, min, max) + 0.5 - min) / range), 256);
            ks_test((() => (range_check(rnd.NextInt4(min, max).z, min, max) + 0.5 - min) / range), 256);
            ks_test((() => (range_check(rnd.NextInt4(min, max).w, min, max) + 0.5 - min) / range), 256);
        }

        [TestCompiler]
        public static void int4_independent_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt4().xy & 255));
            r_test(() => (rnd.NextUInt4().xz & 255));
            r_test(() => (rnd.NextUInt4().xw & 255));
            r_test(() => (rnd.NextUInt4().yz & 255));
            r_test(() => (rnd.NextUInt4().yw & 255));
            r_test(() => (rnd.NextUInt4().zw & 255));
        }

        [TestCompiler]
        public static void int4_independent_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => ((uint2)rnd.NextUInt4().xy >> 24));
            r_test(() => ((uint2)rnd.NextUInt4().xz >> 24));
            r_test(() => ((uint2)rnd.NextUInt4().xw >> 24));
            r_test(() => ((uint2)rnd.NextUInt4().yz >> 24));
            r_test(() => ((uint2)rnd.NextUInt4().yw >> 24));
            r_test(() => ((uint2)rnd.NextUInt4().zw >> 24));
        }


        [TestCompiler]
        public static void uint_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt() & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt() >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 17;
            ks_test((() => (rnd.NextUInt(max) + 0.5) / max), (int)max);
        }

        [TestCompiler]
        public static void uint_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (rnd.NextUInt(max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint min = 3;
            uint max = 17;
            uint range = max - min;
            ks_test((() => (range_check(rnd.NextUInt(min, max), min, max) + 0.5 - min) / range), (int)range);
        }

        [TestCompiler]
        public static void uint_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (range_check(rnd.NextUInt(0, max), 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint2_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt2().x & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt2().y & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint2_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt2().x >> 24) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt2().y >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint2_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint2 max = uint2(13, 17);
            ks_test((() => (rnd.NextUInt2(max).x + 0.5) / max.x), (int)max.x);
            ks_test((() => (rnd.NextUInt2(max).y + 0.5) / max.y), (int)max.y);
        }

        [TestCompiler]
        public static void uint2_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (rnd.NextUInt2(max).x + 0.5) / max), 256);
            ks_test((() => (rnd.NextUInt2(max).y + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint2_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint2 min = uint2(3, 101);
            uint2 max = uint2(17, 117);
            uint2 range = max - min;
            ks_test((() => (range_check(rnd.NextUInt2(min, max).x, min.x, max.x) + 0.5 - min.x) / range.x), (int)range.x);
            ks_test((() => (range_check(rnd.NextUInt2(min, max).y, min.y, max.y) + 0.5 - min.y) / range.y), (int)range.y);
        }

        [TestCompiler]
        public static void uint2_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (range_check(rnd.NextUInt2(0, max).x, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextUInt2(0, max).y, 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint2_independent_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt2().xy & 255));
        }

        [TestCompiler]
        public static void uint2_independent_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt2().xy >> 24));
        }

        [TestCompiler]
        public static void uint3_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt3().x & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt3().y & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt3().z & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint3_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt3().x >> 24) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt3().y >> 24) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt3().z >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint3_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint3 max = uint3(13, 17, 19);
            ks_test((() => (rnd.NextUInt3(max).x + 0.5) / max.x), (int)max.x);
            ks_test((() => (rnd.NextUInt3(max).y + 0.5) / max.y), (int)max.y);
            ks_test((() => (rnd.NextUInt3(max).z + 0.5) / max.z), (int)max.z);
        }

        [TestCompiler]
        public static void uint3_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (rnd.NextUInt3(max).x + 0.5) / max), 256);
            ks_test((() => (rnd.NextUInt3(max).y + 0.5) / max), 256);
            ks_test((() => (rnd.NextUInt3(max).z + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint3_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint3 min = uint3(3, 101, 0xFFFFFFF0);
            uint3 max = uint3(17, 117, 0xFFFFFFFF);
            uint3 range = max - min;
            ks_test((() => (range_check(rnd.NextUInt3(min, max).x, min.x, max.x) + 0.5 - min.x) / range.x), (int)range.x);
            ks_test((() => (range_check(rnd.NextUInt3(min, max).y, min.y, max.y) + 0.5 - min.y) / range.y), (int)range.y);
            ks_test((() => (range_check(rnd.NextUInt3(min, max).z, min.z, max.z) + 0.5 - min.z) / range.z), (int)range.z);
        }

        [TestCompiler]
        public static void uint3_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (range_check(rnd.NextUInt3(0, max).x, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextUInt3(0, max).y, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextUInt3(0, max).z, 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint3_independent_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt3().xy & 255));
            r_test(() => (rnd.NextUInt3().xz & 255));
            r_test(() => (rnd.NextUInt3().yz & 255));
        }

        [TestCompiler]
        public static void uint3_independent_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt3().xy >> 24));
            r_test(() => (rnd.NextUInt3().xz >> 24));
            r_test(() => (rnd.NextUInt3().yz >> 24));
        }

        [TestCompiler]
        public static void uint4_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt4().x & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt4().y & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt4().z & 255u) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt4().w & 255u) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint4_uniform_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => ((rnd.NextUInt4().x >> 24) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt4().y >> 24) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt4().z >> 24) + 0.5) / 256.0), 256);
            ks_test((() => ((rnd.NextUInt4().w >> 24) + 0.5) / 256.0), 256);
        }

        [TestCompiler]
        public static void uint4_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint4 max = uint4(13, 17, 19, 23);
            ks_test((() => (rnd.NextUInt4(max).x + 0.5) / max.x), (int)max.x);
            ks_test((() => (rnd.NextUInt4(max).y + 0.5) / max.y), (int)max.y);
            ks_test((() => (rnd.NextUInt4(max).z + 0.5) / max.z), (int)max.z);
            ks_test((() => (rnd.NextUInt4(max).w + 0.5) / max.w), (int)max.w);
        }

        [TestCompiler]
        public static void uint4_uniform_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (rnd.NextUInt4(max).x + 0.5) / max), 256);
            ks_test((() => (rnd.NextUInt4(max).y + 0.5) / max), 256);
            ks_test((() => (rnd.NextUInt4(max).z + 0.5) / max), 256);
            ks_test((() => (rnd.NextUInt4(max).w + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint4_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            uint4 min = uint4(3, 101, 0xFFFFFFF0, 100);
            uint4 max = uint4(17, 117, 0xFFFFFFFF, 1000);
            uint4 range = max - min;
            ks_test((() => (range_check(rnd.NextUInt4(min, max).x, min.x, max.x) + 0.5 - min.x) / range.x), (int)range.x);
            ks_test((() => (range_check(rnd.NextUInt4(min, max).y, min.y, max.y) + 0.5 - min.y) / range.y), (int)range.y);
            ks_test((() => (range_check(rnd.NextUInt4(min, max).z, min.z, max.z) + 0.5 - min.z) / range.z), (int)range.z);
            ks_test((() => (range_check(rnd.NextUInt4(min, max).w, min.w, max.w) + 0.5 - min.w) / range.w), (int)range.w);
        }

        [TestCompiler]
        public static void uint4_uniform_min_max_limit()
        {
            var rnd = new Random(0x6E624EB7u);
            uint max = 0xFFFFFFFF;
            ks_test((() => (range_check(rnd.NextUInt4(0, max).x, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextUInt4(0, max).y, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextUInt4(0, max).z, 0, max) + 0.5) / max), 256);
            ks_test((() => (range_check(rnd.NextUInt4(0, max).w, 0, max) + 0.5) / max), 256);
        }

        [TestCompiler]
        public static void uint4_independent_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt4().xy & 255));
            r_test(() => (rnd.NextUInt4().xz & 255));
            r_test(() => (rnd.NextUInt4().xw & 255));
            r_test(() => (rnd.NextUInt4().yz & 255));
            r_test(() => (rnd.NextUInt4().yw & 255));
            r_test(() => (rnd.NextUInt4().zw & 255));
        }

        [TestCompiler]
        public static void uint4_independent_high_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test(() => (rnd.NextUInt4().xy >> 24));
            r_test(() => (rnd.NextUInt4().xz >> 24));
            r_test(() => (rnd.NextUInt4().xw >> 24));
            r_test(() => (rnd.NextUInt4().yz >> 24));
            r_test(() => (rnd.NextUInt4().yw >> 24));
            r_test(() => (rnd.NextUInt4().zw >> 24));
        }

        [TestCompiler]
        public static void float_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextFloat())));
        }

        [TestCompiler]
        public static void float_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextFloat() * 65536.0f)));
        }

        [TestCompiler]
        public static void float_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float max = 16.4f;
            ks_test((() => range_check(rnd.NextFloat(max), 0.0f, max) / max));
        }

        [TestCompiler]
        public static void float_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float min = -3.1f;
            float max = 16.4f;
            float range = max - min;
            ks_test((() => (range_check(rnd.NextFloat(min, max), min, max) -min) / range));
        }

        [TestCompiler]
        public static void float2_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextFloat2().x)));
            ks_test((() => range_check01(rnd.NextFloat2().y)));
        }

        [TestCompiler]
        public static void float2_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextFloat2().x * 65536.0f)));
            ks_test((() => frac(rnd.NextFloat2().y * 65536.0f)));
        }

        [TestCompiler]
        public static void float2_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float2 max = float2(16.4f, 1001.33333333f);
            ks_test((() => range_check(rnd.NextFloat2(max).x, 0.0f, max.x) / max.x));
            ks_test((() => range_check(rnd.NextFloat2(max).y, 0.0f, max.y) / max.y));
        }

        [TestCompiler]
        public static void float2_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float2 min = float2(-3.1f, 17.1f);
            float2 max = float2(16.4f, 1001.33333333f);
            float2 range = max - min;
            ks_test((() => (range_check(rnd.NextFloat2(min, max).x, min.x, max.x) - min.x) / range.x));
            ks_test((() => (range_check(rnd.NextFloat2(min, max).y, min.y, max.y) - min.y) / range.y));
        }

        [TestCompiler]
        public static void float2_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => rnd.NextFloat2()));
        }

        [TestCompiler]
        public static void float3_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextFloat3().x)));
            ks_test((() => range_check01(rnd.NextFloat3().y)));
            ks_test((() => range_check01(rnd.NextFloat3().z)));
        }

        [TestCompiler]
        public static void float3_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextFloat3().x * 65536.0f)));
            ks_test((() => frac(rnd.NextFloat3().y * 65536.0f)));
            ks_test((() => frac(rnd.NextFloat3().z * 65536.0f)));
        }

        [TestCompiler]
        public static void float3_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float3 max = float3(16.4f, 1001.33333333f, 3.121f);
            ks_test((() => range_check(rnd.NextFloat3(max).x, 0.0f, max.x) / max.x));
            ks_test((() => range_check(rnd.NextFloat3(max).y, 0.0f, max.y) / max.y));
            ks_test((() => range_check(rnd.NextFloat3(max).z, 0.0f, max.z) / max.z));
        }

        [TestCompiler]
        public static void float3_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float3 min = float3(-3.1f, 17.1f, -0.3f);
            float3 max = float3(16.4f, 1001.33333333f, 3.121f);
            float3 range = max - min;
            ks_test((() => (range_check(rnd.NextFloat3(min, max).x, min.x, max.x) - min.x) / range.x));
            ks_test((() => (range_check(rnd.NextFloat3(min, max).y, min.y, max.y) - min.y) / range.y));
            ks_test((() => (range_check(rnd.NextFloat3(min, max).z, min.z, max.z) - min.z) / range.z));
        }

        [TestCompiler]
        public static void float3_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => rnd.NextFloat3().xy));
            r_test((() => rnd.NextFloat3().xz));
            r_test((() => rnd.NextFloat3().yz));
        }

        [TestCompiler]
        public static void float4_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextFloat4().x)));
            ks_test((() => range_check01(rnd.NextFloat4().y)));
            ks_test((() => range_check01(rnd.NextFloat4().z)));
            ks_test((() => range_check01(rnd.NextFloat4().w)));
        }

        [TestCompiler]
        public static void float4_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextFloat4().x * 65536.0f)));
            ks_test((() => frac(rnd.NextFloat4().y * 65536.0f)));
            ks_test((() => frac(rnd.NextFloat4().z * 65536.0f)));
            ks_test((() => frac(rnd.NextFloat4().w * 65536.0f)));
        }

        [TestCompiler]
        public static void float4_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float4 max = float4(16.4f, 1001.33333333f, 3.121f, 1.3232e23f);
            ks_test((() => range_check(rnd.NextFloat4(max).x, 0.0f, max.x) / max.x));
            ks_test((() => range_check(rnd.NextFloat4(max).y, 0.0f, max.y) / max.y));
            ks_test((() => range_check(rnd.NextFloat4(max).z, 0.0f, max.z) / max.z));
            ks_test((() => range_check(rnd.NextFloat4(max).w, 0.0f, max.w) / max.w));
        }

        [TestCompiler]
        public static void float4_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            float4 min = float4(-3.1f, 17.1f, -0.3f, -22.6f);
            float4 max = float4(16.4f, 1001.33333333f, 3.121f, 1.3232e23f);
            float4 range = max - min;
            ks_test((() => (range_check(rnd.NextFloat4(min, max).x, min.x, max.x) - min.x) / range.x));
            ks_test((() => (range_check(rnd.NextFloat4(min, max).y, min.y, max.y) - min.y) / range.y));
            ks_test((() => (range_check(rnd.NextFloat4(min, max).z, min.z, max.z) - min.z) / range.z));
            ks_test((() => (range_check(rnd.NextFloat4(min, max).w, min.w, max.w) - min.w) / range.w));
        }

        [TestCompiler]
        public static void float4_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => rnd.NextFloat4().xy));
            r_test((() => rnd.NextFloat4().xz));
            r_test((() => rnd.NextFloat4().xw));
            r_test((() => rnd.NextFloat4().yz));
            r_test((() => rnd.NextFloat4().yw));
            r_test((() => rnd.NextFloat4().zw));
        }

        [TestCompiler]
        public static void double_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextDouble())));
        }

        [TestCompiler]
        public static void double_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextDouble() * 35184372088832.0)));
        }

        [TestCompiler]
        public static void double_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double max = 16.4;
            ks_test((() => range_check(rnd.NextDouble(max), 0.0, max) / max));
        }

        [TestCompiler]
        public static void double_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double min = -3.1;
            double max = 16.4;
            double range = max - min;
            ks_test((() => (range_check(rnd.NextDouble(min, max), min, max) - min) / range));
        }

        [TestCompiler]
        public static void double2_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextDouble2().x)));
            ks_test((() => range_check01(rnd.NextDouble2().y)));
        }

        [TestCompiler]
        public static void double2_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextDouble2().x * 35184372088832.0)));
            ks_test((() => frac(rnd.NextDouble2().y * 35184372088832.0)));
        }

        [TestCompiler]
        public static void double2_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double2 max = double2(16.4, 1001.33333333);
            ks_test((() => range_check(rnd.NextDouble2(max).x, 0.0, max.x) / max.x));
            ks_test((() => range_check(rnd.NextDouble2(max).y, 0.0, max.y) / max.y));
        }

        [TestCompiler]
        public static void double2_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double2 min = double2(-3.1, 17.1);
            double2 max = double2(16.4, 1001.33333333);
            double2 range = max - min;
            ks_test((() => (range_check(rnd.NextDouble2(min, max).x, min.x, max.x) - min.x) / range.x));
            ks_test((() => (range_check(rnd.NextDouble2(min, max).y, min.y, max.y) - min.y) / range.y));
        }

        [TestCompiler]
        public static void double2_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => rnd.NextDouble2()));
        }

        [TestCompiler]
        public static void double3_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextDouble3().x)));
            ks_test((() => range_check01(rnd.NextDouble3().y)));
            ks_test((() => range_check01(rnd.NextDouble3().z)));
        }

        [TestCompiler]
        public static void double3_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextDouble3().x * 35184372088832.0)));
            ks_test((() => frac(rnd.NextDouble3().y * 35184372088832.0)));
            ks_test((() => frac(rnd.NextDouble3().z * 35184372088832.0)));
        }

        [TestCompiler]
        public static void double3_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double3 max = double3(16.4, 1001.33333333, 3.121);
            ks_test((() => range_check(rnd.NextDouble3(max).x, 0.0, max.x) / max.x));
            ks_test((() => range_check(rnd.NextDouble3(max).y, 0.0, max.y) / max.y));
            ks_test((() => range_check(rnd.NextDouble3(max).z, 0.0, max.z) / max.z));
        }

        [TestCompiler]
        public static void double3_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double3 min = double3(-3.1, 17.1, -0.3);
            double3 max = double3(16.4, 1001.33333333, 3.121);
            double3 range = max - min;
            ks_test((() => (range_check(rnd.NextDouble3(min, max).x, min.x, max.x) - min.x) / range.x));
            ks_test((() => (range_check(rnd.NextDouble3(min, max).y, min.y, max.y) - min.y) / range.y));
            ks_test((() => (range_check(rnd.NextDouble3(min, max).z, min.z, max.z) - min.z) / range.z));
        }


        [TestCompiler]
        public static void double3_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => rnd.NextDouble3().xy));
            r_test((() => rnd.NextDouble3().xz));
            r_test((() => rnd.NextDouble3().yz));
        }

        [TestCompiler]
        public static void double4_uniform()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => range_check01(rnd.NextDouble4().x)));
            ks_test((() => range_check01(rnd.NextDouble4().y)));
            ks_test((() => range_check01(rnd.NextDouble4().z)));
            ks_test((() => range_check01(rnd.NextDouble4().w)));
        }

        [TestCompiler]
        public static void double4_uniform_low_bits()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test((() => frac(rnd.NextDouble4().x * 35184372088832.0)));
            ks_test((() => frac(rnd.NextDouble4().y * 35184372088832.0)));
            ks_test((() => frac(rnd.NextDouble4().z * 35184372088832.0)));
            ks_test((() => frac(rnd.NextDouble4().w * 35184372088832.0)));
        }

        [TestCompiler]
        public static void double4_uniform_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double4 max = double4(16.4f, 1001.33333333f, 3.121f, 1.3232e23f);
            ks_test((() => range_check(rnd.NextDouble4(max).x, 0.0, max.x) / max.x));
            ks_test((() => range_check(rnd.NextDouble4(max).y, 0.0, max.y) / max.y));
            ks_test((() => range_check(rnd.NextDouble4(max).z, 0.0, max.z) / max.z));
            ks_test((() => range_check(rnd.NextDouble4(max).w, 0.0, max.w) / max.w));
        }

        [TestCompiler]
        public static void double4_uniform_min_max()
        {
            var rnd = new Random(0x6E624EB7u);
            double4 min = double4(-3.1, 17.1, -0.3, -22.6);
            double4 max = double4(16.4, 1001.33333333, 3.121, 1.3232e23);
            double4 range = max - min;
            ks_test((() => (range_check(rnd.NextDouble4(min, max).x, min.x, max.x) - min.x) / range.x));
            ks_test((() => (range_check(rnd.NextDouble4(min, max).y, min.y, max.y) - min.y) / range.y));
            ks_test((() => (range_check(rnd.NextDouble4(min, max).z, min.z, max.z) - min.z) / range.z));
            ks_test((() => (range_check(rnd.NextDouble4(min, max).w, min.w, max.w) - min.w) / range.w));
        }

        [TestCompiler]
        public static void double4_independent()
        {
            var rnd = new Random(0x6E624EB7u);
            r_test((() => rnd.NextDouble4().xy));
            r_test((() => rnd.NextDouble4().xz));
            r_test((() => rnd.NextDouble4().xw));
            r_test((() => rnd.NextDouble4().yz));
            r_test((() => rnd.NextDouble4().yw));
            r_test((() => rnd.NextDouble4().zw));
        }

        [TestCompiler]
        public static void float2_direction()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test(() => {
                float2 dir = rnd.NextFloat2Direction();
                TestUtils.AreEqual(1.0f, length(dir), 0.001f);
                return atan2(dir.x, dir.y) / (2.0f * PI) + 0.5f;
            });
        }

        [TestCompiler]
        public static void double2_direction()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test(() => {
                double2 dir = rnd.NextFloat2Direction();
                TestUtils.AreEqual(1.0, length(dir), 0.000001);
                return atan2(dir.y, dir.x) / (2.0 * PI_DBL) + 0.5;
            });
        }


        [TestCompiler]
        public static void float3_direction()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test(() =>
            {
                float3 dir = rnd.NextFloat3Direction();
                float r = length(dir);
                TestUtils.AreEqual(1.0f, r, 0.001f);

                float phi = atan2(dir.y, dir.x) / (2.0f * PI) + 0.5f;
                float z = saturate(dir.z / r * 0.5f + 0.5f);
                return double2(phi, z);
            });
        }

        [TestCompiler]
        public static void double3_direction()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test(() =>
            {
                double3 dir = rnd.NextDouble3Direction();
                double r = length(dir);
                TestUtils.AreEqual(1.0, r, 0.00001);

                double phi = atan2(dir.y, dir.x) / (2.0 * PI_DBL) + 0.5;
                double z = saturate(dir.z / r * 0.5 + 0.5);
                return double2(phi, z);
            });
        }

        [TestCompiler]
        public static void quaternion_rotation()
        {
            var rnd = new Random(0x6E624EB7u);
            ks_test(() =>
            {
                quaternion q = rnd.NextQuaternionRotation();
                TestUtils.AreEqual(1.0, dot(q, q), 0.00001f);
                Assert.GreaterOrEqual(q.value.w, 0.0f);
                float3 p = float3(1.0f, 2.0f, 3.0f);

                float3 qp = mul(q, p);

                TestUtils.AreEqual(length(p), length(qp), 0.0001f);
                float r = length(qp);

                double phi = atan2(qp.y, qp.x) / (2.0 * PI_DBL) + 0.5;
                double z = saturate(qp.z / r * 0.5 + 0.5);
                return double2(phi, z);
            });
        }
    }
}
