using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BotSpawningSystem))]
[UpdateAfter(typeof(WaterSpawningSystem))]
[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial struct OmniworkerSystem : ISystem
{
   private float speed;
   private float3 bucketPos;
   private float arriveThreshold;

   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
   }

   public void OnUpdate(ref SystemState state)
   {
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      arriveThreshold = config.arriveThreshold;

      //for each omniworker
      foreach (var (omniworkerTag, omniworkerTransform, omniworker) in SystemAPI.Query<OmniworkerBotTag,LocalTransform>().WithEntityAccess())
      {
         float minDist = float.MaxValue;
         float dist;
         //Get closest bucket
         foreach (var (bucket,bucketTransform) in SystemAPI.Query<Bucket,LocalTransform>())
         {
            dist = Vector3.Distance(bucketTransform.Position, omniworkerTransform.Position);
            if (dist < minDist)
            {
               minDist = dist;
               bucketPos = bucketTransform.Position;
            }
         }
         
         float3 dir = bucketPos - omniworkerTransform.Position;
         dist = Vector3.Distance(bucketPos, omniworkerTransform.Position);
         dir = Vector3.Normalize(dir);
      
         if (dist > arriveThreshold)
         {
            var omniworkerPosition = SystemAPI.GetComponent<LocalTransform>(omniworker);
            var prev = omniworkerPosition.Position.y;
            omniworkerPosition.Position += dir * SystemAPI.Time.DeltaTime * speed;
            omniworkerPosition.Position.y = prev;
            SystemAPI.SetComponent(omniworker, omniworkerPosition);
         }
         
      }
   }
}
