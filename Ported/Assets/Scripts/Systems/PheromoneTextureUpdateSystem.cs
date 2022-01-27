using Unity.Entities;
using UnityEngine;

public partial class PheromoneTextureUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, in Ground ground) =>
        {
            var gridEntity = GetSingletonEntity<Grid2D>();
            var grid = GetComponent<Grid2D>(gridEntity);
            var obstacles = GetBuffer<ObstaclePositionAndRadius>(gridEntity);
            var texture = ground.Texture;
            var data = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < grid.columnLength; ++i)
            {
                for (int j = 0; j < grid.rowLength; ++j)
                {
                    var index = j + i * grid.rowLength;
                    var indexTex = i + j * grid.columnLength;
                    data[indexTex] = obstacles[index].IsValid ? new Color(0, obstacles[index].position.x + 0.5f, obstacles[index].position.y + 0.5f, 1f) : Color.black;
                }
            }

            texture.Apply();
        }).WithoutBurst().Run();
    }
}
