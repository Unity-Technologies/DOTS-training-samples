using System;
using System.ComponentModel;

namespace Unity.Entities
{
#if UNITY_SKIP_UPDATES_WITH_VALIDATION_SUITE
    [Obsolete("SceneData has been renamed to SceneSectionData. If you see this message in a user project, remove the UNITY_SKIP_UPDATES_WITH_VALIDATION_SUITE define from the Entities assembly definition file. (RemovedAfter 2019-10-6).", true)]
#else
    [Obsolete("SceneData has been renamed to SceneSectionData. (RemovedAfter 2019-10-6) (UnityUpgradable) -> SceneSectionData", true)]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct SceneData
    {
        public Hash128 SceneGUID;
        public int SubSectionIndex;
        public int FileSize;
        public int SharedComponentCount;
        public Mathematics.MinMaxAABB BoundingVolume;
    }

}
