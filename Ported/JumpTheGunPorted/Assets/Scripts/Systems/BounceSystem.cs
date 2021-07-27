using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BounceSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, ref ParabolaTValue tValue, in Translation translation, in Player playerTag) =>
            {
                if (tValue.Value < 0)
                {
                    // solving parabola path
                    //startBox endBox // TODO: player aim logic using mouse input 
                    float startY = translation.Value.y; // startBox.Y + Player.Y_OFFSET; // TODO: get current height of player tile vs player Y value
                    float endY = translation.Value.y; //endBox.Y + Player.Y_OFFSET; // TODO: get current height of player destination tile vs. player Y value
                    float height = math.max(startY, endY);

                    // make height max of adjacent boxes when moving diagonally
                    // TODO:
                    /*if (startBox.col != endBox.col && startBox.row != endBox.row)
                    {
                        height = Mathf.Max(height, TerrainArea.instance.GetBox(startBox.col, endBox.row).top, TerrainArea.instance.GetBox(endBox.col, startBox.row).top);
                    }*/
                    height += Player.BOUNCE_HEIGHT;

                    JumpTheGun.Parabola.Create(startY, height, endY, out float a, out float b, out float c);

                    // TODO: duration affected by distance to end box
                    //Vector2 startPos = new Vector2(startBox.col, startBox.row);
                    //Vector2 endPos = new Vector2(endBox.col, endBox.row);
                    //float dist = Vector2.Distance(startPos, endPos);
                    float dist = 1.0f;
                    float duration = math.max(1, dist) * Player.BOUNCE_BASE_DURATION;

                    ecb.AddComponent(entity, new Parabola
                    {
                        StartY = startY,
                        Height = height,
                        EndY = endY,
                        A = a,
                        B = b,
                        C = c,
                        Duration = duration
                    });

                    tValue.Value = 0;
                }
            }).Run(); // TODO: can I make this a job or parallel? errors right now doing that, figure out later

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}