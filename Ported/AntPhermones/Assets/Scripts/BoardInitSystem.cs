using Unity.Entities;
using Unity.Transforms;
using  Unity.Mathematics;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Random = Unity.Mathematics.Random;


public class BoardInitSystem : SystemBase
{
   protected override void OnUpdate()
   {  
    
      //There should be at least 3 entities
      Entities.WithStructuralChanges().ForEach((Entity entity, 
          ref Arc arc, in WallAuthoring wall, in LocalToWorld ltw) =>
      {
         float deg2rad = (math.PI * 2) / 360; 
         float minRingWidth = 120; //temp

         //have a random seed
         Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));

          arc.StartAngle = random.NextFloat(0, 90);
          arc.EndAngle = random.NextFloat(arc.StartAngle + minRingWidth, 350);
         // UnityEngine.Debug.Log($"StartAngle '{arc.StartAngle}', End Angle '{arc.EndAngle}'");
         
          for (int i = (int)arc.StartAngle; i < (arc.EndAngle + 1); i++) 
          {
               float rad = deg2rad * i;
               float3 position = new float3(ltw.Position.x + (math.sin(rad) * arc.Radius), 0, 
                  ltw.Position.z + (math.cos(rad) * arc.Radius));
            
               //instantiate prefabs with mesh render
               var instance = EntityManager.Instantiate(wall.wallPrefab);
               SetComponent(instance, new Translation{ Value = position });
            
          }
            
         //Only run this once
         EntityManager.RemoveComponent<WallAuthoring>(entity);
         
      }).WithoutBurst().Run();


      //Place Food
      Entities.WithStructuralChanges().WithAll<FoodTag>().ForEach((Entity entity, ref Arc arc, in LocalToWorld ltw, in FoodSpawnAuthoring foodSpawn) =>
      {
         Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));
         float deg2rad = (math.PI * 2) / 360;
         arc.StartAngle = random.NextFloat(0, 359);
         arc.EndAngle = arc.StartAngle + Arc.Size;
         
         float3 position = new float3(math.sin(deg2rad * arc.StartAngle)*arc.Radius,0, math.cos(deg2rad*arc.StartAngle)*arc.Radius);
         
         SetComponent(entity, new Translation{Value = position});

         EntityManager.RemoveComponent<FoodSpawnAuthoring>(entity);

      }).Run();

   }
   
   
   
}
