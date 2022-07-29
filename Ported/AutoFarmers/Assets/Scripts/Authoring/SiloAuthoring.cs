using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class SiloAuthoring : MonoBehaviour
{
    public GameObject SiloPrefab;
    public int NumberSilos;
    public int resources;
}
class SiloBaker : Baker<SiloAuthoring>
{
    public override void Bake(SiloAuthoring authoring)
    {
        AddComponent(new SiloConfig()
        {
            SiloPrefab = GetEntity(authoring.SiloPrefab),
            NumberSilos = authoring.NumberSilos,
            resources = authoring.resources,
        });
    }
}