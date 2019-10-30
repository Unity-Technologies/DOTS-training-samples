using NUnit.Framework;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        [Test]
        public void JsonSerialization_Serialize_Enums()
        {
            var src = new EnumContainer
            {
                BasicEnum = BasicEnum.A,
                EnumToByte = EnumToByte.B,
                EnumToInt = EnumToInt.C,
                EnumToLong = EnumToLong.A,
                EnumToShort = EnumToShort.B,
                EnumToUint = EnumToUint.C,
                EnumToSByte = EnumToSByte.A,
                EnumToULong = EnumToULong.B,
                EnumToUShort = EnumToUShort.C
            };
            var dst = new EnumContainer();

            var json = JsonSerialization.Serialize(src);
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.BasicEnum, Is.EqualTo(src.BasicEnum));
                Assert.That(dst.EnumToByte, Is.EqualTo(src.EnumToByte));
                Assert.That(dst.EnumToInt, Is.EqualTo(src.EnumToInt));
                Assert.That(dst.EnumToLong, Is.EqualTo(src.EnumToLong));
                Assert.That(dst.EnumToShort, Is.EqualTo(src.EnumToShort));
                Assert.That(dst.EnumToUint, Is.EqualTo(src.EnumToUint));
                Assert.That(dst.EnumToSByte, Is.EqualTo(src.EnumToSByte));
                Assert.That(dst.EnumToULong, Is.EqualTo(src.EnumToULong));
                Assert.That(dst.EnumToUShort, Is.EqualTo(src.EnumToUShort));
            }
        }

        class EnumContainer
        {
            public BasicEnum BasicEnum;
            public EnumToByte EnumToByte;
            public EnumToSByte EnumToSByte;
            public EnumToShort EnumToShort;
            public EnumToUShort EnumToUShort;
            public EnumToLong EnumToLong;
            public EnumToULong EnumToULong;
            public EnumToInt EnumToInt;
            public EnumToUint EnumToUint;
        }

        enum EnumToByte : byte
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToSByte : sbyte
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToShort : short
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToUShort : ushort
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToLong : long
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToULong : ulong
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToInt : int
        {
            A = 1, B = 2, C = 3
        }

        enum EnumToUint : uint
        {
            A = 1, B = 2, C = 3
        }

        enum BasicEnum
        {
            Default, A, B, C
        }
    }
}