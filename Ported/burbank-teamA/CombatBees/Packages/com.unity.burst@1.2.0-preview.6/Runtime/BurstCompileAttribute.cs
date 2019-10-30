using System;
using System.Runtime.CompilerServices;

// Make internals visible to Unity.Burst.Editor for BurstGlobalCompilerOptions
[assembly: InternalsVisibleTo("Unity.Burst.Editor")]
// Make internals visible to burst tests
[assembly: InternalsVisibleTo("btests")]
// Make internals visible to Unity burst tests
[assembly: InternalsVisibleTo("Unity.Burst.Tests.UnitTests")]
[assembly: InternalsVisibleTo("Unity.Burst.Editor.Tests")]
namespace Unity.Burst
{
    // FloatMode and FloatPrecision must be kept in sync with burst.h / Burst.Backend

    /// <summary>
    /// Represents the floating point optimization mode for compilation.
    /// </summary>
    public enum FloatMode
    {
        /// <summary>
        /// Use the default target floating point mode - <see cref="FloatMode.Strict"/>.
        /// </summary>
        Default = 0,
        /// <summary>
        /// No floating point optimizations are performed.
        /// </summary>
        Strict = 1,
        /// <summary>
        /// Reserved for future.
        /// </summary>
        Deterministic = 2,
        /// <summary>
        /// Allows algebraically equivalent optimizations (which can alter the results of calculations), it implies :
        /// <para/> optimizations can assume results and arguments contain no NaNs or +/- Infinity and treat sign of zero as insignificant.
        /// <para/> optimizations can use reciprocals - 1/x * y  , instead of  y/x.
        /// <para/> optimizations can use fused instructions, e.g. madd.
        /// </summary>
        Fast = 3,
    }

    /// <summary>
    /// Represents the floating point precision used for certain builtin operations e.g. sin/cos.
    /// </summary>
    public enum FloatPrecision
    {
        /// <summary>
        /// Use the default target floating point precision - <see cref="FloatPrecision.Medium"/>.
        /// </summary>
        Standard = 0,
        /// <summary>
        /// Compute with an accuracy of 1 ULP - highly accurate, but increased runtime as a result, should not be required for most purposes.
        /// </summary>
        High = 1,
        /// <summary>
        /// Compute with an accuracy of 3.5 ULP - considered acceptable accuracy for most tasks.
        /// </summary>
        Medium = 2,
        /// <summary>
        /// Reserved for future.
        /// </summary>
        Low = 3,
    }

    /// <summary>
    /// This attribute can be used to tag jobs as being Burst Compiled, and optionally set some compilation parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct|AttributeTargets.Method)]
    public class BurstCompileAttribute : System.Attribute
    {
        /// <summary>
        /// Gets or sets the float mode of operation for this burst compilation.
        /// </summary>
        /// <value>
        /// The default is <see cref="FloatMode.Default"/>.
        /// </value>
        public FloatMode FloatMode { get; set; }

        /// <summary>
        /// Gets or sets the floating point precision to use for this burst compilation.
        /// Allows you to trade accuracy for speed of computation, useful when you don't require much precision.
        /// </summary>
        /// <value>
        /// The default is <see cref="FloatPrecision.Standard"/>.
        /// </value>
        public FloatPrecision FloatPrecision { get; set; }

        /// <summary>
        /// Gets or sets whether or not to burst compile the code immediately on first use, or in the background over time.
        /// </summary>
        /// <value>The default is false, true will force this code to be compiled synchronously on first invocation.</value>
        public bool CompileSynchronously { get; set; }

        /// <summary>
        /// Gets or sets whether to compile the code in a way that allows it to be debugged.
        /// If this is set to <code>true</code>, the current implementation skips compilation of this method
        /// so that the original .NET method can be debugged.
        /// In the current implementation, this property is only used in the Editor and ignored for standalone players.
        /// </summary>
        /// <value>
        /// The default is <code>false</code>.
        /// </value>
        public bool Debug { get; set; }

        internal string[] Options { get; set; }

        /// <summary>
        /// Tags a struct/method/class as being burst compiled, with the default <see cref="FloatPrecision"/>, <see cref="FloatMode"/> and <see cref="CompileSynchronously"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// [BurstCompile]
        /// struct MyMethodsAreCompiledByBurstUsingTheDefaultSettings
        /// {
        ///     //....
        /// }
        ///</code>
        /// </example>
        public BurstCompileAttribute()
        {
        }

        /// <summary>
        /// Tags a struct/method/class as being burst compiled, with the specified <see cref="FloatPrecision"/>, <see cref="FloatMode"/> and the default <see cref="CompileSynchronously"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// [BurstCompile(FloatPrecision.Low,FloatMode.Fast)]
        /// struct MyMethodsAreCompiledByBurstWithLowPrecisionAndFastFloatingPointMode
        /// {
        ///     //....
        /// }
        ///</code>
        ///</example>
        /// <param name="floatPrecision">Specify the required floating point precision.</param>
        /// <param name="floatMode">Specify the required floating point mode.</param>
        public BurstCompileAttribute(FloatPrecision floatPrecision, FloatMode floatMode)
        {
            FloatMode = floatMode;
            FloatPrecision = floatPrecision;
        }
    }
}
