using Unity.Entities;
using Unity.Transforms;
using  Unity.Mathematics;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


public class BoardInitSystem : SystemBase
{
   protected override void OnUpdate()
   {
      //There should be at least 3 entities
      Entities.WithStructuralChanges().ForEach((Entity entity, 
          ref Arc arc, in WallAuthoring wall, in LocalToWorld ltw) =>
      {
         float deg2rad = (math.PI * 2) / 360;
         float radius = arc.Radius;
         float minRingWidth = 45; //temp

         Random random = new Random(502); 

          arc.StartAngle = random.NextFloat(0, 180);
          arc.EndAngle = random.NextFloat(arc.StartAngle + minRingWidth, 360);
         
          for (int i = (int)arc.StartAngle; i < (arc.EndAngle + 1); i++) 
          {
               float rad = deg2rad * i;
               float3 position = new float3(ltw.Position.x + (math.sin(rad) * radius), 0, 
                  ltw.Position.z + (math.cos(rad) * radius));
            
               //instantiate prefabs with mesh render
               var instance = EntityManager.Instantiate(wall.wallPrefab);
               SetComponent(instance, new Translation{ Value = position });
            
          }
            
         //Only run this once
         EntityManager.RemoveComponent<WallAuthoring>(entity);
         
      }).WithoutBurst().Run();

   }
   
   
   
}
