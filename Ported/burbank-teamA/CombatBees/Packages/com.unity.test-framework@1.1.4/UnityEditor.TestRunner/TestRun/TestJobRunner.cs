using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.TestRun.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner.TestRun
{
    internal class TestJobRunner
    {
        private static IEnumerable<TestTaskBase> GetTaskList(ExecutionSettings settings)
        {
            if (settings == null)
            {
                yield break;
            }

            if (settings.EditModeIncluded() || (PlayerSettings.runPlayModeTestAsEditModeTest && settings.PlayModeInEditorIncluded()))
            {
                yield return new SaveModiedSceneTask();
                yield return new RegisterFilesForCleanupVerificationTask();
                yield return new LegacyEditModeRunTask();
                yield return new CleanupVerificationTask();
                yield break;
            }

            if (settings.PlayModeInEditorIncluded() && !PlayerSettings.runPlayModeTestAsEditModeTest)
            {
                yield return new SaveModiedSceneTask();
                yield return new LegacyPlayModeRunTask();
                yield break;
            }

            if (settings.PlayerIncluded())
            {
                yield return new LegacyPlayerRunTask();
                yield break;
            }
        }

        [InitializeOnLoadMethod]
        private static void ResumeRunningJobs()
        {
            foreach (var testRun in TestJobDataHolder.instance.TestRuns)
            {
                new TestJobRunner(testRun);
            }
        }

        private TestJobData m_JobData;

        private TestCommandPcHelper m_PcHelper;
        private TestTaskBase[] Tasks;

        public TestJobRunner(TestJobData data)
        {
            m_JobData = data;
            m_PcHelper = new EditModePcHelper();
            Tasks = GetTaskList(data.executionSettings).ToArray();
            EditorApplication.update += ExecuteStep;
        }

        private void ExecuteStep()
        {
            if (!m_JobData.isRunning)
            {
                m_JobData.isRunning = true;
                TestJobDataHolder.instance.TestRuns.Add(m_JobData);
            }

            if (m_JobData.taskIndex >= Tasks.Length)
            {
                StopRun();
                return;
            }

            var task = Tasks[m_JobData.taskIndex];
            var enumerator = task.Execute(m_JobData);
            m_PcHelper.SetEnumeratorPC(enumerator, m_JobData.taskPC);

            try
            {
                if (!enumerator.MoveNext())
                {
                    m_JobData.taskIndex++;
                    m_JobData.taskPC = 0;
                    return;
                }
            }
            catch (TestRunCanceledException)
            {
                StopRun();
                return;
            }
            catch (Exception ex)
            {
                StopRun();
                Debug.LogException(ex);
                CallbacksDelegator.instance.RunFailed("An unexpected error happened while running tests.");
                return;
            }
            
            m_JobData.taskPC = m_PcHelper.GetEnumeratorPC(enumerator);
        }

        private void StopRun()
        {
            m_JobData.isRunning = false;
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= ExecuteStep;
            TestJobDataHolder.instance.TestRuns.Remove(m_JobData);
        }
    }
}