using Unity.Entities;
using UnityEngine;

namespace Unity.Scenes.Editor
{
    [ExecuteAlways]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SceneSystem))]
    class LiveLinkEditorSystemGroup : ComponentSystemGroup
    {
    }
}