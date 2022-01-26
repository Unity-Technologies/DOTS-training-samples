using Unity.Entities;
using Unity.Profiling;
using Unity.Rendering;
using UnityEngine;

public partial class PheromoneTextureUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, in Ground ground) =>
        {
            var texture = ground.Texture;
            var data = texture.GetRawTextureData<Color32>();

            var value = (byte)((int)(Time.ElapsedTime * 10) % 255);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color32(value, 0, 0, 255);
            }

            texture.Apply();
        }).WithoutBurst().Run();
    }
}
