using Unity.Entities;
using UnityEngine;

class RockAuthoring : MonoBehaviour 
{

}

class RockBaker : Baker<RockAuthoring> 
{
    public override void Bake(RockAuthoring authoring)
    {
        AddComponent<Rock>();
    }
}