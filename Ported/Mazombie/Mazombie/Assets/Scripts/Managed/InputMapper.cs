using System;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputMapper : MonoBehaviour
{
    World m_World;
    Entity m_InputEntity;
    bool m_MappingEnabled;

    void OnEnable()
    {
        m_World = World.All[0]; 
        if (m_World == null)
            return;
        m_InputEntity = m_World.EntityManager.CreateEntity(typeof(PlayerInput));
        EnableInputMapping(true);
    }

    private void Update()
    {
        if (m_World == null)
            return;

        var movementInput = Vector2.zero;
        // up
        if (Input.GetKey(KeyCode.W))
            movementInput.y += 1.0f;
        // down
        if (Input.GetKey(KeyCode.S))
            movementInput.y -= 1.0f;
        // left
        if (Input.GetKey(KeyCode.D))
            movementInput.x += 1.0f;
        // right
        if (Input.GetKey(KeyCode.A))
            movementInput.x -= 1.0f;
        
        var inputComponent = m_World.EntityManager.GetComponentData<PlayerInput>(m_InputEntity);

        if (!m_MappingEnabled)
        {
            inputComponent.movement = float2.zero;
        }
        else
        {
            inputComponent.movement = movementInput;
        }

        m_World.EntityManager.SetComponentData(m_InputEntity, inputComponent);
    }

    public void EnableInputMapping(bool isEnabled)
    {
        m_MappingEnabled = isEnabled;
    }
}
