using Unity.Entities;
using Unity.Rendering;

//@rename CannonBallAuthoring_Step1 CannonBallAuthoring
//@rename CannonBallBaker_Step1 CannonBallBaker

#region step1
class CannonBallAuthoring_Step1 : UnityEngine.MonoBehaviour
{
}

class CannonBallBaker_Step1 : Baker<CannonBallAuthoring_Step1>
{
    public override void Bake(CannonBallAuthoring_Step1 authoring)
    {
        // By default, components are zero-initialized.
        // So in this case, the Speed field in CannonBall will be float3.zero.
        AddComponent<CannonBall>();
    }
}
#endregion

#region step2
class CannonBallAuthoring : UnityEngine.MonoBehaviour
{
}

class CannonBallBaker : Baker<CannonBallAuthoring>
{
    public override void Bake(CannonBallAuthoring authoring)
    {
        AddComponent<CannonBall>();
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}
#endregion
