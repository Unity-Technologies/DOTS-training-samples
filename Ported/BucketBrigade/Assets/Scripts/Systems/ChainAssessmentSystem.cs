using System.Xml.Xsl;
using Authoring;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(BucketChainSpawnerSystem))]
    public partial struct ChainAssessmentSystem : ISystem
    {
        private float assessChainTimer;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
            assessChainTimer = 0;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
            var heatMap = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();
            assessChainTimer -= SystemAPI.Time.DeltaTime;

            if (assessChainTimer < 0)
            {
                foreach (var (scooper, thrower, bucketEntity ) in
                    SystemAPI.Query<RefRW<BotScooper>,RefRW<BotThrower>>()
                        .WithAll<BucketChain>()
                        .WithEntityAccess())
                {
                    
                    var bucketEntityChain = state.EntityManager.GetComponentData<BucketChain>(bucketEntity);
                    var passersEmptyBuffer = state.EntityManager.GetBuffer<PasserEmpty>(bucketEntity);
                    var passersFullBuffer = state.EntityManager.GetBuffer<PasserFull>(bucketEntity);
                    
                    var targetWater = state.EntityManager.GetComponentData<TargetWater>(bucketEntityChain.Scooper);
                    var waterVolume = state.EntityManager.GetComponentData<Volume>(targetWater.Value);

                    var scooperTransform =
                        state.EntityManager.GetComponentData<LocalTransform>(bucketEntityChain.Scooper);
                    
                    // Set scooper's nearest water source
                    if(targetWater.Value == Entity.Null || waterVolume.Value <=0){
                        
                        var nearestWater = FindWater(ref state, in scooperTransform.Position);
                        state.EntityManager.SetComponentData(bucketEntityChain.Scooper, new TargetWater{Value = nearestWater});
                        var waterTransform = state.EntityManager.GetComponentData<LocalTransform>(nearestWater);
                        
                        state.EntityManager.SetComponentData(bucketEntityChain.Scooper, new LocationPickup{Value = waterTransform.Position});
                        state.EntityManager.SetComponentData(bucketEntityChain.Scooper, new LocationDropoff{Value = waterTransform.Position});

                        // pick closest fire to SCOOPER
                        var nearestFlame = FindFire(ref state, ref heatMap, in scooperTransform.Position, config.flashpoint);
                        //state.EntityManager.SetComponentData(bucketEntityChain.Thrower, new TargetFlame{Value = nearestFlame});
                        var targetFlame = state.EntityManager.GetComponentData<TargetFlame>(bucketEntityChain.Thrower);

                        if (targetFlame.Value != null)
                        {
                            var flameTransform = state.EntityManager.GetComponentData<LocalTransform>(nearestFlame);
                            state.EntityManager.SetComponentData(bucketEntityChain.Thrower, new LocationPickup{Value = flameTransform.Position});
                            state.EntityManager.SetComponentData(bucketEntityChain.Thrower, new LocationDropoff{Value = flameTransform.Position});

                            // space out the PASSING chains
                            // EMPTY

                            int index = 0;
                            var chainEnd =
                                state.EntityManager.GetComponentData<LocationDropoff>(bucketEntityChain.Thrower);
                            var chainStart =
                                state.EntityManager.GetComponentData<LocationPickup>(bucketEntityChain.Scooper);
                            foreach (var passerEmpty in passersEmptyBuffer)
                            {
                                var locationpickup = GetChainPosition(index , config.totalPassersEmpty, chainEnd.Value ,chainStart.Value );
                                state.EntityManager.SetComponentData(passerEmpty.Value, new LocationPickup{Value = locationpickup});
                                
                                var locationdropoff = GetChainPosition(index+1 , config.totalPassersEmpty, chainEnd.Value ,chainStart.Value );
                                state.EntityManager.SetComponentData(passerEmpty.Value, new LocationDropoff{Value = locationdropoff});
                                index++;
                            }

                            index = 0;
                            // FULL
                            foreach (var passerFull in passersFullBuffer)
                            {
                                var locationpickup = GetChainPosition(index , config.totalPassersFull, chainStart.Value ,chainEnd.Value );
                                state.EntityManager.SetComponentData(passerFull.Value, new LocationPickup{Value = locationpickup});
                                
                                var locationdropoff = GetChainPosition(index+1 , config.totalPassersFull, chainStart.Value ,chainEnd.Value );
                                state.EntityManager.SetComponentData(passerFull.Value, new LocationDropoff{Value = locationdropoff});
                                index++;
                            }

                            var bucketProvider = (config.totalPassersFull > 0) ? passersFullBuffer[config.totalPassersFull - 1].Value : bucketEntityChain.Scooper;
                            state.EntityManager.SetComponentData(bucketEntityChain.Thrower, new BucketProvider{Value = bucketProvider});
                        }
                        
                    }
                }

                assessChainTimer = config.assessChainRate;
            }
        }
        
        float3 GetChainPosition(int _index, int _chainLength, float3 _startPos, float3 _endPos){
            // adds two to pad between the SCOOPER AND THROWER
            float progress = (float) _index / _chainLength;
            float curveOffset = Mathf.Sin(progress * Mathf.PI) * 1f;

            // get Vec2 data
            Vector2 heading = new Vector2(_startPos.x, _startPos.z) -  new Vector2(_endPos.x, _endPos.y);
            float distance = heading.magnitude;
            Vector2 direction = heading / distance;
            Vector2 perpendicular = new Vector2(direction.y, -direction.x);

            //Debug.Log("chain progress: " + progress + ",  curveOffset: " + curveOffset);
            return math.lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + 
                   (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
        }
        
        public Entity FindWater(ref SystemState state, in float3 botPos)
        {
            var minDistance = float.PositiveInfinity;
            var closestWater = Entity.Null;

            foreach (var (waterTransform, waterEntity)
                in SystemAPI.Query<RefRO<WorldTransform>>()
                    .WithAll<WaterAuthoring.Water>()
                    .WithEntityAccess())
            {
                var distance = math.distancesq(botPos, waterTransform.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestWater = waterEntity;
                    minDistance = distance;
                }
            }
            return closestWater;
        }
        
        public Entity FindFire(ref SystemState state, ref DynamicBuffer<ConfigAuthoring.FlameHeat> heatMap, in float3 botPos, in float flashPoint)
        {
            var minDistance = float.PositiveInfinity;
            var closestFire = Entity.Null;

            int index = 0;
            foreach (var (fireTransform, flameCell, fireEntity)
                in SystemAPI.Query<RefRO<LocalTransform>, RefRO<FlameCell>>()
                    .WithEntityAccess())
            {
                if (heatMap[index++].Value < flashPoint) continue;

                var distance = math.distancesq(botPos, fireTransform.ValueRO.Position);

                if (distance < minDistance)
                {
                    closestFire = fireEntity;
                    minDistance = distance;
                }
            }

            return closestFire;
        }
        
    }
}
