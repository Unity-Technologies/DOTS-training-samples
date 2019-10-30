# Unity.Mathematics

A C# math library providing vector types and math functions with a shader like
syntax. Used by the Burst compiler to compile C#/IL to highly efficient
native code.

The main goal of this library is to provide a friendly Math API familiar to SIMD and graphic/shaders developers, using the well known `float4`, `float3` types...etc. with all intrinsics functions provided by a static class `math` that can be imported easily into your C# program with `using static Unity.Mathematics.math`.

In addition to this, the Burst compiler is able to recognize these types and provide the optimized SIMD type for the running CPU on all supported platforms (x64, ARMv7a...etc.)

NOTICE: The API is a work in progress and we may introduce breaking changes (API and underlying behavior)

## Usage

You can use this library in your Unity game by using the Package Manager and referencing the package `com.unity.mathematics`. See the forum [Welcome](https://forum.unity.com/threads/welcome.522627) page for more details.

```C#
using static Unity.Mathematics.math;
namespace MyNamespace
{
    using Unity.Mathematics;
    
    ...
    var v1 = float3(1,2,3);
    var v2 = float3(4,5,6);
    v1 = normalize(v1);
    v2 = normalize(v2);
    var v3 = dot(v1, v2);
    ...
}
```

## Building

Open the `src\Unity.Mathematics.sln` under Visual Studio 2015 or MonoDevelop and compile in Debug\Release.

## Contributing

We don't yet accept PR on this repository. See the FAQ below.

The project is using [editorconfig](http://editorconfig.org/) to keep files correctly formatted for EOL and spaces.

We assume that your IDE has support for `editorconfig`, you can download the following extensions if your IDE is listed:

- [VS2015/VS2017 EditorConfig extension](https://marketplace.visualstudio.com/items?itemName=EditorConfigTeam.EditorConfig)
- [Visual Studio Code EditorConfig extension](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)
- [SublimeText EditorConfig extension](https://github.com/sindresorhus/editorconfig-sublime)

## Frequently Asked Question

### Why developing another Math library instead of using existing Unity Vector3...etc.?

After years of feedback and experience with the previous API, we believe that providing an API that is closer to the way graphics developers have been using math libraries should better help its adoption and the ease of its usage. HLSL / GLSL math library is a very well designed, well understood math library leading to greater consistency.

### Why not using `System.Numerics.Vectors`?

Mainly for the reason mentioned above, `System.Numerics.Vectors` is in many ways similar to our previous Vector library (more object oriented than graphics programming oriented).
Also the fact that our Burst compiler is able to recognize a lot more patterns for SIMD types and math intrinsics makes it easier to work with a dedicated API that reflects this ability.

### Naming convention

In C# `int` and `float` are considered builtin types. Burst extends this set of bultin types to also include vectors, matrices and quaternions. These types are bultin in the sense that Burst knows about them and is be able to generate better code using these types than what would be possible with equivalent code using custom types.

To signify that these types are bultin their type names are in all lower case. The operators on these bultin types found in `Unity.Mathematics.math` are considered intrinsics and are thus always in lower case.

There are no plans to extend the set of intrinsic types beyond the current set of vectors (`typeN`), matrices (`typeNxN`) and quaternions (`quaternion`).

This convention has the added benefit of making the library highly compatible with shader code and makes porting or sharing code between the two almost frictionless.

### Why can't we send a PR yet?

We are working on providing a Contributor License Agreement (CLA) with a sign-over functionality and our UCL License doesn't cover this yet.

## Licensing

Unity Companion License (“License”) Software Copyright © 2019 Unity Technologies ApS

For licensing details see [LICENSE.md](LICENSE.md)

