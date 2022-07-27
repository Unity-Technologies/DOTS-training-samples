using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

class FireFighterLineAuthoring : UnityEngine.MonoBehaviour
{
}

class FireFighterLineBaker : Baker<FireFighterLineAuthoring>
{
    public override void Bake(FireFighterLineAuthoring authoring)
    {
        AddComponent(new FireFighterLine
        {

            LineId = 0,
            StartPosition = new float2(0, 0),
            EndPosition = new float2(0, 0)
        });
    }
}