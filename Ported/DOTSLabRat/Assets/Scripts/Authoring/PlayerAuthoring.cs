using Unity.Entities;
using Unity.Mathematics;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

namespace DOTSRATS
{
    public class PlayerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
    {
        public int PlayerNumber;
        public UnityEngine.Color PlayerColor;
        
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddBuffer<PlacedArrow>(entity);
            dstManager.AddComponentData(entity, new Player
            {
                playerNumber = PlayerNumber,
                color = PlayerColor,
                score = 0,
                arrowToPlace = new int2(-1,-1)
            });
        }
    }
}