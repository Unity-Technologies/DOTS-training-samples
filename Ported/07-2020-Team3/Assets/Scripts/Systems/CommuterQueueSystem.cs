using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterQueueSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var queueRandom = new Random((uint)(Time.ElapsedTime * 10));

        Entities.ForEach((ref CommuterBoarding commuterBoarding, ref Translation translation, in Commuter commuter) =>
        {
            if (commuterBoarding.QueueIndex >= 0)
                return;

            var queueIndex = queueRandom.NextInt(0, 5);
            var platform = GetComponent<Platform>(commuter.CurrentPlatform);
            Entity queueWaypointEntity;
            switch (queueIndex)
            {
                case 0:
                    queueWaypointEntity = platform.Queue0;
                    break;
                case 1:
                    queueWaypointEntity = platform.Queue1;
                    break;
                case 2:
                    queueWaypointEntity = platform.Queue2;
                    break;
                case 3:
                    queueWaypointEntity = platform.Queue3;
                    break;
                default:
                    queueWaypointEntity = platform.Queue4;
                    break;
            }

            var queueWaypoint = GetComponent<Waypoint>(queueWaypointEntity);
            commuterBoarding.QueueIndex = queueIndex;
            translation.Value = queueWaypoint.WorldPosition;
        }).ScheduleParallel();
    }
}
