using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : SystemBase 
{
    protected override void OnUpdate()
    {
        var pathBuffers = this.GetBufferFromEntity<PathNode>();
        var settingsEntity = GetSingletonEntity<Settings>();
        var settings = GetComponentDataFromEntity<Settings>()[settingsEntity];

        Entities.WithAll<Farmer>()
           .ForEach((Entity entity, ref Velocity speed, in Translation translation) =>
           {
               var pathNodes = pathBuffers[entity];
               
               var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                   (int)math.floor(translation.Value.z));

               var targetPosition = farmerPosition;
               if (Input.GetKeyDown(KeyCode.UpArrow)) targetPosition.y += 1;
               if (Input.GetKeyDown(KeyCode.DownArrow)) targetPosition.y -= 1;
               if (Input.GetKeyDown(KeyCode.RightArrow)) targetPosition.x += 1;
               if (Input.GetKeyDown(KeyCode.LeftArrow)) targetPosition.x -= 1;

               targetPosition = math.clamp(targetPosition, 0, settings.GridSize);

               if (math.length(farmerPosition) != math.length(targetPosition)) 
                   pathNodes.Insert(0,new PathNode { Value = targetPosition });
           }).Run();
    }
}