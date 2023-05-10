using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GlobalSettingsAuthority : MonoBehaviour
{
    public int MapSizeX;
    public int MapSizeY;
    
    class Baker : Baker<GlobalSettingsAuthority>
    {
        public override void Bake(GlobalSettingsAuthority authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GlobalSettings()
            {
                MapSizeX = authoring.MapSizeX,
                MapSizeY = authoring.MapSizeY,
                AntRandomSteering = math.PI / 4f,
                AntSpeed = 0.2f,
                AntAccel = 0.07f
    });
        }
    }
}

public struct GlobalSettings: IComponentData
{
    public int MapSizeX;
    public int MapSizeY;
    public float AntRandomSteering;
    public float AntSpeed;
    public float AntAccel;
}
