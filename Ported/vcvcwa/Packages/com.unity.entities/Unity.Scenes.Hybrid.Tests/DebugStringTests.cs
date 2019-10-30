using NUnit.Framework;
using Unity.Collections;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Unity.Scenes.Hybrid.Tests
{
    public class DebugStringTests
    {
        [Test]
        public void DebugStrings_DontAllocateMemory_WhenConditionallyDisabled()
        {
            var intArray = new[] {1, 2, 3, 4, 5};
            var nativeIntArray = new NativeArray<int>(intArray, Allocator.Temp);

            try
            {
                Assert.That(() =>
                {
                    LiveLinkMsg.LogReceived("Debug int string: " + nativeIntArray.ToDebugString(i => "int " + i));
                    LiveLinkMsg.LogInfo("Debug int string: " + nativeIntArray.ToDebugString(i => "int " + i));
                    LiveLinkMsg.LogSend("Debug int string: " + nativeIntArray.ToDebugString(i => "int " + i));
                }, Is.Not.AllocatingGCMemory(), "Unexpected memory allocation on conditional code.");
            }
            finally
            {
                nativeIntArray.Dispose();
            }
        }
    }
}