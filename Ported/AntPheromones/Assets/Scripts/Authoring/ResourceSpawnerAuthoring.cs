using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class ResourceSpawnerAuthoring : UnityMonoBehaviour
	, IConvertGameObjectToEntity
	, IDeclareReferencedPrefabs
{
	public UnityGameObject ResourcePrefab;

	// This function is required by IDeclareReferencedPrefabs
	public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
	{
		// Conversion only converts the GameObjects in the scene.
		// This function allows us to inject extra GameObjects,
		// in this case prefabs that live in the assets folder.
		referencedPrefabs.Add(ResourcePrefab);
	}

	public void Convert(Entity entity, EntityManager dstManager
		, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new ResourceSpawner
		{
			ResourcePrefab = conversionSystem.GetPrimaryEntity(ResourcePrefab)
		});
	}
}
