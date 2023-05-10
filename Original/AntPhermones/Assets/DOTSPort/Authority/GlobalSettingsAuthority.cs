using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GlobalSettingsAuthority : MonoBehaviour
{
    public int MapSizeX = 128;
    public int MapSizeY = 128;
    public float AntSpeed = 0.2f;
    public float AntAccel = 0.07f;
    public float TrailAddSpeed = 0.5f;
    public float TrailDecay = 0.01f;

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
                AntSpeed = authoring.AntSpeed,
                AntAccel = authoring.AntAccel,
                TrailAddSpeed = authoring.TrailAddSpeed,
                TrailDecay = authoring.TrailDecay
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
    public float TrailAddSpeed;
    public float TrailDecay;
}
