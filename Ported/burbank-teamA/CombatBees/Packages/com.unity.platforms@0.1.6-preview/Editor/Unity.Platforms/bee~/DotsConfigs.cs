using DotsBuildTargets;
using Newtonsoft.Json.Linq;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class DotsConfigs
{
    static IEnumerable<DotsRuntimeCSharpProgramConfiguration> MakeConfigs()
    {
        var platformList = new List<DotsBuildSystemTarget>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            foreach (var type in types)
            {
                if (type.IsAbstract)
                    continue;

                if (!type.IsSubclassOf(typeof(DotsBuildSystemTarget)))
                    continue;

                platformList.Add((DotsBuildSystemTarget)Activator.CreateInstance(type));
            }
        }

        foreach (var platform in platformList)
        {
            foreach (var config in platform.GetConfigs())
                yield return config;
        }
    }

    private static readonly Lazy<DotsRuntimeCSharpProgramConfiguration[]> _configs = new Lazy<DotsRuntimeCSharpProgramConfiguration[]>(() => MakeConfigs().ToArray());

    private static Lazy<DotsRuntimeCSharpProgramConfiguration> _projectFileConfig = new Lazy<DotsRuntimeCSharpProgramConfiguration>(
        () => ReadConfigFromSelectConfigFile() ?? HostDotnet);

    private static DotsRuntimeCSharpProgramConfiguration ReadConfigFromSelectConfigFile()
    {
        var file = NPath.CurrentDirectory.Combine("selectedconfig.json");
        if (!file.FileExists())
            return null;

        var json = file.ReadAllText();
        var jobject = JObject.Parse(json);

        var configName = jobject["Config"].Value<string>().ToLower();

        return _configs.Value.FirstOrDefault(c => c.Identifier == configName);
    }

    static Lazy<DotsRuntimeCSharpProgramConfiguration> _multiThreadedJobsTestConfig = new Lazy<DotsRuntimeCSharpProgramConfiguration>(()=> HostDotnet.WithMultiThreadedJobs(true).WithIdentifier(HostDotnet.Identifier + "-mt"));
    public static DotsRuntimeCSharpProgramConfiguration MultithreadedJobsTestConfig => _multiThreadedJobsTestConfig.Value;

    public static DotsRuntimeCSharpProgramConfiguration HostDotnet => _configs.Value.First(c => c.ScriptingBackend == ScriptingBackend.Dotnet && c.Platform.GetType() == Unity.BuildSystem.NativeProgramSupport.Platform.HostPlatform.GetType());
    public static DotsRuntimeCSharpProgramConfiguration[] Configs => BuildProgram.IsRequestedTargetExactlyProjectFiles() ? new[] { ProjectFileConfig } : _configs.Value;

    //todo: read this from buildsettings config file
    public static DotsRuntimeCSharpProgramConfiguration ProjectFileConfig => _projectFileConfig.Value;
}