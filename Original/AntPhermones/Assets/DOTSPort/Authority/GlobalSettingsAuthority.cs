using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

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
                MapSizeY = authoring.MapSizeY
            });
        }
    }
}

public struct GlobalSettings: IComponentData
{
    public int MapSizeX;
    public int MapSizeY;
}
