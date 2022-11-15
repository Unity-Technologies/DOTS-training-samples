using Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// Since converting player input to movement depends on the camera view angle,
// we must map the current active VCams transform to ECS space.
public class ActiveVCamMapper : MonoBehaviour
{
    public CinemachineVirtualCamera ActiveVCam;
    
    World m_World;
    Entity m_VCamEntity;

    void OnEnable()
    {
        m_World = World.All[0]; // todo: WorldUtils get default world
        if (m_World == null)
            return;
        m_VCamEntity = m_World.EntityManager.CreateEntity(typeof(ActiveCamera));
        m_World.EntityManager.AddComponent<LocalToWorldTransform>(m_VCamEntity);
    }

    private void Update()
    {
        if (m_World == null)
            return;
        var activeVCamTransform = ActiveVCam.transform;
        var activeVCamLocalToWorld = m_World.EntityManager.GetComponentData<LocalToWorldTransform>(m_VCamEntity);

        activeVCamLocalToWorld.Value = UniformScaleTransform
            .FromPositionRotation(activeVCamTransform.position, activeVCamTransform.rotation);

        m_World.EntityManager.SetComponentData(m_VCamEntity, activeVCamLocalToWorld);
    }
}