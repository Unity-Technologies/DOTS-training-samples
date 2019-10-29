using System.Collections.Generic;
using System.IO;
using UnityEditor;
using PropertyAttribute = Unity.Properties.PropertyAttribute;

namespace Unity.Build.Common
{
    public sealed class ClassicScriptingSettings : IBuildSettingsComponent
    {
        [Property]
        public ScriptingImplementation ScriptingBackend { get; set; } = ScriptingImplementation.Mono2x;

        [Property]
        public Il2CppCompilerConfiguration Il2CppCompilerConfiguration { get; set; } = Il2CppCompilerConfiguration.Release;

        [Property]
        public bool UseIncrementalGC { get; set; } = false;

        // Note: We haven't exposed ScriptingDefineSymbols, ApiCompatibilityLevel, AllowUnsafeCode. Because those affect scripting compilation pipeline, this raises few questions:
        //       - Editor will not reflect the same set compilation result as building to player, which is not very good.
        //       - We need to either decide to have somekind of global project settings (used both in Editor and while building to player) or have
        //         "active build settings" property which would be used as information what kind of enviromnent to simulate, in this it may make sense to but 'ScriptingDefineSymbols, ApiCompatibilityLevel, AllowUnsafeCode' here
    }
}
