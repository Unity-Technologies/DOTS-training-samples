using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TripleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref TripleIntersection tripleIntersection) =>
            {
                NativeArray<int> directions = new NativeArray<int>(3, Allocator.Temp);
                NativeArray<Lane> laneIns = new NativeArray<Lane>(3, Allocator.Temp);
                NativeArray<Lane> laneOuts = new NativeArray<Lane>(3, Allocator.Temp);

                directions[0] = tripleIntersection.lane0Direction;
                directions[1] = tripleIntersection.lane1Direction;
                directions[2] = tripleIntersection.lane2Direction;
                
                laneIns[0] = GetComponent<Lane>(tripleIntersection.laneIn0);
                laneIns[1] = GetComponent<Lane>(tripleIntersection.laneIn1);
                laneIns[2] = GetComponent<Lane>(tripleIntersection.laneIn2);
                
                laneOuts[0] = GetComponent<Lane>(tripleIntersection.laneOut0);
                laneOuts[1] = GetComponent<Lane>(tripleIntersection.laneOut1);
                laneOuts[2] = GetComponent<Lane>(tripleIntersection.laneOut2);
                
                if (tripleIntersection.car == Entity.Null)
                {
                    for (int i = 0; i < laneIns.Length; i++)
                    {
                        if (directions[i] != -1)
                            continue;
                        if (laneIns[i].Car == Entity.Null)
                            continue;
                        CarPosition carPosition = GetComponent<CarPosition>(laneIns[i].Car);
                        if (carPosition.Value == laneIns[i].Length)
                        {
                            // TODO: Make this random
                            int value = 0;
                            if (i == 0)
                                value = 1;
                            if (i == 1)
                                value = 2;
                            directions[i] = value;
                            if(i == 0)
                                tripleIntersection.lane0Direction = value;
                            else if(i == 1)
                                tripleIntersection.lane1Direction = value;
                            if(i == 2)
                                tripleIntersection.lane2Direction = value;
                        }
                    }
                    
                    // TODO: Give proper priority
                    for (int i = 0; i < laneIns.Length; i++)
                    {
                        if (directions[i] != -1)
                        {
                            tripleIntersection.car = laneIns[i].Car;
                            tripleIntersection.carIndex = i;
                            Lane lane = laneIns[i];
                            lane.Car = Entity.Null;
                            if (i == 0)
                                SetComponent(tripleIntersection.laneIn0, lane);    
                            else if (i == 1)
                                SetComponent(tripleIntersection.laneIn1, lane);    
                            else
                                SetComponent(tripleIntersection.laneIn2, lane);    
                            break;
                        }
                    }
                }
                else
                {
                    int destination = directions[tripleIntersection.carIndex];
                    Lane newLaneOut = laneOuts[destination];
                    newLaneOut.Car = tripleIntersection.car;
                    if (destination == 0)
                    {
                        SetComponent(tripleIntersection.laneOut0, newLaneOut);    
                    }
                    else if (destination == 1)
                    {
                        SetComponent(tripleIntersection.laneOut1, newLaneOut);    
                    }
                    else
                    {
                        SetComponent(tripleIntersection.laneOut2, newLaneOut);    
                    }
                    
                    SetComponent(tripleIntersection.car, new CarPosition{Value = 0});
                    tripleIntersection.car = Entity.Null;
                    
                    // Reset directions
                    if (tripleIntersection.carIndex == 0)
                        tripleIntersection.lane0Direction = -1;
                    else if (tripleIntersection.carIndex == 1)
                        tripleIntersection.lane1Direction = -1;    
                    else
                        tripleIntersection.lane2Direction = -1;    
                }

                directions.Dispose();
                laneIns.Dispose();
                laneOuts.Dispose();
            }).Run();

        //ecb.Playback(EntityManager);
        //ecb.Dispose();
    }
}
