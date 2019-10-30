#if UNITY_2019_3_OR_NEWER
namespace Unity.Burst
{
        /// <summary>
        /// Represents the types of compiled code that are run on the current thread.
        /// </summary>
        public enum BurstExecutionEnvironment
        {
            /// <summary>
            /// Use the default (aka FloatMode specified via Compile Attribute - <see cref="FloatMode"/>
            /// </summary>
            Default=0,
            /// <summary>
            /// Override the specified float mode and run the non deterministic version
            /// </summary>
            NonDeterministic=0,
            /// <summary>
            /// Override the specified float mode and run the deterministic version
            /// </summary>
            Deterministic=1,
        }

}
#endif