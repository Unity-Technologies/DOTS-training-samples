using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BotSpawningSystem))]
[UpdateAfter(typeof(WaterSpawningSystem))]
[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial struct OmniworkerBucketSystem : ISystem
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

      //for each omniworker that goes for the bucket
      foreach (var (omniworkerTag, omniworkerTransform, omniworker) in SystemAPI.Query<OmniworkerGoForBucketTag,LocalTransform>().WithEntityAccess())
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
         Vector3 bucketPosWithoutY = new Vector3(bucketPos.x, 0f, bucketPos.z);
         Vector3 omniworkerPosWithoutY = new Vector3(omniworkerTransform.Position.x, 0f, omniworkerTransform.Position.z);
         dist = Vector3.Distance(bucketPosWithoutY, omniworkerPosWithoutY);
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
            SystemAPI.SetComponentEnabled<OmniworkerGoForBucketTag>(omniworker, false);
            SystemAPI.SetComponentEnabled<OmniworkerGoForWaterTag>(omniworker, true);
            //take the bucket and change tag
            //another system for looking for water
         }
         
      }
   }
}
