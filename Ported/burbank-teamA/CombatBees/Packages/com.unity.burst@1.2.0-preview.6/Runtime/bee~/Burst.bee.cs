using System;
using System.Collections.Generic;
using System.Linq;
using Bee.Core;
using Bee.DotNet;
using NiceIO;
using Unity.BuildSystem.NativeProgramSupport;
using Unity.BuildTools;

public abstract class BurstCompiler
{
    public static NPath BurstExecutable { get; set; }
    public abstract string TargetPlatform { get; }
    public abstract string TargetArchitecture { get; set; }
    public abstract string ObjectFormat { get; }
    public abstract string ObjectFileExtension { get; }
    public abstract bool UseOwnToolchain { get; }

    // Options
    public virtual bool SafetyChecks { get; } = false;
    public virtual bool DisableVectors { get; } = false;
    public virtual bool Link { get; set; } = true;
    public abstract string FloatPrecision { get; }

    static string[] GetBurstCommandLineArgs(BurstCompiler compiler, NPath outputPrefixForObjectFile, NPath outputDirForPatchedAssemblies, string pinvokeName, DotNetAssembly[] inputAssemblies)
    {
        var commandLineArguments = new[]
        {
            $"--platform={compiler.TargetPlatform}",
            $"--target={compiler.TargetArchitecture}",
            $"--format={compiler.ObjectFormat}",
            compiler.SafetyChecks ? "--safety-checks" : "",
            $"--dump=\"None\"",
            compiler.DisableVectors ? "--disable-vectors" : "",
            compiler.Link ? "" : "--nolink",
            $"--float-precision={compiler.FloatPrecision}",
            $"--keep-intermediate-files",
            "--verbose",
            $"--patch-assemblies-into={outputDirForPatchedAssemblies}",
            $"--output={outputPrefixForObjectFile}",
            $"--only-static-methods",
            "--method-prefix=burstedmethod_",
            $"--pinvoke-name={pinvokeName}"
        }.Concat(inputAssemblies.Select(asm => $"--root-assembly={asm.Path}"));
        if (!compiler.UseOwnToolchain)
            commandLineArguments = commandLineArguments.Concat(new[] { "--no-native-toolchain" });

        if (!HostPlatform.IsWindows)
            commandLineArguments = new[] { BurstExecutable.ToString(SlashMode.Native) }.Concat(commandLineArguments);

        var commandLineArgumentsArray = commandLineArguments.ToArray();
        return commandLineArgumentsArray;
    }

    static IEnumerable<NPath> AddDebugSymbolPaths(DotNetAssembly[] assemblies)
    {
        return assemblies.SelectMany(asm =>
        {
            var ret = new List<NPath> {asm.Path};
            if (asm.DebugSymbolPath != null)
                ret.Add(asm.DebugSymbolPath);
            return ret;
        });
    }

    public static BagOfObjectFilesLibrary SetupBurstCompilationForAssemblies(
        BurstCompiler compiler,
        DotNetAssembly unpatchedInputAssembly,
        NPath outputDirForObjectFile,
        NPath outputDirForPatchedAssemblies,
        string pinvokeName,
        out DotNetAssembly patchedAssembly)
    {
        if (compiler.Link)
        {
            throw new ArgumentException("BurstCompiler.Link must be false for SetupBurstCompilationForAssemblies");
        }

        // FIXME: burst can generate multiple object files when BurstCompile attribute has different settings
        // e.g. [BurstCompile(FloatPrecision.Medium, FloatMode.Deterministic)]
        var objectFile = outputDirForObjectFile.Combine("lib_burst_generated_part_0" + compiler.ObjectFileExtension);

        patchedAssembly = unpatchedInputAssembly.ApplyDotNetAssembliesPostProcessor(
            outputDirForPatchedAssemblies,
            (inputAssemblies, targetDir) =>
            {
                var executableStringFor = HostPlatform.IsWindows ? BurstExecutable.ToString(SlashMode.Native) : "mono";
                var commandLineArgs = GetBurstCommandLineArgs(
                    compiler,
                    outputDirForObjectFile.Combine(pinvokeName),
                    outputDirForPatchedAssemblies,
                    pinvokeName,
                    inputAssemblies);

                var inputPaths = AddDebugSymbolPaths(inputAssemblies);
                var targetFiles = inputPaths.Select(p => targetDir.Combine(p.FileName)).Concat(new []{ objectFile });

                Backend.Current.AddAction(
                    "Burst",
                    //todo: make burst process pdbs
                    targetFiles.ToArray(),
                    inputPaths.Concat(new []{ BurstExecutable }).ToArray(),
                    executableStringFor,
                    commandLineArgs);
            });

        return new BagOfObjectFilesLibrary(new[] {objectFile});
    }

    public static DynamicLibrary SetupBurstCompilationAndLinkForAssemblies(
        BurstCompiler compiler,
        DotNetAssembly unpatchedInputAssembly,
        NPath targetNativeLibrary,
        NPath outputDirForPatchedAssemblies,
        out DotNetAssembly patchedAssembly)
    {
        if (!compiler.Link)
        {
            throw new ArgumentException("BurstCompiler.Link must be true for SetupBurstCompilationAndLinkForAssemblies");
        }

        patchedAssembly = unpatchedInputAssembly.ApplyDotNetAssembliesPostProcessor(
            outputDirForPatchedAssemblies,
            (inputAssemblies, targetDir) =>
            {
                var executableStringFor = HostPlatform.IsWindows ? BurstExecutable.ToString(SlashMode.Native) : "mono";

                var pinvokeName = HostPlatform.IsWindows ? targetNativeLibrary.FileNameWithoutExtension : targetNativeLibrary.FileName;
                var commandLineArgs = GetBurstCommandLineArgs(
                    compiler,
                    targetNativeLibrary.ChangeExtension(""),
                    outputDirForPatchedAssemblies,
                    pinvokeName,
                    inputAssemblies);

                var inputPaths = AddDebugSymbolPaths(inputAssemblies);
                var targetFiles = inputPaths.Select(p => targetDir.Combine(p.FileName)).Concat(new [] { targetNativeLibrary });

                Backend.Current.AddAction(
                    "Burst",
                    //todo: make burst process pdbs
                    targetFiles.ToArray(),
                    inputPaths.Concat(new []{ BurstExecutable }).ToArray(),
                    executableStringFor,
                    commandLineArgs);
            });

        return new DynamicLibrary(targetNativeLibrary, symbolFiles: null);
    }
}

public class BurstCompilerForEmscripten : BurstCompiler
{
    public override string TargetPlatform { get; } = "Wasm";
    public override string TargetArchitecture { get; set; } = "WASM32";
    public override string ObjectFormat { get; } = "Wasm";
    public override string FloatPrecision { get; } = "High";
    public override bool SafetyChecks { get; } = true;
    public override bool DisableVectors { get; } = true;
    public override bool Link => false;
    public override string ObjectFileExtension { get; } = ".ll";
    public override bool UseOwnToolchain { get; } = false;
}

public class BurstCompilerForWindows : BurstCompiler
{
    public override string TargetPlatform { get; } = "Windows";

    //--target=VALUE         Target CPU <Auto|X86_SSE2|X86_SSE4|X64_SSE2|X64_
    //    SSE4|AVX|AVX2|AVX512|WASM32|ARMV7A_NEON32|ARMV8A_
    //    AARCH64|THUMB2_NEON32> Default: Auto
    public override string TargetArchitecture { get; set; } = "X86_SSE2";
    public override string ObjectFormat { get; } = "Coff";
    public override string FloatPrecision { get; } = "High";
    public override bool SafetyChecks { get; } = true;
    public override bool DisableVectors { get; } = false;
    public override bool Link { get; set; } = false;//true;
    public override string ObjectFileExtension { get; } = ".obj";
    public override bool UseOwnToolchain { get; } = true;
}

public class BurstCompilerForWindows64 : BurstCompilerForWindows
{
    public override string TargetArchitecture { get; set; } = "X64_SSE4";
}

public class BurstCompilerForMac : BurstCompiler
{
    public override string TargetPlatform { get; } = "macOS";

    //--target=VALUE         Target CPU <Auto|X86_SSE2|X86_SSE4|X64_SSE2|X64_
    //    SSE4|AVX|AVX2|AVX512|WASM32|ARMV7A_NEON32|ARMV8A_
    //    AARCH64|THUMB2_NEON32> Default: Auto
    public override string TargetArchitecture { get; set; } = "X64_SSE2";
    public override string ObjectFormat { get; } = "MachO";
    public override string FloatPrecision { get; } = "High";
    public override bool SafetyChecks { get; } = true;
    public override bool DisableVectors { get; } = false;
    public override bool Link { get; set; } = false;
    public override string ObjectFileExtension { get; } = ".o";
    public override bool UseOwnToolchain { get; } = true;
}
