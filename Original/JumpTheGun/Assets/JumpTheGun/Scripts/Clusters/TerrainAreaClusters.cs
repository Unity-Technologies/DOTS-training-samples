using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class TerrainAreaClusters
{
    	public static int2 BoxFromLocalPosition(float3 localPos, Config config){
			return new int2((int)math.round(localPos.x / config.spacing), (int)math.round(localPos.z / config.spacing));
		}

		public static float3 LocalPositionFromBox(int col, int row, Config config, float yPosition = 0){
			return new float3(col * config.spacing, yPosition, row * config.spacing);
		}
}
