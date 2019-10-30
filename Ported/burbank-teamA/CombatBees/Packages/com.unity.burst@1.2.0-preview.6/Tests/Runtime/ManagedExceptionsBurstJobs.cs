using NUnit.Framework;
using System.Text.RegularExpressions;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_2019_3_OR_NEWER
namespace ExceptionsFromBurstJobs
{
    class ManagedExceptionsBurstJobs
    {
        [BurstCompile(CompileSynchronously = true)]
        struct ThrowArgumentExceptionJob : IJob
        {
            public void Execute()
            {
                throw new System.ArgumentException("A");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowArgumentException()
        {
            LogAssert.Expect(LogType.Exception, new Regex("ArgumentException: A"));

            var jobData = new ThrowArgumentExceptionJob();
            jobData.Run();
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowArgumentNullExceptionJob : IJob
        {
            public void Execute()
            {
                throw new System.ArgumentNullException("N");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowArgumentNullException()
        {
            LogAssert.Expect(LogType.Exception, new Regex("System.ArgumentNullException: N"));

            var jobData = new ThrowArgumentNullExceptionJob();
            jobData.Run();
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowNullReferenceExceptionJob : IJob
        {
            public void Execute()
            {
                throw new System.NullReferenceException("N");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowNullReferenceException()
        {
            LogAssert.Expect(LogType.Exception, new Regex("NullReferenceException: N"));

            var jobData = new ThrowNullReferenceExceptionJob();
            jobData.Run();
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowInvalidOperationExceptionJob : IJob
        {
            public void Execute()
            {
                throw new System.InvalidOperationException("IO");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowInvalidOperationException()
        {
            LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: IO"));

            var jobData = new ThrowInvalidOperationExceptionJob();
            jobData.Run();
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowNotSupportedExceptionJob : IJob
        {
            public void Execute()
            {
                throw new System.NotSupportedException("NS");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowNotSupportedException()
        {
            LogAssert.Expect(LogType.Exception, new Regex("NotSupportedException: NS"));

            var jobData = new ThrowNotSupportedExceptionJob();
            jobData.Run();
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowUnityExceptionJob : IJob
        {
            public void Execute()
            {
                throw new UnityException("UE");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowUnityException()
        {
            LogAssert.Expect(LogType.Exception, new Regex("UnityException: UE"));

            var jobData = new ThrowUnityExceptionJob();
            jobData.Run();
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowIndexOutOfRangeExceptionJob : IJob
        {
            public void Execute()
            {
                throw new System.IndexOutOfRangeException("IOOR");
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
        public void ThrowIndexOutOfRange()
        {
            LogAssert.Expect(LogType.Exception, new Regex("IndexOutOfRangeException: IOOR"));

            var jobData = new ThrowIndexOutOfRangeExceptionJob();
            jobData.Run();
        }
    }
}
#endif // #if UNITY_2019_3_OR_NEWER
