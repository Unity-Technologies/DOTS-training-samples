using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GlobalSettingsAuthority : MonoBehaviour
{
    public int MapSizeX = 128;
    public int MapSizeY = 128;
	public int FoodBufferSize = 10;
    public float FoodRadius = 1f;
    public float AntRandomSteering = math.PI / 4;
    public float AntSpeed = 0.2f;
    public float AntAccel = 0.07f;
    public float AntGoalSteerStrength = 0.04f;
    public float TrailAddSpeed = 0.5f;
    public float TrailDecay = 0.01f;
    public Color ExitedColor = Color.yellow;
    public Color RegularColor = Color.white;

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
                FoodRadius = authoring.FoodRadius,
                AntRandomSteering = authoring.AntRandomSteering,
                AntSpeed = authoring.AntSpeed,
                AntAccel = authoring.AntAccel,
                AntGoalSteerStrength = authoring.AntGoalSteerStrength,
                TrailAddSpeed = authoring.TrailAddSpeed,
                TrailDecay = authoring.TrailDecay,
                ExitedColor = authoring.ExitedColor,
                RegularColor = authoring.RegularColor
            });
        }
    }
}

public struct GlobalSettings: IComponentData
{
    public int MapSizeX;
    public int MapSizeY;
    public int FoodBufferSize;
    public float FoodRadius;
    public float AntRandomSteering;
    public float AntSpeed;
    public float AntAccel;
    public float AntGoalSteerStrength;
    public float TrailAddSpeed;
    public float TrailDecay;
    public Vector4 ExitedColor;
    public Vector4 RegularColor;
}
