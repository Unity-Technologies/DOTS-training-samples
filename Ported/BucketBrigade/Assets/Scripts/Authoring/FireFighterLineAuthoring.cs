using Unity.Entities;
using Unity.Rendering;

class FireFighterLineAuthoring : UnityEngine.MonoBehaviour
{
}

class FireFighterLineBaker : Baker<FireFighterLineAuthoring>
{
    public override void Bake(FireFighterLineAuthoring authoring)
    {
    }
}