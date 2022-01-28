﻿using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using Unity.Mathematics;

[UpdateAfter(typeof(DecayPheromone))]
public partial class PheromoneTextureUpdateSystem : SystemBase
{

    protected override void OnUpdate()
    {
        var entity = GetSingletonEntity<Ground>();
        var gridEntity = GetSingletonEntity<Grid2D>();
        var pheromone = GetBuffer<Pheromone>(gridEntity);
        var obstacles = GetBuffer<ObstaclePositionAndRadius>(gridEntity);
        var grid = GetComponent<Grid2D>(gridEntity);

        var renderer = EntityManager.GetSharedComponentData<RenderMesh>(entity);
        var texture = renderer.material.mainTexture as Texture2D;
        var data = texture.GetRawTextureData<Color32>();

        Job.WithCode(() =>
        {
            for (int i = 0; i < grid.columnLength; ++i)
            {
                for (int j = 0; j < grid.rowLength; ++j)
                {
                    var index = j + i * grid.rowLength;
                    //var indexTex = i + j * grid.columnLength;
                    data[index] = new Color(math.clamp(pheromone[index].Value, 0, 1), obstacles[index].IsValid ? obstacles[index].position.x + 0.5f : 0f, obstacles[index].IsValid ? obstacles[index].position.y : 0f, 1f);
                }
            }
        }).Run();

        texture.Apply();
    }
}