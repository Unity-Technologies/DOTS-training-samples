using Unity.Entities;
using UnityEngine;

public class NeedUpdateTrajectoryAuthoring : MonoBehaviour
{
}

public class NeedUpdateTrajectoryBaker : Baker<NeedUpdateTrajectoryAuthoring>
{
    public override void Bake(NeedUpdateTrajectoryAuthoring authoring)
    {
        AddComponent<NeedUpdateTrajectory>();
    }
}
