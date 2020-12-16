using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DestroyRocks : SystemBase
{
    protected override void OnUpdate()
    {
        var pathBuffers = this.GetBufferFromEntity<PathNode>();
        
        /*
        Entities
            .WithAll<Farmer>()
            .WithAll<DestroyRockIntention>()
            .ForEach((Entity entity) =>
            {
                var path = pathBuffers[entity];

                if (path.Length == 0)
                {
                    path.Clear();
                    path.Add(new PathNode { Value = new int2(0, 0)});
                    path.Add(new PathNode { Value = new int2(1, 1)});
                    path.Add(new PathNode { Value = new int2(2, 2)});
                    path.Add(new PathNode { Value = new int2(3, 3)});
                    path.Add(new PathNode { Value = new int2(4, 4)});
                    path.Add(new PathNode { Value = new int2(5, 5)});   
                }
            }).Run();
            */
    }
}