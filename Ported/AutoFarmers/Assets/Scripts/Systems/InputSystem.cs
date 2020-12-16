using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : SystemBase 
{
    protected override void OnUpdate()
    {
        var pathBuffers = this.GetBufferFromEntity<PathNode>();

        
        Entities.WithAll<Farmer>()
           .ForEach((Entity entity, ref Velocity speed, ref Path path, in Translation translation) =>
           {
               var pathNodes = pathBuffers[entity];
               
               var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                   (int)math.floor(translation.Value.z));

               var targetPosition = farmerPosition;
               if (Input.GetKeyDown(KeyCode.UpArrow)) targetPosition.y += 1;
               if (Input.GetKeyDown(KeyCode.DownArrow)) targetPosition.y -= 1;
               if (Input.GetKeyDown(KeyCode.RightArrow)) targetPosition.x += 1;
               if (Input.GetKeyDown(KeyCode.LeftArrow)) targetPosition.x -= 1;

               if (math.length(farmerPosition) != math.length(targetPosition)) 
                   pathNodes.Add(new PathNode { Value = targetPosition });
           }).Run();
    }
}