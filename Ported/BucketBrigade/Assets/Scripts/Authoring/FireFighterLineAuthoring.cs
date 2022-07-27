using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

class FireFighterLineAuthoring : UnityEngine.MonoBehaviour
{
    public int LineId;
    public float2 StartPosition;
    public float2 EndPosition;
}

class FireFighterLineBaker : Baker<FireFighterLineAuthoring>
{
    public override void Bake(FireFighterLineAuthoring authoring)
    {
        AddComponent(new FireFighterLine
        {
            LineId = authoring.LineId,
            StartPosition = authoring.StartPosition,
            EndPosition = authoring.EndPosition
        });
    }
}