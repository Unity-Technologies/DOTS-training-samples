using Unity.Entities;
using Unity.Collections;

// An empty component is called a "tag component".
struct PlatformTag : IComponentData
{
}

struct PlatformWalkways : IComponentData
{
    public NativeArray<WalkwayInfo> Walkways;
}

struct PlatformStaticInfo : IComponentData
{
    //public NativeArray<PlatformInfoU> TaskList;
}

struct PlatformDynamicInfo : IComponentData
{
}
