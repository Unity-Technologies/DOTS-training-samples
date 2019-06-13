using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
public class Proxy_Bee : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum Team
    {
        YellowTeam,
        PurpleTeam
    }

    public Team myTeam;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        C_Velocity vel = new C_Velocity() { Value = Vector3.zero };
        C_Shared_Team team = new C_Shared_Team() { Team = myTeam };


        dstManager.AddComponent(entity, typeof(Tag_Bee));
        dstManager.AddComponent(entity, typeof(C_TargetType));
        dstManager.AddComponent(entity, typeof(C_Random));
        dstManager.AddComponent(entity, typeof(C_PreviousPos));
        dstManager.AddComponent(entity, typeof(C_DeathTimer));
        dstManager.AddComponentData(entity, vel);
        dstManager.AddComponent(entity, typeof(C_Size));
        dstManager.AddComponent(entity, typeof(NonUniformScale));
        dstManager.AddSharedComponentData(entity, team);
        dstManager.AddComponent(entity, typeof(Tag_Bee_Init));
        dstManager.AddComponent(entity, typeof(Tag_Random_Init));
        dstManager.AddComponent(entity, typeof(Tag_ILookWhereImGoing));
        dstManager.AddComponent(entity, typeof(Tag_StretchByVelocity));
    }
}
