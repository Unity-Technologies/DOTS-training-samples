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
         float maxRingWidth = 300; //temp


          //have a random seed
          Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));

         //the split arcs' angles will be set upon creatin
         if (arc.split == 0)
         {
            arc.StartAngle = random.NextFloat(0, 359);                                                
            arc.EndAngle = random.NextFloat(arc.StartAngle + minRingWidth, arc.StartAngle + maxRingWidth);     
         }
        
         float diff = math.abs(arc.EndAngle - arc.StartAngle);
         if (arc.split == 0 && diff <= 165)
         {
            float opening = (360 - (diff*2))/2;
            float start = arc.EndAngle + opening;
            float end = start + diff;
            
            //add and split
            EntityArchetype archetype = EntityManager.CreateArchetype(
               typeof(Arc),
               typeof(WallAuthoring),
               typeof(LocalToWorld));
            
            //add new arc entity
            Entity splitArc = EntityManager.CreateEntity(archetype);
            EntityManager.AddComponentData(splitArc, new WallAuthoring{wallPrefab = wall.wallPrefab});
            EntityManager.AddComponentData(splitArc, new Arc
            {
               Radius = arc.Radius,
               StartAngle = start,
               EndAngle =  end,
               split = 1
            });

         }
         
         //create arcs
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
      Entities.WithStructuralChanges().WithAll<FoodTag>().ForEach((Entity entity, in LocalToWorld ltw, in FoodSpawnAuthoring foodSpawn) =>
      {
         // return;

         Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));
         float foodAngleRad = random.NextFloat(0, math.radians(360.0f));
         float foodRadius = 20.0f;
         
         float3 position = new float3(math.sin(foodAngleRad) *foodRadius,0, math.cos(foodAngleRad)* foodRadius);
         
         SetComponent(entity, new Translation{Value = position});

         EntityManager.RemoveComponent<FoodSpawnAuthoring>(entity);

      }).Run();

   }

   static float ClampAngle(float angle)
   {
      float result = angle - math.ceil((angle / 360f) * 360f);
      if (result < 0)
      {
         result += 360f;
      }
      return result;

   }
   
}
