using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Wrapper type to display the <see cref="Entity"/> inspector header.
    /// </summary>
    readonly struct EntityHeader
    {
        readonly EntityInspectorContext m_Context;

        public EntityHeader(EntityInspectorContext context)
        {
            m_Context = context;
        }

        [CreateProperty, UsedImplicitly]
        public string Name
        {
            get => m_Context.GetTargetName();
            set => m_Context.SetTargetName(value);
        }

        [CreateProperty, UsedImplicitly] public int Index => m_Context.Entity.Index;
        [CreateProperty, UsedImplicitly] public int Version => m_Context.Entity.Version;
        [CreateProperty, UsedImplicitly] public GameObject ConvertedFrom => m_Context.GetOriginatingGameObject();
    }
}
