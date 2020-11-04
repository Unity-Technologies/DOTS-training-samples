using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(DoubleIntersectionSystem))]
public class TripleIntersectionSystem : SystemBase
{
    private Random m_Random;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_Random = Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 10000));
    }

    protected override void OnUpdate()
    {
        //var ecb = new EntityCommandBuffer(Allocator.Temp);

        Random random = Random.CreateFromIndex(m_Random.NextUInt());

        float deltaTime = Time.DeltaTime;
        
        Entities
            .ForEach((Entity entity, ref TripleIntersection tripleIntersection) =>
            {
                NativeArray<int> directions = new NativeArray<int>(3, Allocator.Temp);
                NativeArray<Lane> laneIns = new NativeArray<Lane>(3, Allocator.Temp);
                NativeArray<Entity> laneInEntities = new NativeArray<Entity>(3, Allocator.Temp);
                NativeArray<Lane> laneOuts = new NativeArray<Lane>(3, Allocator.Temp);

                directions[0] = tripleIntersection.lane0Direction;
                directions[1] = tripleIntersection.lane1Direction;
                directions[2] = tripleIntersection.lane2Direction;
                
                laneIns[0] = GetComponent<Lane>(tripleIntersection.laneIn0);
                laneIns[1] = GetComponent<Lane>(tripleIntersection.laneIn1);
                laneIns[2] = GetComponent<Lane>(tripleIntersection.laneIn2);
                
                laneInEntities[0] = tripleIntersection.laneIn0;
                laneInEntities[1] = tripleIntersection.laneIn1;
                laneInEntities[2] = tripleIntersection.laneIn2;
                
                laneOuts[0] = GetComponent<Lane>(tripleIntersection.laneOut0);
                laneOuts[1] = GetComponent<Lane>(tripleIntersection.laneOut1);
                laneOuts[2] = GetComponent<Lane>(tripleIntersection.laneOut2);
                
                for (int i = 0; i < laneIns.Length; i++)
                {
                    if (directions[i] != -1)
                        continue;
                    DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(laneInEntities[i]);
                    if (cars.IsEmpty)
                        continue;
                    Entity laneFirstCar = cars[0];
                    CarPosition carPosition = GetComponent<CarPosition>(laneFirstCar);
                    CarSpeed carSpeed = GetComponent<CarSpeed>(laneFirstCar);
                    
                    carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
                    if (carSpeed.NormalizedValue > 1.0f){
                        carSpeed.NormalizedValue = 1.0f;    
                    }
	            
                    float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                    
                    if(newPosition > laneIns[i].Length)
                    {
                        newPosition = laneIns[i].Length;
                        int value = random.NextInt(2);
                        if ((i == 0 || i == 1) && value == i)
                            value = 2;
                        
                        directions[i] = value;
                        if(i == 0)
                            tripleIntersection.lane0Direction = value;
                        else if(i == 1)
                            tripleIntersection.lane1Direction = value;
                        if(i == 2)
                            tripleIntersection.lane2Direction = value;
                    }
                    SetComponent(laneFirstCar, new CarPosition{Value = newPosition});
                    SetComponent(laneFirstCar, carSpeed);
                }
                
                if (tripleIntersection.car == Entity.Null)
                {
                    // TODO: Give proper priority
                    for (int i = 0; i < laneIns.Length; i++)
                    {
                        if (directions[i] != -1)
                        {
                            DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(laneInEntities[i]);
                            tripleIntersection.car = cars[0];
                            tripleIntersection.carIndex = i;
                            Lane lane = laneIns[i];
                            cars.RemoveAt(0);
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
                    // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
                    // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy
                    
                    int destination = directions[tripleIntersection.carIndex];
                    Lane newLaneOut = laneOuts[destination];
                    if (destination == 0)
                    {
                        SetComponent(tripleIntersection.laneOut0, newLaneOut);
                        DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(tripleIntersection.laneOut0);
                        cars.Add(tripleIntersection.car);
                    }
                    else if (destination == 1)
                    {
                        SetComponent(tripleIntersection.laneOut1, newLaneOut);
                        DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(tripleIntersection.laneOut1);
                        cars.Add(tripleIntersection.car);
                    }
                    else
                    {
                        SetComponent(tripleIntersection.laneOut2, newLaneOut);
                        DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(tripleIntersection.laneOut2);
                        cars.Add(tripleIntersection.car);
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
