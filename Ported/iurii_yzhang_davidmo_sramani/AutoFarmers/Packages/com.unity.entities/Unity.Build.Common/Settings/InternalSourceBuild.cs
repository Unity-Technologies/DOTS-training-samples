using System.Collections.Generic;
using System.IO;
using UnityEditor;
using PropertyAttribute = Unity.Properties.PropertyAttribute;

namespace Unity.Build.Common
{
    internal sealed class InternalSourceBuildConfiguration : IBuildSettingsComponent
    {
        [Property]
        public bool Enabled { get; set; } = false;
    }
}
