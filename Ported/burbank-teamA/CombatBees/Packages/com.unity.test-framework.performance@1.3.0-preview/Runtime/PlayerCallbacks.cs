using System;
using UnityEngine;
using UnityEngine.TestRunner;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Runtime;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

[assembly: TestRunCallback(typeof(PlayerCallbacks))]

namespace Unity.PerformanceTesting
{
    public class PlayerCallbacks: ITestRunCallback
    {
        private static bool saved;
        public void RunStarted(ITest testsToRun)
        {
        }

        public void RunFinished(ITestResult testResults)
        {
        }

        public void TestStarted(ITest test)
        {
        }

        public void TestFinished(ITestResult result)
        {
            if(saved) return;
            var run = ReadPerformanceTestRun();
            run.PlayerSystemInfo = GetSystemInfo();
            run.QualitySettings = GetQualitySettings();
            run.ScreenSettings = GetScreenSettings();
            run.TestSuite = Application.isEditor ? "Editmode" : "Playmode";
            run.BuildSettings.Platform = Application.platform.ToString();

            TestContext.Out?.Write("##performancetestruninfo:" + JsonUtility.ToJson(run));
            saved = true;
        }
        
        private PerformanceTestRun ReadPerformanceTestRun()
        {
            try
            {            
                var runResource = Resources.Load<TextAsset>(Utils.TestRunInfo.Replace(".json", ""));
                var json = Application.isEditor ? PlayerPrefs.GetString(Utils.PlayerPrefKeyRunJSON) : runResource.text;
                return JsonUtility.FromJson<PerformanceTestRun>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }
        
        private static PlayerSystemInfo GetSystemInfo()
        {
            return new PlayerSystemInfo
            {
                OperatingSystem = SystemInfo.operatingSystem,
                DeviceModel = SystemInfo.deviceModel,
                DeviceName = SystemInfo.deviceName,
                ProcessorType = SystemInfo.processorType,
                ProcessorCount = SystemInfo.processorCount,
                GraphicsDeviceName = SystemInfo.graphicsDeviceName,
                SystemMemorySize = SystemInfo.systemMemorySize,
#if ENABLE_XR
                XrModel = UnityEngine.XR.XRDevice.model,
                XrDevice = UnityEngine.XR.XRSettings.loadedDeviceName
#endif
            };
        }

        private QualitySettings GetQualitySettings()
        {
            return new QualitySettings()
            {
                Vsync = UnityEngine.QualitySettings.vSyncCount,
                AntiAliasing = UnityEngine.QualitySettings.antiAliasing,
                ColorSpace = UnityEngine.QualitySettings.activeColorSpace.ToString(),
                AnisotropicFiltering = UnityEngine.QualitySettings.anisotropicFiltering.ToString(),
                BlendWeights = UnityEngine.QualitySettings.skinWeights.ToString()
            };
        }

        private ScreenSettings GetScreenSettings()
        {
            return new ScreenSettings
            {
                ScreenRefreshRate = Screen.currentResolution.refreshRate,
                ScreenWidth = Screen.currentResolution.width,
                ScreenHeight = Screen.currentResolution.height,
                Fullscreen = Screen.fullScreen
            };
        }
    }
}