using System.Collections;

namespace UnityEditor.TestTools.TestRunner.TestRun.Tasks
{
    internal abstract class TestTaskBase
    {
        public abstract IEnumerator Execute(TestJobData testJobData);
    }
}