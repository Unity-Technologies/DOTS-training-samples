using System;
using System.Runtime.CompilerServices;

namespace Unity.Mathematics
{
    public struct half : System.IEquatable<half>, IFormattable
    {
        public ushort value;

        /// <summary>half zero value.</summary>
        public static readonly half zero = new half();

        public static float MaxValue { get { return 65504.0f; } }
        public static float MinValue { get { return -65504.0f; } }

        /// <summary>Constructs a half value from a half value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public half(half x)
        {
            value = x.value;
        }

        /// <summary>Constructs a half value from a float value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public half(float v)
        {
            value = (ushort)math.f32tof16(v);
        }

        /// <summary>Constructs a half value from a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public half(double v)
        {
            value = (ushort)math.f32tof16((float)v);
        }

        /// <summary>Explicitly converts a float value to a half value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator half(float v) { return new half(v); }

        /// <summary>Explicitly converts a double value to a half value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator half(double v) { return new half(v); }

        /// <summary>Implicitly converts a half value to a float value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(half d) { return math.f16tof32(d.value); }

        /// <summary>Implicitly converts a half value to a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(half d) { return math.f16tof32(d.value); }


        /// <summary>Returns whether two half values are equal.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(half lhs, half rhs) { return lhs.value == rhs.value; }

        /// <summary>Returns whether two half values are different.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(half lhs, half rhs) { return lhs.value != rhs.value; }


        /// <summary>Returns true if the half is equal to a given half, false otherwise.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(half rhs) { return value == rhs.value; }

        /// <summary>Returns true if the half is equal to a given half, false otherwise.</summary>
        public override bool Equals(object o) { return Equals((half)o); }

        /// <summary>Returns a hash code for the half.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() { return (int)value; }

        /// <summary>Returns a string representation of the half.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return math.f16tof32(value).ToString();
        }

        /// <summary>Returns a string representation of the half using a specified format and culture-specific format information.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return math.f16tof32(value).ToString(format, formatProvider);
        }
    }

    public static partial class math
    {
        /// <summary>Returns a half value constructed from a half values.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half half(half x) { return new half(x); }

        /// <summary>Returns a half value constructed from a float value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half half(float v) { return new half(v); }

        /// <summary>Returns a half value constructed from a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static half half(double v) { return new half(v); }

        /// <summary>Returns a uint hash code of a half value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(half v)
        {
            return v.value * 0x745ED837u + 0x816EFB5Du;
        }
    }
}
