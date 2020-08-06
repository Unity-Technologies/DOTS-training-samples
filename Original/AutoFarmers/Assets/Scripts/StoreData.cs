using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct StoreData_Spawner:IComponentData
{
    public Entity prefab;
    public int count;
    public int mapX;
    public int mapY;
}


public struct Store:IComponentData
{
    public int nbPlantsSold; //per frame / gets reset every frame and added to global economy data
    public float2 position;
}
