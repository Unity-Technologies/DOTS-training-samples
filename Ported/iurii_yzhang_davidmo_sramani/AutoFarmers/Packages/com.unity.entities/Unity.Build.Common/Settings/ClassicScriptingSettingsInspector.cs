using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Properties.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Build.Common
{
    [UsedImplicitly]
    sealed class ClassicScriptingSettingsInspector : IInspector<ClassicScriptingSettings>
    {
        EnumField m_ScriptingBackend;
        VisualElement m_Il2CppCompilerConfiguration;

        public VisualElement Build(InspectorContext<ClassicScriptingSettings> context)
        {
            var root = new VisualElement();
            context.DoDefaultGui(root, nameof(ClassicScriptingSettings.ScriptingBackend)); 
            context.DoDefaultGui(root, nameof(ClassicScriptingSettings.Il2CppCompilerConfiguration));
            context.DoDefaultGui(root, nameof(ClassicScriptingSettings.UseIncrementalGC));

            m_ScriptingBackend = root.Q<EnumField>(nameof(ClassicScriptingSettings.ScriptingBackend));
            m_Il2CppCompilerConfiguration = root.Q<VisualElement>(nameof(ClassicScriptingSettings.Il2CppCompilerConfiguration));

            return root;
        }

        public void Update(InspectorContext<ClassicScriptingSettings> context)
        {
            m_Il2CppCompilerConfiguration.SetEnabled((ScriptingImplementation)m_ScriptingBackend.value == ScriptingImplementation.IL2CPP);
        }
    }
}