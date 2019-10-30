using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Collections;

namespace Unity.Serialization.Tests
{
    [TestFixture]
    public class PackedBinaryStreamTests
    {
        /// <summary>
        /// This is used as a compile time check to make sure we have no managed references within the stream.
        /// </summary>
        [UsedImplicitly]
        struct CanTakeAPointer
        {
            unsafe PackedBinaryStream* m_Stream;
        }
    }
}