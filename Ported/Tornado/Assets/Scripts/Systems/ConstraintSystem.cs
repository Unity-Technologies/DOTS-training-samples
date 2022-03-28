using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    public partial class ConstraintSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();
            if (!pointDisplacement.isInitialized) return;

            for (int i = 0; i < pointDisplacement.links.Length; i++)
            {
				Link link = pointDisplacement.links[i];

				VerletPoints point1 = pointDisplacement.points[link.endIndex];
				VerletPoints point2 = pointDisplacement.points[link.startIndex];

				float dx = point2.currentPosition.x - point1.currentPosition.x;
				float dy = point2.currentPosition.y - point1.currentPosition.y;
				float dz = point2.currentPosition.z - point1.currentPosition.z;

				float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
				float extraDist = dist - link.length;



				float pushX = (dx / dist * extraDist) * .5f;
				float pushY = (dy / dist * extraDist) * .5f;
				float pushZ = (dz / dist * extraDist) * .5f;

				point1.currentPosition.x += pushX;
				point1.currentPosition.y += pushY;
				point1.currentPosition.z += pushZ;

				point2.currentPosition.x -= pushX;
				point2.currentPosition.y -= pushY;
				point2.currentPosition.z -= pushZ;

				pointDisplacement.points[link.endIndex] = point1;
				pointDisplacement.points[link.startIndex] = point2;
			}

        }
    }
}