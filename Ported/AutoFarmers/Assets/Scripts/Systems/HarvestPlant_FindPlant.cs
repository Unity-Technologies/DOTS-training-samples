using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class HarvestPlant_FindPlant : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            GetEntityQuery(ComponentType.ReadOnly<CellTypeElement>());
        }

        static int2 nextInspiral(int2 inp)
        {

            // required info
            int level = math.max(math.abs(inp.x), math.abs(inp.y));
            int2 delta = new int2(0,0);

            // calculate current direction (start up)
            if (-inp.x == level)
                delta.y = 1;    // going up
            else if (inp.y == level)
                delta.x = 1;    // going right
            else if (inp.x == level)
                delta.y = -1;    // going down
            else if (-inp.y == level)
                delta.x = -1;    // going left

            // check if we need to turn down or left
            if (inp.x > 0 && (inp.x == inp.y || inp.x == -inp.y))
            {
                // change direction (clockwise)
                delta = new int2(delta.y, -delta.x);
            }

            // move to next coordinate
            return inp + delta;
        }


        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            int2 GridSize = GetSingleton<Grid>().Size;
            Entity gridEntity = GetSingletonEntity<Grid>();
            DynamicBuffer<CellTypeElement> typeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);
            DynamicBuffer<CellEntityElement> entityBuffer = EntityManager.GetBuffer<CellEntityElement>(gridEntity);
            ComponentDataFromEntity<Sowed> sowedAccessor = GetComponentDataFromEntity<Sowed>();

            Entities
                .WithAll<HarvestPlant_Intent>()
                .WithNone<HarvestPlant>()
                .WithNone<TakePlantToStore>()
                .WithReadOnly(typeBuffer)
                .WithReadOnly(entityBuffer)
                .WithReadOnly(sowedAccessor)
                .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed, in Translation tr) =>
                {
                    int2 origin = CellPosition.FromTranslation(tr.Value);

                    Random random = new Random(randomSeed.Value);

                    bool plantFound = false;
                /* int numTries = 100;
                 int2 search = new int2(0, 0);
                 for (int i = 0; i < numTries; i++)
                 {
                     int2 pos = origin + search;
                     if (pos.x < 0 || pos.x >= GridSize.x || pos.y < 0 || pos.y >= GridSize.y) continue;
                     int idx = pos.x + pos.y * GridSize.x;
                     if (typeBuffer[idx].Value == CellType.Plant )
                     {
                         plantFound = true;
                         search = search + origin;
                         break;
                     }
                     search = nextInspiral(search);
                 }*/

                     int2 search = new int2(0,0);
                    for (int j = 0; j < GridSize.y; j++)
                    {
                        for (int i = 0; i < GridSize.x; i++)
                        {
                            int idx = i +j * GridSize.x;
                            if (typeBuffer[idx].Value == CellType.Plant)
                            {
                                plantFound = true;
                                search = new int2(i, j);
                            }
                        }
                    }

                    if (plantFound)
                    {
                        UnityEngine.Debug.LogError("Cell " + search);
                        Entity gridCellEnt = entityBuffer[search.x + search.y * GridSize.x].Value;
                        UnityEngine.Debug.LogError("Cell ent " + gridCellEnt);

                        if (!sowedAccessor.HasComponent(gridCellEnt))
                        {
                            UnityEngine.Debug.LogError("Grid has no plant but it's celltype was pant?");
                        }

                        var sowed = sowedAccessor[gridCellEnt];
                        if (sowed.Plant != Entity.Null)
                        {
                            UnityEngine.Debug.LogError("found a plant " + sowed.Plant);
                        }
                        Entity targetEntity = sowed.Plant;

                        //ecb.RemoveComponent<PathFindingTargetReached_Tag>(entityInQueryIndex, entity);
                        //ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                        ecb.AddComponent<HarvestPlant>(entityInQueryIndex, entity, new HarvestPlant
                        {
                            X = search.x,
                            Y = search.y,
                        });

                         ecb.AddComponent(entityInQueryIndex, entity, new Target()
                         {
                             Value = targetEntity
                         });
                    }
                    else
                    {
                        UnityEngine.Debug.Log("No plant found");
                        ecb.RemoveComponent<HarvestPlant_Intent>(entityInQueryIndex, entity);
                    }

                    randomSeed.Value = random.state;
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
