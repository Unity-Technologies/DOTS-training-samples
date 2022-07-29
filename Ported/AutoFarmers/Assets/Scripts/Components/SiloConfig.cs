using Unity.Entities;
using Unity.Mathematics;

struct SiloConfig : IComponentData
{
    public Entity SiloPrefab;
    public int NumberSilos;
    public int resources;
    

}