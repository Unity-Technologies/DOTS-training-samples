using NUnit.Framework;
using static Unity.Mathematics.math;
using Burst.Compiler.IL.Tests;

namespace Unity.Mathematics.Tests
{
    [TestFixture]
    public class TestBitmanipulation
    {
        [TestCompiler]
        public static void bitmask_bool4()
        {
            TestUtils.AreEqual(0b0000, bitmask(new bool4(false, false, false, false)));
            TestUtils.AreEqual(0b0001, bitmask(new bool4(true, false, false, false)));
            TestUtils.AreEqual(0b0010, bitmask(new bool4(false, true, false, false)));
            TestUtils.AreEqual(0b0100, bitmask(new bool4(false, false, true, false)));
            TestUtils.AreEqual(0b1000, bitmask(new bool4(false, false, false, true)));

            TestUtils.AreEqual(0b1111, bitmask(new bool4(true, true, true, true)));
            TestUtils.AreEqual(0b1110, bitmask(new bool4(false, true, true, true)));
            TestUtils.AreEqual(0b1101, bitmask(new bool4(true, false, true, true)));
            TestUtils.AreEqual(0b1011, bitmask(new bool4(true, true, false, true)));
            TestUtils.AreEqual(0b0111, bitmask(new bool4(true, true, true, false)));
        }

        [TestCompiler]
        public static void countbits_int1()
        {
            TestUtils.AreEqual(countbits( 0x01234567), 12);
            TestUtils.AreEqual(countbits( 0x456789AB), 16);
            TestUtils.AreEqual(countbits(-0x01234567), 21);
            TestUtils.AreEqual(countbits(-0x456789AB), 17);
            TestUtils.AreEqual(countbits(-1), 32);
        }

        [TestCompiler]
        public static void countbits_int2()
        {
            TestUtils.AreEqual(countbits(int2(0, 0x01234567)), int2(0, 12));
            TestUtils.AreEqual(countbits(int2(0x456789AB, -0x01234567)), int2(16, 21));
            TestUtils.AreEqual(countbits(int2(-0x456789AB, -1)), int2(17, 32));
        }

        [TestCompiler]
        public static void countbits_int3()
        {
            TestUtils.AreEqual(countbits(int3(0, 0x01234567, 0x456789AB)), int3(0, 12, 16));
            TestUtils.AreEqual(countbits(int3(-0x01234567, -0x456789AB, -1)), int3(21, 17, 32));
        }

        [TestCompiler]
        public static void countbits_int4()
        {
            TestUtils.AreEqual(countbits(int4(0, 0x01234567, 0x456789AB, -0x01234567)), int4(0, 12, 16, 21));
            TestUtils.AreEqual(countbits(int4(-0x456789AB, -1, -7, -15)), int4(17, 32, 30, 29));
        }

        [TestCompiler]
        public static void countbits_uint()
        {
            TestUtils.AreEqual(countbits(0u), 0);
            TestUtils.AreEqual(countbits(0x01234567u), 12);
            TestUtils.AreEqual(countbits(0x456789ABu), 16);
            TestUtils.AreEqual(countbits(0x89ABCDEFu), 20);
            TestUtils.AreEqual(countbits(0xCDEF0123u), 16);
            TestUtils.AreEqual(countbits(0xFFFFFFFFu), 32);
        }

        [TestCompiler]
        public static void countbits_uint2()
        {
            TestUtils.AreEqual(countbits(uint2(0, 0x01234567)), int2(0, 12));
            TestUtils.AreEqual(countbits(uint2(0x456789AB, 0x89ABCDEFu)), int2(16, 20));
            TestUtils.AreEqual(countbits(uint2(0xCDEF0123u, 0xFFFFFFFFu)), int2(16, 32));
        }

        [TestCompiler]
        public static void countbits_uint3()
        {
            TestUtils.AreEqual(countbits(uint3(0, 0x01234567, 0x456789AB)), int3(0, 12, 16));
            TestUtils.AreEqual(countbits(uint3(0x89ABCDEFu, 0xCDEF0123u, 0xFFFFFFFFu)), int3(20, 16, 32));
        }

        [TestCompiler]
        public static void countbits_uint4()
        {
            TestUtils.AreEqual(countbits(uint4(0, 0x01234567, 0x456789AB, 0x89ABCDEFu)), int4(0, 12, 16, 20));
            TestUtils.AreEqual(countbits(uint4(0xCDEF0123u, 0xFFFFFFFFu, 0xFFFFFFF5u, 0xFFFFFFF1u)), int4(16, 32, 30, 29));
        }

        [TestCompiler]
        public static void countbits_long()
        {
            TestUtils.AreEqual(countbits(0L), 0);
            TestUtils.AreEqual(countbits(0x0123456789ABCDEFL), 32);
            TestUtils.AreEqual(countbits(-0x0123456789ABCDEFL), 33);
            TestUtils.AreEqual(countbits(-1L), 64);
        }

        [TestCompiler]
        public static void countbits_ulong()
        {
            TestUtils.AreEqual(countbits(0UL), 0);
            TestUtils.AreEqual(countbits(0x0123456789ABCDEFUL), 32);
            TestUtils.AreEqual(countbits(0x89ABCDEF01234567UL), 32);
            TestUtils.AreEqual(countbits(0xFFFFFFFFFFFFFFFFUL), 64);
        }

        [TestCompiler]
        public static void lzcnt_int()
        {
            TestUtils.AreEqual(lzcnt(0), 32);
            TestUtils.AreEqual(lzcnt(1), 31);
            TestUtils.AreEqual(lzcnt(2), 30);
            TestUtils.AreEqual(lzcnt(3), 30);
            TestUtils.AreEqual(lzcnt(0x5321), 17);
            TestUtils.AreEqual(lzcnt(0x04435321), 5);
            TestUtils.AreEqual(lzcnt(-1), 0);
            TestUtils.AreEqual(lzcnt(-2147483648), 0);
        }

        [TestCompiler]
        public static void lzcnt_int2()
        {
            TestUtils.AreEqual(lzcnt(int2(0, 1)), int2(32, 31));
            TestUtils.AreEqual(lzcnt(int2(2, 3)), int2(30, 30));
            TestUtils.AreEqual(lzcnt(int2(0x5321, 0x04435321)), int2(17, 5));
            TestUtils.AreEqual(lzcnt(int2(-1, -2147483648)), int2(0, 0));
        }

        [TestCompiler]
        public static void lzcnt_int3()
        {
            TestUtils.AreEqual(lzcnt(int3(0, 1, 2)), int3(32, 31, 30));
            TestUtils.AreEqual(lzcnt(int3(3, 0x5321, 0x04435321)), int3(30, 17, 5));
            TestUtils.AreEqual(lzcnt(int3(-1, -2147483648, 17)), int3(0, 0, 27));
        }

        [TestCompiler]
        public static void lzcnt_int4()
        {
            TestUtils.AreEqual(lzcnt(int4(0, 1, 2, 3)), int4(32, 31, 30, 30));
            TestUtils.AreEqual(lzcnt(int4(0x5321, 0x04435321, -1, -2147483648)), int4(17, 5, 0, 0));
        }

        [TestCompiler]
        public static void lzcnt_uint()
        {
            TestUtils.AreEqual(lzcnt(0u), 32);
            TestUtils.AreEqual(lzcnt(1u), 31);
            TestUtils.AreEqual(lzcnt(2u), 30);
            TestUtils.AreEqual(lzcnt(3u), 30);
            TestUtils.AreEqual(lzcnt(0x5321u), 17);
            TestUtils.AreEqual(lzcnt(0x04435321u), 5);
            TestUtils.AreEqual(lzcnt(0x84435320u), 0);
            TestUtils.AreEqual(lzcnt(0xFFFFFFFFu), 0);
        }

        [TestCompiler]
        public static void lzcnt_uint2()
        {
            TestUtils.AreEqual(lzcnt(uint2(0u, 1u)), int2(32, 31));
            TestUtils.AreEqual(lzcnt(uint2(2u, 3u)), int2(30, 30));
            TestUtils.AreEqual(lzcnt(uint2(0x5321u, 0x04435321u)), int2(17, 5));
            TestUtils.AreEqual(lzcnt(uint2(0x84435320u, 0xFFFFFFFFu)), int2(0, 0));
        }

        [TestCompiler]
        public static void lzcnt_uint3()
        {
            TestUtils.AreEqual(lzcnt(uint3(0u, 1u, 2u)), int3(32, 31, 30));
            TestUtils.AreEqual(lzcnt(uint3(3u, 0x5321u, 0x04435321u)), int3(30, 17, 5));
            TestUtils.AreEqual(lzcnt(uint3(0x84435320u, 0xFFFFFFFFu, 17)), int3(0, 0, 27));
        }

        [TestCompiler]
        public static void lzcnt_uint4()
        {
            TestUtils.AreEqual(lzcnt(uint2(0u, 1u)), int2(32, 31));
            TestUtils.AreEqual(lzcnt(uint2(2u, 3u)), int2(30, 30));
            TestUtils.AreEqual(lzcnt(uint2(0x5321u, 0x04435321u)), int2(17, 5));
            TestUtils.AreEqual(lzcnt(uint2(0x84435320u, 0xFFFFFFFFu)), int2(0, 0));
        }

        [TestCompiler]
        public static void lzcnt_long()
        {
            TestUtils.AreEqual(lzcnt(0L), 64);
            TestUtils.AreEqual(lzcnt(1L), 63);
            TestUtils.AreEqual(lzcnt(0x1FFF1234L), 35);
            TestUtils.AreEqual(lzcnt(0x1FFFF1234L), 31);
            TestUtils.AreEqual(lzcnt(0x1FFFFFFF1234L), 19);
            TestUtils.AreEqual(lzcnt(-1L), 0);
            TestUtils.AreEqual(lzcnt(-9223372036854775808L), 0);
        }

        [TestCompiler]
        public static void lzcnt_ulong()
        {
            TestUtils.AreEqual(lzcnt(0UL), 64);
            TestUtils.AreEqual(lzcnt(1UL), 63);
            TestUtils.AreEqual(lzcnt(0x1FFF1234UL), 35);
            TestUtils.AreEqual(lzcnt(0x1FFFF1234UL), 31);
            TestUtils.AreEqual(lzcnt(0x1FFFFFFF1234UL), 19);
            TestUtils.AreEqual(lzcnt(0xFFFFFFFFFFFFFFFFUL), 0);
            TestUtils.AreEqual(lzcnt(0x8000000000000000UL), 0);
        }

        [TestCompiler]
        public static void tzcnt_int()
        {
            TestUtils.AreEqual(tzcnt(0), 32);
            TestUtils.AreEqual(tzcnt(1), 0);
            TestUtils.AreEqual(tzcnt(2), 1);
            TestUtils.AreEqual(tzcnt(3), 0);
            TestUtils.AreEqual(tzcnt(0x53210), 4);
            TestUtils.AreEqual(tzcnt(0x44420000), 17);
            TestUtils.AreEqual(tzcnt(-2), 1);
            TestUtils.AreEqual(tzcnt(-2147483647), 0);
            TestUtils.AreEqual(tzcnt(-2147483648), 31);
        }

        [TestCompiler]
        public static void tzcnt_int2()
        {
            TestUtils.AreEqual(tzcnt(int2(0, 1)), int2(32, 0));
            TestUtils.AreEqual(tzcnt(int2(2, 3)), int2(1, 0));
            TestUtils.AreEqual(tzcnt(int2(0x53210, 0x44420000)), int2(4, 17));
            TestUtils.AreEqual(tzcnt(int2(-2, -2147483647)), int2(1, 0));
            TestUtils.AreEqual(tzcnt(int2(-2147483648, 20)), int2(31, 2));
        }

        [TestCompiler]
        public static void tzcnt_int3()
        {
            TestUtils.AreEqual(tzcnt(int3(0, 1, 2)), int3(32, 0, 1));
            TestUtils.AreEqual(tzcnt(int3(3, 0x53210, 0x44420000)), int3(0, 4, 17));
            TestUtils.AreEqual(tzcnt(int3(-2, -2147483647, -2147483648)), int3(1, 0, 31));
        }

        [TestCompiler]
        public static void tzcnt_int4()
        {
            TestUtils.AreEqual(tzcnt(int4(0, 1, 2, 3)), int4(32, 0, 1, 0));
            TestUtils.AreEqual(tzcnt(int4(0x53210, 0x44420000, -2, -2147483647)), int4(4, 17, 1, 0));
            TestUtils.AreEqual(tzcnt(int4(-2147483648, 20, 4132, -8)), int4(31, 2, 2, 3));
        }

        [TestCompiler]
        public static void tzcnt_uint()
        {
            TestUtils.AreEqual(tzcnt(0u), 32);
            TestUtils.AreEqual(tzcnt(1u), 0);
            TestUtils.AreEqual(tzcnt(2u), 1);
            TestUtils.AreEqual(tzcnt(3u), 0);
            TestUtils.AreEqual(tzcnt(0x53210u), 4);
            TestUtils.AreEqual(tzcnt(0x44420000u), 17);
            TestUtils.AreEqual(tzcnt(0xFFFFFFFEu), 1);
            TestUtils.AreEqual(tzcnt(0x80000001u), 0);
            TestUtils.AreEqual(tzcnt(0x80000000u), 31);
        }

        [TestCompiler]
        public static void tzcnt_uint2()
        {
            TestUtils.AreEqual(tzcnt(uint2(0u, 1u)), int2(32, 0));
            TestUtils.AreEqual(tzcnt(uint2(2u, 3u)), int2(1, 0));
            TestUtils.AreEqual(tzcnt(uint2(0x53210u, 0x44420000u)), int2(4, 17));
            TestUtils.AreEqual(tzcnt(uint2(0xFFFFFFFEu, 0x80000001u)), int2(1, 0));
            TestUtils.AreEqual(tzcnt(uint2(0x80000000u, 20u)), int2(31, 2));
        }

        [TestCompiler]
        public static void tzcnt_uint3()
        {
            TestUtils.AreEqual(tzcnt(uint3(0u, 1u, 2u)), int3(32, 0, 1));
            TestUtils.AreEqual(tzcnt(uint3(3u, 0x53210u, 0x44420000u)), int3(0, 4, 17));
            TestUtils.AreEqual(tzcnt(uint3(0xFFFFFFFEu, 0x80000001u, 0x80000000u)), int3(1, 0, 31));
        }

        [TestCompiler]
        public static void tzcnt_uint4()
        {
            TestUtils.AreEqual(tzcnt(uint4(0u, 1u, 2u, 3u)), int4(32, 0, 1, 0));
            TestUtils.AreEqual(tzcnt(uint4(0x53210u, 0x44420000u, 0xFFFFFFFE, 0x80000001u)), int4(4, 17, 1, 0));
            TestUtils.AreEqual(tzcnt(uint4(0x80000000u, 20u, 4132u, 0xFFFFFFF8u)), int4(31, 2, 2, 3));
        }

        [TestCompiler]
        public static void tzcnt_long()
        {
            TestUtils.AreEqual(tzcnt(0L), 64);
            TestUtils.AreEqual(tzcnt(1L), 0);
            TestUtils.AreEqual(tzcnt(2L), 1);
            TestUtils.AreEqual(tzcnt(0x44420000L), 17);
            TestUtils.AreEqual(tzcnt(0x444200000000L), 33);
            TestUtils.AreEqual(tzcnt(-9223372036854775808L), 63);
            TestUtils.AreEqual(tzcnt(-9223372036854775807L), 0);
        }

        [TestCompiler]
        public static void tzcnt_ulong()
        {
            TestUtils.AreEqual(tzcnt(0UL), 64);
            TestUtils.AreEqual(tzcnt(1UL), 0);
            TestUtils.AreEqual(tzcnt(2UL), 1);
            TestUtils.AreEqual(tzcnt(0x44420000UL), 17);
            TestUtils.AreEqual(tzcnt(0x444200000000UL), 33);
            TestUtils.AreEqual(tzcnt(0x8000000000000000UL), 63);
            TestUtils.AreEqual(tzcnt(0x8000000000000001UL), 0);
        }

        [TestCompiler]
        public static void reversebits_int()
        {
            TestUtils.AreEqual(reversebits(-1872213312), 0x03521609);
            TestUtils.AreEqual(reversebits(0x1260dafa), 0x5f5b0648);
            TestUtils.AreEqual(reversebits(-1312858670), 0x4bbafd8d);
            TestUtils.AreEqual(reversebits(0x74239b12), 0x48d9c42e);
        }

        [TestCompiler]
        public static void reversebits_int2()
        {
            TestUtils.AreEqual(reversebits(int2(-1872213312, 0x1260dafa)), int2(0x03521609, 0x5f5b0648));
            TestUtils.AreEqual(reversebits(int2(-1312858670, 0x74239b12)), int2(0x4bbafd8d, 0x48d9c42e));
        }

        [TestCompiler]
        public static void reversebits_int3()
        {
            TestUtils.AreEqual(reversebits(int3(-1872213312, 0x1260dafa, -1312858670)), int3(0x03521609, 0x5f5b0648, 0x4bbafd8d));
            TestUtils.AreEqual(reversebits(int3(0x74239b12, 0, -1)), int3(0x48d9c42e, 0, -1));
        }

        [TestCompiler]
        public static void reversebits_int4()
        {
            TestUtils.AreEqual(reversebits(int4(-1872213312, 0x1260dafa, -1312858670, 0x74239b12)), int4(0x03521609, 0x5f5b0648, 0x4bbafd8d, 0x48d9c42e));
        }

        [TestCompiler]
        public static void reversebits_uint()
        {
            TestUtils.AreEqual(reversebits(0x90684ac0u), 0x03521609u);
            TestUtils.AreEqual(reversebits(0x1260dafau), 0x5f5b0648u);
            TestUtils.AreEqual(reversebits(0xb1bf5dd2u), 0x4bbafd8du);
            TestUtils.AreEqual(reversebits(0x74239b12u), 0x48d9c42eu);
        }

        [TestCompiler]
        public static void reversebits_uint2()
        {
            TestUtils.AreEqual(reversebits(uint2(0x90684ac0u, 0x1260dafau)), uint2(0x03521609u, 0x5f5b0648u));
            TestUtils.AreEqual(reversebits(uint2(0xb1bf5dd2u, 0x74239b12u)), uint2(0x4bbafd8du, 0x48d9c42eu));
        }

        [TestCompiler]
        public static void reversebits_uint3()
        {
            TestUtils.AreEqual(reversebits(uint3(0x90684ac0u, 0x1260dafau, 0xb1bf5dd2u)), uint3(0x03521609u, 0x5f5b0648u, 0x4bbafd8du));
            TestUtils.AreEqual(reversebits(uint3(0x74239b12u, 0u, 0xFFFFFFFF)), uint3(0x48d9c42eu, 0u, 0xFFFFFFFF));
        }

        [TestCompiler]
        public static void reversebits_uint4()
        {
            TestUtils.AreEqual(reversebits(uint4(0x90684ac0u, 0x1260dafau, 0xb1bf5dd2u, 0x74239b12u)), uint4(0x03521609u, 0x5f5b0648u, 0x4bbafd8du, 0x48d9c42eu));
        }

        [TestCompiler]
        public static void reversebits_long()
        {
            TestUtils.AreEqual(reversebits(0x1260dafab1bf5dd2L), 0x4bbafd8d5f5b0648L);
        }

        [TestCompiler]
        public static void reversebits_ulong()
        {
            TestUtils.AreEqual(reversebits(0x1260dafab1bf5dd2ul), 0x4bbafd8d5f5b0648ul);
        }

        [TestCompiler]
        public static void rol_int()
        {
            TestUtils.AreEqual(rol(219257022, 11), -1933184920);
            TestUtils.AreEqual(rol(-1586446996, 11), -2048170741);
            TestUtils.AreEqual(rol(-279484078, 11), -1152739462);
            TestUtils.AreEqual(rol(-1692078607, 11), 661621977);
        }

        [TestCompiler]
        public static void rol_int2()
        {
            TestUtils.AreEqual(rol(int2(219257022, -1586446996), 11), int2(-1933184920, -2048170741));
            TestUtils.AreEqual(rol(int2(-279484078, -1692078607), 11), int2(-1152739462, 661621977));
        }

        [TestCompiler]
        public static void rol_int3()
        {
            TestUtils.AreEqual(rol(int3(219257022, -1586446996, -279484078), 11), int3(-1933184920, -2048170741, -1152739462));
        }


        [TestCompiler]
        public static void rol_int4()
        {
            TestUtils.AreEqual(rol(int4(219257022, -1586446996, -279484078, -1692078607), 11), int4(-1933184920, -2048170741, -1152739462, 661621977));
        }

        [TestCompiler]
        public static void rol_uint()
        {
            TestUtils.AreEqual(rol(219257022u, 11), 2361782376u);
            TestUtils.AreEqual(rol(2708520300u, 11), 2246796555u);
            TestUtils.AreEqual(rol(4015483218u, 11), 3142227834u);
            TestUtils.AreEqual(rol(2602888689u, 11), 661621977u);
        }

        [TestCompiler]
        public static void rol_uint2()
        {
            TestUtils.AreEqual(rol(uint2(219257022u, 2708520300u), 11), uint2(2361782376u, 2246796555u));
            TestUtils.AreEqual(rol(uint2(4015483218u, 2602888689u), 11), uint2(3142227834u, 661621977u));
        }

        [TestCompiler]
        public static void rol_uint3()
        {
            TestUtils.AreEqual(rol(uint3(219257022u, 2708520300u, 4015483218u), 11), uint3(2361782376u, 2246796555u, 3142227834u));
        }

        [TestCompiler]
        public static void rol_uint4()
        {
            TestUtils.AreEqual(rol(uint4(219257022u, 2708520300u, 4015483218u, 2602888689u), 11), uint4(2361782376u, 2246796555u, 3142227834u, 661621977u));
        }


        [TestCompiler]
        public static void rol_long()
        {
            TestUtils.AreEqual(rol(6894885722123239465L, 37), 4769317691753349395L);
            TestUtils.AreEqual(rol(9017875690541231318L, 37), 7702732954299909421L);
            TestUtils.AreEqual(rol(-6252342588442027279L, 37), 4304137451269976409L);
            TestUtils.AreEqual(rol(2788577329702376155L, 37), -5493728106787075631L);
        }

        [TestCompiler]
        public static void rol_ulong()
        {
            TestUtils.AreEqual(rol(6894885722123239465UL, 37), 4769317691753349395UL);
            TestUtils.AreEqual(rol(9017875690541231318UL, 37), 7702732954299909421UL);
            TestUtils.AreEqual(rol(12194401485267524337UL, 37), 4304137451269976409UL);
            TestUtils.AreEqual(rol(2788577329702376155UL, 37), 12953015966922475985UL);
        }


        [TestCompiler]
        public static void ror_int()
        {
            TestUtils.AreEqual(ror(-1710129111, 11), 87245360);
            TestUtils.AreEqual(ror(1232136068, 11), -259445220);
            TestUtils.AreEqual(ror(1800875222, 11), -1697813787);
            TestUtils.AreEqual(ror(-98246768, 11), -232831845);
        }

        [TestCompiler]
        public static void ror_int2()
        {
            TestUtils.AreEqual(ror(int2(-1710129111, 1232136068), 11), int2(87245360, -259445220));
            TestUtils.AreEqual(ror(int2(1800875222, -98246768), 11), int2(-1697813787, -232831845));
        }

        [TestCompiler]
        public static void ror_int3()
        {
            TestUtils.AreEqual(ror(int3(-1710129111, 1232136068, 1800875222), 11), int3(87245360, -259445220, -1697813787));
        }

        [TestCompiler]
        public static void ror_int4()
        {
            TestUtils.AreEqual(ror(int4(-1710129111, 1232136068, 1800875222, -98246768), 11), int4(87245360, -259445220, -1697813787, -232831845));
        }


        [TestCompiler]
        public static void ror_uint()
        {
            TestUtils.AreEqual(ror(2584838185u, 11), 87245360u);
            TestUtils.AreEqual(ror(1232136068u, 11), 4035522076u);
            TestUtils.AreEqual(ror(1800875222u, 11), 2597153509u);
            TestUtils.AreEqual(ror(4196720528u, 11), 4062135451u);
        }

        [TestCompiler]
        public static void ror_uint2()
        {
            TestUtils.AreEqual(ror(uint2(2584838185u, 1232136068u), 11), uint2(87245360u, 4035522076u));
            TestUtils.AreEqual(ror(uint2(1800875222u, 4196720528u), 11), uint2(2597153509u, 4062135451u));
        }

        [TestCompiler]
        public static void ror_uint3()
        {
            TestUtils.AreEqual(ror(uint3(2584838185u, 1232136068u, 1800875222u), 11), uint3(87245360u, 4035522076u, 2597153509u));
        }

        [TestCompiler]
        public static void ror_uint4()
        {
            TestUtils.AreEqual(ror(uint4(2584838185u, 1232136068u, 1800875222u, 4196720528u), 11), uint4(87245360u, 4035522076u, 2597153509u, 4062135451u));
        }

        [TestCompiler]
        public static void ror_long()
        {
            TestUtils.AreEqual(ror(6894885722123239465L, 37), 4958617126915898480L);
            TestUtils.AreEqual(ror(9017875690541231318L, 37), 5429856151504760689L);
            TestUtils.AreEqual(ror(-6252342588442027279L, 37), 6219170745001040316L);
            TestUtils.AreEqual(ror(2788577329702376155L, 37), 8389344736564320290L);
        }

        [TestCompiler]
        public static void ror_ulong()
        {
            TestUtils.AreEqual(ror(6894885722123239465UL, 37), 4958617126915898480UL);
            TestUtils.AreEqual(ror(9017875690541231318UL, 37), 5429856151504760689UL);
            TestUtils.AreEqual(ror(12194401485267524337UL, 37), 6219170745001040316UL);
            TestUtils.AreEqual(ror(2788577329702376155UL, 37), 8389344736564320290UL);
        }


    }


    class Assert2
    {
        public static void AreEqual(object a, object b)
        {

        }
    }

}
