using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Unity.Mathematics;

namespace Burst.Compiler.IL.Tests.Helpers
{
    [Flags]
    internal enum DataRange
    {
        // Standard Test (Zero, Minus100To100, Inf, Nan)
        Standard = Zero | Minus100To100 | Inf | NaN,

        // Standard Test (Zero, ZeroExclusiveTo100, Inf, Nan)
        StandardPositive = Zero | ZeroExclusiveTo100 | Inf | NaN,

        // Standard Between -1 and 1 (Zero, MinusOneInclusiveToOneInclusive, Inf, Nan)
        Standard11 = Zero | MinusOneInclusiveToOneInclusive | Inf | NaN,

        Zero = 1 << 1,
        ZeroExclusiveToOneInclusive = 1 << 2,
        MinusOneInclusiveToOneInclusive = 1 << 3,
        Minus100To100 = 1 << 4,
        ZeroExclusiveTo100 = 1 << 5,
        Inf = 1 << 6,
        NaN = 1 << 7,
    }

    internal static class DataRangeExtensions
    {
        private const int VectorsCount = 6;

        public static IEnumerable<object> ExpandRange(this DataRange dataRange, Type type, int seed)
        {
            if (type == typeof(bool))
            {
                yield return true;
                yield return false;
            }
            else if (type == typeof(int))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    yield return (int)(double)value;
                }
            }
            else if (type == typeof(uint))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    var d = (double) value;
                    if (d >= 0.0)
                    {
                        yield return (uint) d;
                    }
                }
            }
            else if (type == typeof(long))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    yield return (long)(double)value;
                }
            }
            else if (type == typeof(ulong))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    var d = (double)value;
                    if (d >= 0.0)
                    {
                        yield return (ulong)d;
                    }
                }
            }
            else if (type == typeof(short))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    yield return (short)(double)value;
                }
            }
            else if (type == typeof(ushort))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf|DataRange.NaN), typeof(double), seed))
                {
                    var d = (double)value;
                    if (d >= 0.0)
                    {
                        yield return (ushort)d;
                    }
                }
            }
            else if (type == typeof(sbyte))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    var d = (double)value;
                    yield return (sbyte) d;
                }
            }
            else if (type == typeof(ushort))
            {
                foreach (var value in ExpandRange(dataRange & ~(DataRange.Inf | DataRange.NaN), typeof(double), seed))
                {
                    var d = (double)value;
                    if (d >= 0.0)
                    {
                        yield return (byte)d;
                    }
                }
            }
            else if (type == typeof(float))
            {
                foreach (var value in ExpandRange(dataRange, typeof(double), seed))
                {
                    var d = (double) value;
                    if (double.IsNaN(d))
                    {
                        yield return float.NaN;
                    }
                    else if (double.IsPositiveInfinity(d))
                    {
                        yield return float.PositiveInfinity;
                    }
                    else if (double.IsNegativeInfinity(d))
                    {
                        yield return float.NegativeInfinity;
                    }
                    else
                    {
                        yield return (float) (double) value;
                    }
                }
            }
            else if (type == typeof(double))
            {
                if ((dataRange & (DataRange.Minus100To100)) != 0)
                {
                    yield return -100.0;
                    yield return -77.9;
                    yield return -50.0;
                    yield return -36.5;
                    yield return -9.1;
                    if ((dataRange & (DataRange.Zero)) != 0)
                    {
                        yield return 0.0;
                    }
                    yield return 5.1;
                    yield return 43.5;
                    yield return 50.0;
                    yield return 76.8;
                    yield return 100.0;

                    if ((dataRange & (DataRange.NaN)) != 0)
                    {
                        yield return double.NaN;
                    }

                    if ((dataRange & (DataRange.Inf)) != 0)
                    {
                        yield return double.PositiveInfinity;
                        yield return double.NegativeInfinity;
                    }
                }
                else if ((dataRange & DataRange.ZeroExclusiveTo100) != 0)
                {
                    foreach (var value in ExpandRange(dataRange | DataRange.Minus100To100, typeof(double), seed))
                    {
                        var d = (double)value;
                        if (double.IsNaN(d) || double.IsInfinity(d))
                        {
                            yield return d;
                        }
                        else if (d != 0.0)
                        {
                            d = d * 0.5 + 50.1;
                            if (d > 100.0)
                            {
                                d = 100.0;
                            }
                            yield return d;
                        }
                    }

                    if ((dataRange & (DataRange.Zero)) != 0)
                    {
                        yield return 0.0;
                    }
                }
                else if ((dataRange & (DataRange.MinusOneInclusiveToOneInclusive)) != 0)
                {
                    foreach (var value in ExpandRange(dataRange | DataRange.Minus100To100, typeof(double), seed))
                    {
                        var d = (double) value;
                        // Return nan/inf as-is
                        if (double.IsNaN(d) || double.IsInfinity(d) || d == 0.0)
                        {
                            yield return d;
                        }
                        else
                        {
                            yield return d / 100.0;
                        }
                    }
                }
                else if ((dataRange & (DataRange.ZeroExclusiveToOneInclusive)) != 0)
                {
                    foreach (var value in ExpandRange(dataRange | DataRange.ZeroExclusiveTo100, typeof(double), seed))
                    {
                        var d = (double) value;
                        if (double.IsNaN(d) || double.IsInfinity(d) || d == 0.0)
                        {
                            yield return d;
                        }
                        else
                        {
                            yield return d / 100.0;
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException($"Invalid datarange `{dataRange}`: missing either Minus100To100 | MinusOneInclusiveToOneInclusive | ZeroExclusiveToOneInclusive`");
                }
            }
            else if (type.Namespace == "Unity.Mathematics")
            {
                if (type.IsByRef)
                {
                    type = type.GetElementType();
                }

                if (type.Name.StartsWith("bool"))
                {
                    var size = (uint)(type.Name["bool".Length] - '0');
                    var bools = ExpandRange(dataRange & ~(DataRange.NaN | DataRange.Inf), typeof(bool), seed).OfType<bool>().ToArray();
                    var indices = Enumerable.Range(0, bools.Length).ToList();
                    var originalIndices = new List<int>(indices);
                    var random = new System.Random(seed);
                    switch (size)
                    {
                        case 2:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = bools[NextIndex(random, indices, originalIndices)];
                                var y = bools[NextIndex(random, indices, originalIndices)];
                                yield return new bool2(x, y);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = bools[NextIndex(random, indices, originalIndices)];
                                var y = bools[NextIndex(random, indices, originalIndices)];
                                var z = bools[NextIndex(random, indices, originalIndices)];
                                yield return new bool3(x, y, z);
                            }
                            break;
                        case 4:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = bools[NextIndex(random, indices, originalIndices)];
                                var y = bools[NextIndex(random, indices, originalIndices)];
                                var z = bools[NextIndex(random, indices, originalIndices)];
                                var w = bools[NextIndex(random, indices, originalIndices)];
                                yield return new bool4(x, y, z, w);
                            }
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported DataRange type `{type}`");
                    }
                }
                else if (type.Name.StartsWith("int"))
                {
                    var size = (uint) (type.Name["int".Length] - '0');
                    var ints = ExpandRange(dataRange & ~(DataRange.NaN | DataRange.Inf), typeof(int), seed).OfType<int>().ToArray();
                    var indices = Enumerable.Range(0, ints.Length).ToList();
                    var originalIndices = new List<int>(indices);
                    var random = new System.Random(seed);
                    switch (size)
                    {
                        case 2:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = ints[NextIndex(random, indices, originalIndices)];
                                var y = ints[NextIndex(random, indices, originalIndices)];
                                yield return new int2(x, y);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = ints[NextIndex(random, indices, originalIndices)];
                                var y = ints[NextIndex(random, indices, originalIndices)];
                                var z = ints[NextIndex(random, indices, originalIndices)];
                                yield return new int3(x, y, z);
                            }
                            break;
                        case 4:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = ints[NextIndex(random, indices, originalIndices)];
                                var y = ints[NextIndex(random, indices, originalIndices)];
                                var z = ints[NextIndex(random, indices, originalIndices)];
                                var w = ints[NextIndex(random, indices, originalIndices)];
                                yield return new int4(x, y, z, w);
                            }
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported DataRange type `{type}`");
                    }
                }
                else if (type.Name.StartsWith("uint"))
                {
                    var size = (uint)(type.Name["uint".Length] - '0');
                    var uints = ExpandRange(dataRange & ~(DataRange.NaN | DataRange.Inf), typeof(uint), seed).OfType<uint>().ToArray();
                    var indices = Enumerable.Range(0, uints.Length).ToList();
                    var originalIndices = new List<int>(indices);
                    var random = new System.Random(seed);
                    switch (size)
                    {
                        case 2:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = uints[NextIndex(random, indices, originalIndices)];
                                var y = uints[NextIndex(random, indices, originalIndices)];
                                yield return new uint2(x, y);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = uints[NextIndex(random, indices, originalIndices)];
                                var y = uints[NextIndex(random, indices, originalIndices)];
                                var z = uints[NextIndex(random, indices, originalIndices)];
                                yield return new uint3(x, y, z);
                            }
                            break;
                        case 4:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = uints[NextIndex(random, indices, originalIndices)];
                                var y = uints[NextIndex(random, indices, originalIndices)];
                                var z = uints[NextIndex(random, indices, originalIndices)];
                                var w = uints[NextIndex(random, indices, originalIndices)];
                                yield return new uint4(x, y, z, w);
                            }
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported DataRange type `{type}`");
                    }
                }
                else if (type.Name.StartsWith("float"))
                {
                    var size = (uint) (type.Name["float".Length] - '0');
                    var floats = ExpandRange(dataRange & ~(DataRange.NaN | DataRange.Inf), typeof(float), seed).OfType<float>().ToList();
                    var originalIndices = Enumerable.Range(0, floats.Count).ToList();
                    var indices = new List<int>(originalIndices);
                    // We only put NaN and Inf in the first set of values
                    if ((dataRange & DataRange.NaN) != 0)
                    {
                        indices.Add(floats.Count);
                        floats.Add(float.NaN);
                    }
                    if ((dataRange & DataRange.Inf) != 0)
                    {
                        indices.Add(floats.Count);
                        floats.Add(float.PositiveInfinity);
                    }

                    var random = new System.Random(seed);
                    switch (size)
                    {
                        case 2:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = floats[NextIndex(random, indices, originalIndices)];
                                var y = floats[NextIndex(random, indices, originalIndices)];
                                yield return new float2(x, y);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = floats[NextIndex(random, indices, originalIndices)];
                                var y = floats[NextIndex(random, indices, originalIndices)];
                                var z = floats[NextIndex(random, indices, originalIndices)];
                                yield return new float3(x, y, z);
                            }
                            break;
                        case 4:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = floats[NextIndex(random, indices, originalIndices)];
                                var y = floats[NextIndex(random, indices, originalIndices)];
                                var z = floats[NextIndex(random, indices, originalIndices)];
                                var w = floats[NextIndex(random, indices, originalIndices)];
                                yield return new float4(x, y, z, w);
                            }
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported DataRange type `{type}`");
                    }
                }
                else if (type.Name.StartsWith("double"))
                {
                    var size = (uint)(type.Name["double".Length] - '0');
                    var doubles = ExpandRange(dataRange & ~(DataRange.NaN | DataRange.Inf), typeof(double), seed).OfType<double>().ToList();
                    var originalIndices = Enumerable.Range(0, doubles.Count).ToList();
                    var indices = new List<int>(originalIndices);
                    // We only put NaN and Inf in the first set of values
                    if ((dataRange & DataRange.NaN) != 0)
                    {
                        indices.Add(doubles.Count);
                        doubles.Add(double.NaN);
                    }
                    if ((dataRange & DataRange.Inf) != 0)
                    {
                        indices.Add(doubles.Count);
                        doubles.Add(double.PositiveInfinity);
                    }

                    var random = new System.Random(seed);
                    switch (size)
                    {
                        case 2:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = doubles[NextIndex(random, indices, originalIndices)];
                                var y = doubles[NextIndex(random, indices, originalIndices)];
                                yield return new double2(x, y);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = doubles[NextIndex(random, indices, originalIndices)];
                                var y = doubles[NextIndex(random, indices, originalIndices)];
                                var z = doubles[NextIndex(random, indices, originalIndices)];
                                yield return new double3(x, y, z);
                            }
                            break;
                        case 4:
                            for (int i = 0; i < VectorsCount; i++)
                            {
                                var x = doubles[NextIndex(random, indices, originalIndices)];
                                var y = doubles[NextIndex(random, indices, originalIndices)];
                                var z = doubles[NextIndex(random, indices, originalIndices)];
                                var w = doubles[NextIndex(random, indices, originalIndices)];
                                yield return new double4(x, y, z, w);
                            }
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported DataRange type `{type}`");
                    }
                }
                else
                {
                    throw new NotSupportedException($"Unsupported DataRange type `{type}`");
                }
            }
            else
            {
                throw new NotSupportedException($"Unsupported DataRange type `{type}`");
            }
        }

        private static int NextIndex(System.Random random, List<int> indices, List<int> originalIndices)
        {
            var id = random.Next(0, indices.Count - 1);
            var index = indices[id];
            indices.RemoveAt(id);
            if (indices.Count == 0)
            {
                indices.AddRange(originalIndices);
            }
            return index;
        }
    }
}