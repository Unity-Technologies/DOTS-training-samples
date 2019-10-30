using System;
using NUnit.Framework.Interfaces;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace UnityEditor.TestTools.TestRunner.TestRun
{
    [Serializable]
    internal class TestJobData
    {
        [SerializeField] 
        public int taskIndex;

        [SerializeField] 
        public int taskPC;

        [SerializeField] 
        public bool isRunning;
        
        [SerializeField]
        public ExecutionSettings executionSettings;
        
        [SerializeField]
        public string[] existingFiles;

        public ITest testTree;

        public TestJobData(ExecutionSettings settings)
        {
            executionSettings = settings;
            isRunning = false;
            taskIndex = 0;
            taskPC = 0;
        }
    }
}
