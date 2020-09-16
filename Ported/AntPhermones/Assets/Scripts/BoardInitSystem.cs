using Unity.Entities;
using Unity.Transforms;
using  Unity.Mathematics;
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
          arc.EndAngle = random.NextFloat(arc.StartAngle + minRingWidth, 359);
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

   }
   
   
   
}
