using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ClickSpawnSystem : SystemBase
{
    float m_SpawnOffset = 2;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SpawnZones>();
    }

    protected override void OnUpdate()
    {
        var zones = GetSingleton<SpawnZones>();
        if ( Input.GetMouseButton(0)){ 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if ( Physics.Raycast (ray, out var hit, 100.0f)) {
                var newFood = EntityManager.Instantiate(zones.FoodPrefab);
                EntityManager.SetComponentData(newFood, new Translation
                {
                    Value = hit.point + hit.normal * m_SpawnOffset,
                });
            }
        } 
    }
}
