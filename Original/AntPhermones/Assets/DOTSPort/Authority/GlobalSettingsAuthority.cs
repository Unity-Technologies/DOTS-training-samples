using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GlobalSettingsAuthority : MonoBehaviour
{
    public int MapSizeX;
    public int MapSizeY;
    public int FoodBufferSize;
    public float AntRandomSteering;
    public float AntSpeed;
    public float AntAccel;

    class Baker : Baker<GlobalSettingsAuthority>
    {
        public override void Bake(GlobalSettingsAuthority authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GlobalSettings()
            {
                MapSizeX = authoring.MapSizeX,
                MapSizeY = authoring.MapSizeY,
                FoodBufferSize = authoring.FoodBufferSize,
                AntRandomSteering = authoring.AntRandomSteering,
                AntSpeed = authoring.AntSpeed,
                AntAccel = authoring.AntAccel,
            });
        }
    }
}

public struct GlobalSettings: IComponentData
{
    public int MapSizeX;
    public int MapSizeY;
    public int FoodBufferSize;
    public float AntRandomSteering;
    public float AntSpeed;
    public float AntAccel;
}
