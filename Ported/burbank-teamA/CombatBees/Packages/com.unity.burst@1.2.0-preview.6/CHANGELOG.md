# Changelog

## [1.2.0-preview.6] - 2019-10-16

- New multi-threaded compilation support when building a standalone player.
- Improve `BurstCompiler.CompileFunctionPointer` to compile asynchronously function pointers in the Editor
- Improve of error codes and messages infrastructure.
- Upgraded Burst to use LLVM Version 8.0.1 by default, bringing the latest optimization improvements from the LLVM project.
- Fix issue with libtinfo5 missing on Linux.
- Fix possible NullReferenceException when an entry point function is calling another empty function.
- Fix an exception occurring while calculating the size of a struct with indirect dependencies to itself.
- Fix potential failure when loading MDB debugging file.
- Fix linker issue with folder containing spaces.
- Fix issue with package validation by removing ifdef around namespaces.
- Fix issue with an internal compiler exception related to an empty stack

## [1.2.0-preview.5] - 2019-09-23

- Fix crashing issue during the shutdown of the editor.

## [1.2.0-preview.4] - 2019-09-20

- Fix a logging issue on shutdown.

## [1.2.0-preview.3] - 2019-09-20

- Fix potential logging of an error while shutting down the editor.

## [1.2.0-preview.2] - 2019-09-20

- New multi-threaded compilation of jobs/function pointers in the editor.
- Improve caching of compiled jobs/function pointers.
- Fix a caching issue where some jobs/function pointers would not be updated in the editor when updating their code.
- Fix an issue where type initializers with interdependencies were not executed in the correct order.
- Fix an issue with `Failed to resolve assembly Windows, Version=255.255.255.255...` when building for Xbox One.
- Fix compilation error on ARM32 when calling an external function.
- Fix an issue with function pointers that would generate invalid code if a non-blittable type is used in a struct passed by ref.
- Fix an issue with function pointers that would generate invalid code in case containers/pointers passed to the function are memory aliased.
- Report a compiler error if a function pointer is trying to be compiled without having the `[BurstCompile]` attribute on the method and owning type.

## [1.2.0-preview.1] - 2019-09-09

- Fix assembly caching issue, cache usage now conservative (Deals with methods that require resolving multiple assemblies prior to starting the compilation - generics).
- Fix Mac OS compatibility of Burst (10.10 and up) - fixes undefined symbol _futimens_.

## [1.1.3-preview.3] - 2019-09-02

- Query android API target level from player settings when building android standalone players.
- Add calli opcode support to support bindings to native code.

## [1.1.3-preview.2] - 2019-08-29

- Fix to allow calling [BurstDiscard] functions from static constructors.
- Correctly error if a DLLImport function uses a struct passed by value, but allow handle structs (structs with a single pointer/integer in them) as these require no ABI pain.
- Upgraded Burst to use LLVM Version 8 by default, bringing the latest optimisation improvements from the LLVM project.
- Added support for multiple LLVM versions, this does increase the package size, however it allows us to retain compatability with platforms that still require older versions of LLVM.
- Fix bug in assembly caching, subsequent runs should now correctly use cached jit code as appropriate.
- Add support for Lumin platform

## [1.1.3-preview.1] - 2019-08-26

- Add support for use of the MethodImpl(MethodImplOptions.NoOptimization) on functions.
- Fix an issue whereby static readonly vector variables could not be constructed unless using the constructor whose number of elements matched the width of the vector.
- Fix an issue whereby static readonly vector variables could not be struct initialized.
- Improve codegen for structs with explicit layout and overlapping fields.
- Fix a bug causing SSE4 instructions to be run on unsupported processors.
- Fix an issue where storing a pointer would fail as our type normalizer would cast the pointer to an i8.
- Begin to add Burst-specific aliasing information by instructing LLVM on our stack-allocation and global variables rules.

## [1.1.2] - 2019-07-26

- Fix an issue where non-readonly static variable would not fail in Burst while they are not supported.
- Fix issue with char comparison against an integer. Add partial support for C# char type.
- Improve codegen for struct layout with simple explicit layout.
- Fix NullReferenceException when using a static variable with a generic declaring type.
- Fix issue with `stackalloc` not clearing the allocated stack memory as it is done in .NET CLR.

## [1.1.1] - 2019-07-11

- Fix a compiler error when using a vector type as a generic argument of a NativeHashMap container.
- Disable temporarily SharedStatic/Execution mode for current 2019.3 alpha8 and before.
- Fix detection of Android NDK for Unity 2019.3.
- Update documentation for known issues.

## [1.1.0] - 2019-07-09

- Fix detection of Android NDK for Unity 2019.3.
- Update documentation for known issues.

## [1.1.0-preview.4] - 2019-07-05

- Burst will now report a compilation error when writing to a `[ReadOnly]` container/variable.
- Fix regression with nested generics resolution for interface calls.
- Fix issue for UWP with Burst generating non appcert compliant binaries.
- Fix issue when reading/writing vector types to a field of an explicit layout.
- Fix build issue on iOS, use only hash names for platforms with clang toolchain to mitigate issues with long names in LLVM IR.
- Allow calls to intrinsic functions (e.g `System.Math.Log`) inside static constructors.
- Improve performance when detecting if a method needs to be recompiled at JIT time.
- Fix an issue with explicit struct layout and vector types.

## [1.1.0-preview.3] - 2019-06-28

- Fix issue with generic resolution that could fail.
- Add support for readonly static data through generic instances.
- Add internal support for `SharedStatic<T>` for TypeManager.
- Add intrinsic support for `math.bitmask`.

## [1.1.0-preview.2] - 2019-06-20

- Fix issue where uninitialized values would be loaded instead for native containers containing big structs.
- Fix issue where noalias analysis would fail for native containers containing big structs.
- Fix issue when calling "internal" methods that take bool parameters.
- Add support for `MethodImplOptions.AggressiveInlining` to force inlining.
- Fix issue in ABITransform that would cause compilation errors with certain explicit struct layouts.
- Disable debug information generation for PS4 due to IR compatability issue with latest SDK.
- Implemented an assembly level cache for JIT compilation to improve iteration times in the Editor.
- Implement a hard cap on the length of symbols to avoid problems for platforms that ingest IR for AOT.
- Add support for `FunctionPointer<T>` usable from Burst Jobs via `BurstCompiler.CompileFunctionPointer<T>`.
- Add `BurstCompiler.Options` to allow to control/enable/disable Burst jobs compilation/run at runtime.
- Add `BurstRuntime.GetHashCode32<T>` and `GetHashCode64<T>` to allow to generate a hash code for a specified time from a Burst job.

## [1.0.0] - 2019-04-16

- Release stable version.

## [1.0.0-preview.14] - 2019-04-15

- Bump to mathematics 1.0.1
- Fix android ndk check on windows when using the builtin toolchain.
- Fix crash when accessing a field of a struct with an explicit layout through an embedded struct.
- Fix null pointer exception on building for android if editor version is less than 2019.1.
- Workaround IR compatibility issue with AOT builds on IOS.

## [1.0.0-preview.13] - 2019-04-12

- Fix linker error on symbol `$___check_bounds already defined`.
- Fix StructLayout Explicit size calculation and backing storage.

## [1.0.0-preview.12] - 2019-04-09

- Fix crash when accessing a NativeArray and performing in-place operations (e.g `nativeArray[i] += 121;`).

## [1.0.0-preview.11] - 2019-04-08

- Improve error logging for builder player with Burst.
- Fix NullReferenceException when storing to a field which is a generic type.

## [1.0.0-preview.10] - 2019-04-05

- Update known issues in the user manual.
- Improve user manual documentation about debugging, `[BurstDiscard]` attribute, CPU architectures supported...
- Fix an issue where Burst callbacks could be sent to the editor during shutdowns, causing an editor crash.
- Improve error messages for external tool chains when building for AOT.

## [1.0.0-preview.9] - 2019-04-03

- Fix an auto-vectorizer issue not correctly detecting the safe usage of NativeArray access when performing in-place operations (e.g `nativeArray[i] += 121;`).
- Add support for dynamic dispatch of functions based on CPU features available at runtime.
  - Fix issue when running SSE4 instructions on a pre-SSE4 CPU.
- Fix write access to `NativeArray<bool>`.
- Remove dependencies to C runtime for Windows/Linux build players (for lib_burst_generated.so/.dll).
- Updated API documentation.
- Update User manual.
- Static link some libraries into the Burst llvm wrapper to allow better support for some linux distros.

## [1.0.0-preview.8] - 2019-03-28

- Fix for iOS symbol names growing too long, reduced footprint of function names via pretty printer and a hash.

## [1.0.0-preview.7] - 2019-03-28

- Burst will now only generate debug information for AOT when targeting a Development Build.
- Added support for locating the build tools (standalone) for generating AOT builds on windows, without having to install Visual Studio complete.
- Fix Log Timings was incorrectly being passed along to AOT builds, causing them to fail.
- Fix editor crash if Burst aborted compilation half way through (because editor was being closed).
- Fix issue with job compilation that could be disabled when using the Burst inspector.
- Fix issue with spaces in certain paths (e.g. ANDROID_NDK_ROOT) when building for AOT.
- Restore behavior of compiling ios projects from windows with Burst, (Burst does not support cross compiling for ios) - we still generate a valid output project, but with no Burst code.
- Add support for Android embedded NDK.
- Fix issue where certain control flow involving object construction would crash the compiler in release mode.

## [1.0.0-preview.6] - 2019-03-17

- Fix invalid codegen with deep nested conditionals.
- Fix issue with Burst menu "Enable Compilation" to also disable cache jobs.
- Improve handling of PS4 toolchain detection.

## [1.0.0-preview.5] - 2019-03-16

- Fix regression with JIT caching that was not properly recompiling changed methods.
- Remove NativeDumpFlags from public API.
- Remove usage of PropertyChangingEventHandler to avoid conflicts with custom Newtonsoft.Json.
- Fix issue when a job could implement multiple job interfaces (IJob, IJobParallelFor...) but only the first one would be compiled.

## [1.0.0-preview.4] - 2019-03-15

- Fix "Error while verifying module: Invalid bitcast" that could happen with return value in the context of deep nested conditionals.
- Fix support for AOT compilation with float precision/mode.
- Fix fast math for iOS/PS4.
- Fix issue with double not using optimized intrinsics for scalars.
- Fix issue when loading a MDB file was failing when building a standalone player.
- Fix no-alias analysis that would be disabled in a standalone player if only one of the method was failing.
- Fix bug with explicit layout struct returned as a pointer by a property but creating an invalid store.
- Change `FloatPrecision.Standard` defaulting from `FloatPrecision.High` (ULP1) to `FloatPrecision.Medium` (ULP3.5).

## [1.0.0-preview.3] - 2019-03-14

- Fix compilation issue with uTiny builds.

## [1.0.0-preview.2] - 2019-03-13

- Fix no-alias warning spamming when building a standalone player.
- Improve the layout of the options/buttons for the inspector so that they at least attempt to layout better when the width is too small for all the buttons.
- Fix formatting of error messages so the Unity Console can correctly parse the location as a clickable item (Note however it does not appear to allow double clicking on absolute paths).
- Change Burst menu to Jobs/Burst. Improve order of menu items.
- Fix for AOTSettings bug related to StandaloneWindows vs StandaloneWindows64.

## [1.0.0-preview.1] - 2019-03-11

- Fix regression when resolving the type of generic used in a field.
- Fix linker for XboxOne, UWP.
- Fix performance codegen when using large structs.
- Fix codegen when a recursive function is involved with platform dependent ABI transformations.

## [0.2.4-preview.50] - 2019-02-27

- Fix meta file conflict.
- Fix changelog format.

## [0.2.4-preview.49] - 2019-02-27

- Move back com.unity.burst.experimental for function pointers support, but use internal modifier for this API.
- Restructure package for validation.

## [0.2.4-preview.48] - 2019-02-26

- Move back com.unity.burst.experimental for function pointers support, but use internal modifier for this API.

## [0.2.4-preview.47] - 2019-02-26

- Fix an issue during publish stage which was preventing to release the binaries.

## [0.2.4-preview.46] - 2019-02-26

- iOS player builds now use static linkage (to support TestFlight)  - Minimum supported Unity versions are 2018.3.6f1 or 2019.1.0b4.
- Fix a warning in Burst AOT settings.
- Enable forcing synchronous job compilation from menu.

## [0.2.4-preview.45] - 2019-02-07

- Disable Burst AOT settings support for unity versions before 2019.1.

## [0.2.4-preview.44] - 2019-02-06

- Fix incorrect conversions when performing subtraction with enums and floats.
- Fix compatability issue with future unity versions.
- Fix bug with ldfld bitcast on structs with explicit layouts.
- Guard against an issue resolving debug locations if the scope is global.

## [0.2.4-preview.43] - 2019-02-01

- Add preliminary support for Burst AOT settings in the player settings.
- Move BurstCompile (delegate/function pointers support) from com.unity.burst package to com.unity.burst.experimental package.
- Fix issue with stackalloc allocating a pointer size for the element type resulting in possible StackOverflowException.
- Add support for disabling Burst compilation from Unity editor with the command line argument `--burst-disable-compilation` .
- Add support for forcing synchronous compilation from Unity editor with the command line argument `--burst-force-sync-compilation`.
- Fix a compiler crash when generating debugging information.
- Fix invalid codegen involving ternary operator

## [0.2.4-preview.42] - 2019-01-22

- Fix a compilation error when implicit/explicit operators are used returning different type for the same input type.

## [0.2.4-preview.41] - 2019-01-17

- Fix codegen issue with Interlocked.Decrement that was instead performing an increment.
- Fix codegen issue for an invalid layout of struct with nested recursive pointer references.
- Fix for Fogbugz case : https://fogbugz.unity3d.com/f/cases/1109514/.
- Fix codegen issue with ref bool on a method argument creating a compiler exception.

## [0.2.4-preview.40] - 2018-12-19

- Fix bug when a write to a pointer type of an argument of a generic function.
- Breaking change of API: `Accuracy` -> `FloatPrecision`, and `Support` => `FloatMode`.
- Add `FloatMode.Deterministic` mode with early preview of deterministic mathematical functions.
- Fix bug with fonts in inspector being incorrectly reloaded.

## [0.2.4-preview.39] - 2018-12-06

- Add preview support for readonly static arrays typically used for LUT.
- Fix an issue with generics incorrectly being resolved in certain situations.
- Fix ARM32/ARM64 compilation issues for some instructions.
- Fix ARM compilation issues on UWP.
- Fix issue with math.compress.
- Add support for `ldnull` for storing a managed null reference to a ref field (e.g for DisposeSentinel).

## [0.2.4-preview.38] - 2018-11-17

- Fix issue when converting an unsigned integer constant to a larger unsigned integer (e.g (ulong)uint.MaxValue).
- Fix crash in editor when IRAnalysis can return an empty string .
- Fix potential crash of Cecil when reading symbols from assembly definition.

## [0.2.4-preview.37] - 2018-11-08

- Fix a crash on Linux and MacOS in the editor with dlopen crashing when trying to load burst-llvm (linux).

## [0.2.4-preview.36] - 2018-11-08

- Fix a crash on Linux and MacOS in the editor with dlopen crashing when trying to load burst-llvm (mac).

## [0.2.4-preview.35] - 2018-10-31

- Try to fix a crash on macosx in the editor when a job is being compiled by Burst at startup time.
- Fix Burst accidentally resolving reference assemblies.
- Add support for Burst for ARM64 when building UWP player.

## [0.2.4-preview.34] - 2018-10-12

- Fix compiler exception with an invalid cast that could occur when using pinned variables (e.g `int32&` resolved to `int32**` instead of `int32*`).

## [0.2.4-preview.33] - 2018-10-10

- Fix a compiler crash with methods incorrectly being marked as external and throwing an exception related to ABI.

## [0.2.4-preview.32] - 2018-10-04

- Fix codegen and linking errors for ARM when using mathematical functions on plain floats.
- Add support for vector types GetHashCode.
- Add support for DllImport (only compatible with Unity `2018.2.12f1`+ and ` 2018.3.0b5`+).
- Fix codegen when converting uint to int when used in a binary operation.

## [0.2.4-preview.31] - 2018-09-24

- Fix codegen for fmodf to use inline functions instead.
- Add extended disassembly output to the Burst inspector.
- Fix generic resolution through de-virtualize methods.
- Fix bug when accessing float3.zero. Prevents static constructors being considered intrinsics.
- Fix NoAlias attribute checking when generics are used.

## [0.2.4-preview.30] - 2018-09-11

- Fix IsValueType throwing a NullReferenceException in case of using generics.
- Fix discovery for Burst inspector/AOT methods inheriting from IJobProcessComponentData or interfaces with generics.
- Add `[NoAlias]` attribute.
- Improved codegen for csum.
- Improved codegen for abs(int).
- Improved codegen for abs on floatN/doubleN.

## [0.2.4-preview.29] - 2018-09-07

- Fix issue when calling an explicit interface method not being matched through a generic constraint.
- Fix issue with or/and binary operation on a bool returned by a function.

## [0.2.4-preview.28] - 2018-09-05

- Fix a compilation issue when storing a bool returned from a function to a component of a bool vector.
- Fix AOT compilation issue with a duplicated dictionary key.
- Fix settings of ANDROID_NDK_ROOT if it is not setup in Unity Editor.

## [0.2.4-preview.27] - 2018-09-03

- Improve detection of jobs within nested generics for AOT/Burst inspector.
- Fix compiler bug of comparison of a pointer to null pointer.
- Fix crash compilation of sincos on ARM (neon/AARCH64).
- Fix issue when using a pointer to a VectorType resulting in an incorrect access of a vector type.
- Add support for doubles (preview).
- Improve AOT compiler error message/details if the compiler is failing before the linker.

## [0.2.4-preview.26] - 2018-08-21

- Added support for cosh, sinh and tanh.

## [0.2.4-preview.25] - 2018-08-16

- Fix warning in unity editor.

## [0.2.4-preview.24] - 2018-08-15

- Improve codegen of math.compress.
- Improve codegen of math.asfloat/asint/asuint.
- Improve codegen of math.csum for int4.
- Improve codegen of math.count_bits.
- Support for lzcnt and tzcnt intrinsics.
- Fix AOT compilation errors for PS4 and XboxOne.
- Fix an issue that could cause wrong code generation for some unsafe ptr operations.

## [0.2.4-preview.23] - 2018-07-31

- Fix bug with switch case to support not only int32.

## [0.2.4-preview.22] - 2018-07-31

- Fix issue with pointers comparison not supported.
- Fix a StackOverflow exception when calling an interface method through a generic constraint on a nested type where the declaring type is a generic.
- Fix an issue with EntityCommandBuffer.CreateEntity/AddComponent that could lead to ArgumentException/IndexOutOfRangeException.

## [0.2.4-preview.21] - 2018-07-25

- Correct issue with Android AOT compilation being unable to find the NDK.

## [0.2.4-preview.20] - 2018-07-05

- Prepare the user documentation for a public release.

## [0.2.4-preview.19] - 2018-07-02

- Fix compilation error with generics when types are coming from different assemblies.

## [0.2.4-preview.18] - 2018-06-26

- Add support for subtracting pointers.

## [0.2.4-preview.17] - 2018-06-25

- Bump only to force a new version pushed.

## [0.2.4-preview.16] - 2018-06-25

- Fix AOT compilation errors.

## [0.2.4-preview.15] - 2018-06-25

- Fix crash for certain access to readonly static variable.
- Fix StackOverflowException when using a generic parameter type into an interface method.

## [0.2.4-preview.14] - 2018-06-23

- Fix an issue with package structure that was preventing Burst to work in Unity.

## [0.2.4-preview.13] - 2018-06-22

- Add support for Burst timings menu.
- Improve codegen for sin/cos.
- Improve codegen when using swizzles on vector types.
- Add support for sincos intrinsic.
- Fix AOT deployment.

## [0.2.4-preview.12] - 2018-06-13

- Fix a bug in codegen that was collapsing methods overload of System.Threading.Interlocked to the same method.

## [0.2.4-preview.11] - 2018-06-05

- Fix exception in codegen when accessing readonly static fields from different control flow paths.

## [0.2.4-preview.10] - 2018-06-04

- Fix a potential stack overflow issue when a generic parameter constraint on a type is also referencing another generic parameter through a generic interface constraint
- Update to latest Unity.Mathematics:
  - Fix order of parameters and codegen for step functions.

## [0.2.4-preview.9] - 2018-05-29

- Fix bug when casting an IntPtr to an enum pointer that was causing an invalid codegen exception.

## [0.2.4-preview.8] - 2018-05-24

- Breaking change: Move Unity.Jobs.Accuracy/Support to Unity.Burst.
- Deprecate ComputeJobOptimizationAttribute in favor of BurstCompileAttribute.
- Fix bug when using enum with a different type than int.
- Fix bug with IL stind that could lead to a memory corruption.

## [0.2.4-preview.7] - 2018-05-22

- Add support for nested structs in SOA native arrays.
- Add support for arbitrary sized elements in full SOA native arrays.
- Fix bug with conversion from signed/unsigned integers to signed numbers (integers & floats).
- Add support for substracting pointers at IL level.
- Improve codegen with pointers arithmetic to avoid checking for overflows.

## [0.2.4-preview.6] - 2018-05-11

- Remove `bool1` from mathematics and add proper support in Burst.
- Add support for ARM platforms in the Burst inspector UI.

## [0.2.4-preview.5] - 2018-05-09

- Add support for readonly static fields.
- Add support for stackalloc.
- Fix potential crash on MacOSX when using memset is used indirectly.
- Fix crash when trying to write to a bool1*.
- Fix bug with EnableBurstCompilation checkbox not working in Unity Editor.

## [0.2.4-preview.4] - 2018-05-03

- Fix an issue on Windows with `DllNotFoundException` occurring when trying to load `burst-llvm.dll` from a user profile containing unicode characters in the folder path.
- Fix an internal compiler error occurring with IL dup instruction.

## [0.2.4-preview.3] - 2018-05-03

- Add support for struct with an explicit layout.
- Fix noalias regression (that was preventing the auto-vectorizer to work correctly on basic loops).

## 0.2.3 (21 March 2018)

- Improve error messages for static field access.
- Improve collecting of compilable job by trying to collect concrete job type instances (issue #23).

## 0.2.2 (19 March 2018)

- Improve error messages in case using `is` or `as` cast in C#.
- Improve error messages if a static delegate instance is used.
- Fix codegen error when converting a byte/ushort to a float.
