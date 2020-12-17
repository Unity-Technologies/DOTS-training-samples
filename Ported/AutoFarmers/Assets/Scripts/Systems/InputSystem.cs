using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PathMovement))]
public class InputSystem : SystemBase 
{
    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var pathBuffers = this.GetBufferFromEntity<PathNode>();
        var pathMovement = World.GetExistingSystem<PathMovement>();

        Entities.WithAll<Farmer>()
           .ForEach((Entity entity, ref Velocity speed, in Translation translation) =>
           {
               var pathNodes = pathBuffers[entity];

               var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                   (int)math.floor(translation.Value.z));

               if (pathNodes.Length == 0)
               {
                  var result = pathMovement.FindNearbyRock(farmerPosition.x, farmerPosition.y, 3600, tileBuffer, pathNodes);
               }
           }).WithoutBurst().Run();
    }
}