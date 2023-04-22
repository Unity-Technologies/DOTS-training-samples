using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

[UpdateAfter(typeof(OmniworkerWaterSystem))]
[BurstCompile]
public partial struct OmniworkerFireSystem : ISystem
{
   private float speed;
   private float3 firePos;
   private float arriveThreshold;
   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
   }
   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      arriveThreshold = config.arriveThreshold;

      //for each omniworker that goes for extuinguishing the fire
      foreach (var (omniworkerTransform, omniworker) in SystemAPI.Query<LocalTransform>().WithEntityAccess().WithAll<OmniworkerGoForFireTag>())
      {
         float minDist = float.MaxValue;
         float dist;
         //Get closest fire
         foreach (var fireTransform in SystemAPI.Query<LocalTransform>().WithAll<OnFire>())
         {
            dist = Vector3.Distance(fireTransform.Position, omniworkerTransform.Position);
            if (dist < minDist)
            {
               minDist = dist;
               firePos = fireTransform.Position;
            }
         }
         
         float3 dir = firePos - omniworkerTransform.Position;
         Vector3 firePosWithoutY = new Vector3(firePos.x, 0f, firePos.z);
         Vector3 omniworkerPosWithoutY = new Vector3(omniworkerTransform.Position.x, 0f, omniworkerTransform.Position.z);
         dist = Vector3.Distance(firePosWithoutY, omniworkerPosWithoutY);
         dir = Vector3.Normalize(dir);
      
         if (dist > arriveThreshold)
         {
            var omniworkerPosition = SystemAPI.GetComponent<LocalTransform>(omniworker);
            var prev = omniworkerPosition.Position.y;
            omniworkerPosition.Position += dir * SystemAPI.Time.DeltaTime * speed;
            omniworkerPosition.Position.y = prev;
            SystemAPI.SetComponent(omniworker, omniworkerPosition);
         }
         else
         {
            SystemAPI.SetComponentEnabled<OmniworkerGoForFireTag>(omniworker, false);
            SystemAPI.SetComponentEnabled<OmniworkerGoForBucketTag>(omniworker, true);
            //extuinguish the fire and change tag
         }
         
      }
   }
}
