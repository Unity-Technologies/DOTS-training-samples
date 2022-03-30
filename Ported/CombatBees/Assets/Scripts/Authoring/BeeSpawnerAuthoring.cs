
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public enum TeamTag
{
	yellowTeam,
	blueTeam,
};

public class BeeSpawnerAuthoring : UnityMonoBehaviour
	, IConvertGameObjectToEntity
	, IDeclareReferencedPrefabs
{
	public UnityGameObject BeePrefab;
	[UnityRange(0, 1000)] public int TeamBeeCount;
	public float3 FieldCenterPosition;
	public float BeeHiveXOffset;
	public TeamTag BeeTeam;

	// This function is required by IDeclareReferencedPrefabs
	public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
	{
		// Conversion only converts the GameObjects in the scene.
		// This function allows us to inject extra GameObjects,
		// in this case prefabs that live in the assets folder.
		referencedPrefabs.Add(BeePrefab);
	}

	// This function is required by IConvertGameObjectToEntity
	public void Convert(Entity entity, EntityManager dstManager
		, GameObjectConversionSystem conversionSystem)
	{
		// GetPrimaryEntity fetches the entity that resulted from the conversion of
		// the given GameObject, but of course this GameObject needs to be part of
		// the conversion, that's why DeclareReferencedPrefabs is important here.
		dstManager.AddComponentData(entity, new BeeSpawnerComponent
		{
			BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
			BeeCount = TeamBeeCount,
			BeeSpawnPosition = new float3(FieldCenterPosition.x + BeeHiveXOffset, FieldCenterPosition.y, FieldCenterPosition.z),
			BeeTeamTag = BeeTeam
		});
	}
}
