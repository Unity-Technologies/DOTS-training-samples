/// AutoBuildPlayerSetting.cs - automatically setup player settings
///


using UnityEngine;
using UnityEditor;

namespace Ext {

    public class AutoBuildPlayerSetting
    {

        private static void SetupCommonSettings()
        {
            PlayerSettings.graphicsJobMode = GraphicsJobMode.Native;
            PlayerSettings.graphicsJobs = false;

            PlayerSettings.muteOtherAudioSources = false;

            PlayerSettings.colorSpace = ColorSpace.Gamma;
            PlayerSettings.companyName = "Unity Technologies";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;
            PlayerSettings.MTRendering = true;
            PlayerSettings.productName = "DotsTrainingGroup2";
            PlayerSettings.allowedAutorotateToPortrait = PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        }

        public static void SetupAndroid()
        {
            SetupCommonSettings();

            PlayerSettings.Android.minifyDebug = false;
            PlayerSettings.Android.minifyRelease = false;

            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            PlayerSettings.gpuSkinning = false;

            PlayerSettings.Android.androidIsGame = false;
            PlayerSettings.Android.androidTVCompatibility = false;
            PlayerSettings.Android.ARCoreEnabled = false;
            PlayerSettings.Android.blitType = AndroidBlitType.Auto;
            PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            PlayerSettings.Android.disableDepthAndStencilBuffers = false;
            PlayerSettings.Android.forceInternetPermission = true;
            PlayerSettings.Android.forceSDCardPermission = true;
            PlayerSettings.Android.keystoreName = "group2.keystore";
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.useCustomKeystore = true;

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.unity.dotstraining.group2");

            PlayerSettings.Android.keyaliasName = "group2";
            PlayerSettings.Android.keyaliasPass = "group2";
            PlayerSettings.Android.keystoreName = "./Credentials/group2.keystore";
            PlayerSettings.Android.keystorePass = "group2";

            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Low);
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);
        }

    }

}
