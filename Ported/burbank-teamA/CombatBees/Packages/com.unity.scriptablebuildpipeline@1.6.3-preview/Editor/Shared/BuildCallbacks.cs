using System;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Basic implementation of IDependencyCallback, IPackingCallback, IWritingCallback, & IScriptsCallback.
    /// Uses Func implementation for callbacks. <seealso cref="IDependencyCallback"/>, <seealso cref="IPackingCallback"/>
    /// <seealso cref="IWritingCallback"/>, and <seealso cref="IScriptsCallback"/>
    /// </summary>
    public class BuildCallbacks : IDependencyCallback, IPackingCallback, IWritingCallback, IScriptsCallback
    {
        /// <summary>
        /// Func delegate for the callback after scripts have been compiled.
        /// </summary>
        public Func<IBuildParameters, IBuildResults, ReturnCode> PostScriptsCallbacks { get; set; }

        /// <summary>
        /// Func delegate for the callback after dependency calculation has occurred.
        /// </summary>
        public Func<IBuildParameters, IDependencyData, ReturnCode> PostDependencyCallback { get; set; }

        /// <summary>
        /// Func delegate for the callback after packing has occurred.
        /// </summary>
        public Func<IBuildParameters, IDependencyData, IWriteData, ReturnCode> PostPackingCallback { get; set; }

        /// <summary>
        /// Func delegate for the callback after writing content has occurred.
        /// </summary>
        public Func<IBuildParameters, IDependencyData, IWriteData, IBuildResults, ReturnCode> PostWritingCallback { get; set; }

        /// <inheritdoc />
        public ReturnCode PostScripts(IBuildParameters parameters, IBuildResults results)
        {
            if (PostScriptsCallbacks != null)
                return PostScriptsCallbacks(parameters, results);
            return ReturnCode.Success;
        }

        /// <inheritdoc />
        public ReturnCode PostDependency(IBuildParameters buildParameters, IDependencyData dependencyData)
        {
            if (PostDependencyCallback != null)
                return PostDependencyCallback(buildParameters, dependencyData);
            return ReturnCode.Success;
        }

        /// <inheritdoc />
        public ReturnCode PostPacking(IBuildParameters buildParameters, IDependencyData dependencyData, IWriteData writeData)
        {
            if (PostPackingCallback != null)
                return PostPackingCallback(buildParameters, dependencyData, writeData);
            return ReturnCode.Success;
        }

        /// <inheritdoc />
        public ReturnCode PostWriting(IBuildParameters parameters, IDependencyData dependencyData, IWriteData writeData, IBuildResults results)
        {
            if (PostWritingCallback != null)
                return PostWritingCallback(parameters, dependencyData, writeData, results);
            return ReturnCode.Success;
        }
    }
}
