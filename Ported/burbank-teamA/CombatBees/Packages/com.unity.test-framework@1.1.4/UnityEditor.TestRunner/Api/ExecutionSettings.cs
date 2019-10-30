using System;
using System.Linq;

namespace UnityEditor.TestTools.TestRunner.Api
{
    public class ExecutionSettings
    {
        public ExecutionSettings(params Filter[] filtersToExecute)
        {
            filters = filtersToExecute;
        }
        
        internal BuildTarget? targetPlatform;
        public ITestRunSettings overloadTestRunSettings;
        internal Filter filter;
        public Filter[] filters;
        public bool runSynchronously;
        public int playerHeartbeatTimeout = 60*10;

        internal bool EditModeIncluded()
        {
            return filters.Any(f => IncludesTestMode(f.testMode, TestMode.EditMode));
        }
        
        internal bool PlayModeInEditorIncluded()
        {
            return filters.Any(f => IncludesTestMode(f.testMode, TestMode.PlayMode) && targetPlatform == null);
        }

        internal bool PlayerIncluded()
        {
            return filters.Any(f => IncludesTestMode(f.testMode, TestMode.PlayMode) && targetPlatform != null);
        }

        private static bool IncludesTestMode(TestMode testMode, TestMode modeToCheckFor)
        {
            return (testMode & modeToCheckFor) == modeToCheckFor;
        }
    }
}
