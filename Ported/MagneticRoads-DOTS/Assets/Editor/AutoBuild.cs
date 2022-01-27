using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Ext
{

    public class AutoBuild
    {

        [MenuItem("Tools/Quick Build Android Development Build/Build and Run on connected Android device")]
        public static void BuildAndRunAndroidAPK()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "BUILD_INTERNAL");
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
            PerformBuildAndroid(true);
        }

        public static void PerformBuildAndroid(bool bRun = false)
        {

            //select enabled scene to scenes list
            string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

            AutoBuildPlayerSetting.SetupAndroid();

            return;

            /*BuildPlayerOptions bpo = new BuildPlayerOptions();
            bpo.scenes = scenes;
            bpo.target = BuildTarget.Android;
            bpo.locationPathName = "./group2.apk";

            //check if that file exist
            if (File.Exists(bpo.locationPathName) == true)
            {
                Debug.Log("<AutoBuild.PerformBuildAndroid> file " + bpo.locationPathName + " exist. Remove it ...");
                FileAttributes attr = File.GetAttributes(bpo.locationPathName);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    Directory.Delete(bpo.locationPathName);
                else
                    File.Delete(bpo.locationPathName);
            }

            bpo.options = BuildOptions.None;
            if (bRun == true)
            {
                bpo.options |= BuildOptions.AutoRunPlayer;
            }
            bpo.options |= BuildOptions.Development;
            bpo.options |= BuildOptions.ConnectWithProfiler;

            bpo.targetGroup = BuildTargetGroup.Android;
            BuildReport report = BuildPipeline.BuildPlayer(bpo);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("<AutoBuild.PerformBuild> Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("<AutoBuild.PerformBuild> Build failed #" + summary.totalErrors);
            }*/
        }

        public static void AutoBuildAndroid()
        {
            Debug.Log("<AutoBuild.AutoBuildAndroid> start Android build automation...");
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "BUILD_INTERNAL");
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            PerformBuildAndroid(false);
        }

    }

}