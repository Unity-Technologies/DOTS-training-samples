using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DoorAnimatorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref DoorCurrAnimTime animTime, in DoorState doorState) =>
        {
            switch(doorState.value)
            {
                // Door closing
                case 0:
                    {
                        animTime.value = math.max(0.0f, animTime.value - deltaTime);
                        break;
                    }
                // Door opening
                case 1:
                    {
                        animTime.value = math.min(1.0f, animTime.value + deltaTime);
                        break;
                    }
            }
        }).Schedule();
    }
}
