using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : SystemBase 
{
    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var pathBuffers = this.GetBufferFromEntity<PathNode>();
        
        Entities.WithAll<Farmer>()
           .ForEach((Entity entity, ref Velocity speed, in Translation translation) =>
           {
               var pathNodes = pathBuffers[entity];

               var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                   (int)math.floor(translation.Value.z));

               if (pathNodes.Length == 0)
                   Pathing.FindNearbyRock(pathNodes, tileBuffer, farmerPosition, 100, settings);
               /*var targetPosition = farmerPosition;
               if (Input.GetKeyDown(KeyCode.UpArrow)) targetPosition.y += 1;
               if (Input.GetKeyDown(KeyCode.DownArrow)) targetPosition.y -= 1;
               if (Input.GetKeyDown(KeyCode.RightArrow)) targetPosition.x += 1;
               if (Input.GetKeyDown(KeyCode.LeftArrow)) targetPosition.x -= 1;

               targetPosition = math.clamp(targetPosition, 0, settings.GridSize);

               if (math.length(farmerPosition) != math.length(targetPosition)) 
                   pathNodes.Insert(0,new PathNode { Value = targetPosition });*/
           }).Run();
    }
}