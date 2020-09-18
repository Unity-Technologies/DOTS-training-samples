using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics.Authoring;
using Unity.Physics;

public class BuildingCreate : SystemBase
{
    protected override void OnUpdate()
    {
        BuildingCreationData buildingData = GetSingleton<BuildingCreationData>();

        Entities.WithStructuralChanges().ForEach((Entity entity, in SpawnData spawnData) => {

            Entity previousEntityA = Entity.Null;
            Entity previousEntityB = Entity.Null;
            Entity previousEntityC = Entity.Null;
            Entity previousEntityD = Entity.Null;

            for (int i = 0; i < spawnData.height; i++)
            {
                float3 position = spawnData.position;


                RenderBounds bounds_vertical = EntityManager.GetComponentData<RenderBounds>(buildingData.vertical);
                float verticalHeight = bounds_vertical.Value.Size.y;
                float verticalWidth = bounds_vertical.Value.Size.x;
                float verticalDepth = bounds_vertical.Value.Size.z;
                RenderBounds bounds_horizontal = EntityManager.GetComponentData<RenderBounds>(buildingData.horizontal);
                float horizontalHeight = bounds_horizontal.Value.Size.y;
                float horizontalWidth = bounds_horizontal.Value.Size.x;
                float horizontalDepth = bounds_horizontal.Value.Size.z;
                RenderBounds bounds_horizontalthin = EntityManager.GetComponentData<RenderBounds>(buildingData.horizontalthin);
                float horizontalthinHeight = bounds_horizontal.Value.Size.y;
                float horizontalthinWidth = bounds_horizontal.Value.Size.x;
                float horizontalthinDepth = bounds_horizontal.Value.Size.z;

                //Vertical Column positioning
                float position_vert = position.y + i * verticalHeight;// + i * horizontalHeight;
                float position_depth = .5f * horizontalWidth + .5f*verticalWidth;
                float3 positionA = new float3( position_depth + position.x, position_vert,  position_depth + position.z);
                float3 positionB = new float3(-position_depth + position.x, position_vert,  position_depth + position.z);
                float3 positionC = new float3(-position_depth + position.x, position_vert, -position_depth + position.z);
                float3 positionD = new float3( position_depth + position.x, position_vert, -position_depth + position.z);

                var instanceA = EntityManager.Instantiate(buildingData.vertical);
                EntityManager.SetComponentData(instanceA, new Translation { Value = positionA });
                var instanceB = EntityManager.Instantiate(buildingData.vertical);
                EntityManager.SetComponentData(instanceB, new Translation { Value = positionB });
                var instanceC = EntityManager.Instantiate(buildingData.vertical);
                EntityManager.SetComponentData(instanceC, new Translation { Value = positionC });
                var instanceD = EntityManager.Instantiate(buildingData.vertical);
                EntityManager.SetComponentData(instanceD, new Translation { Value = positionD });

                //Horizontal Beam positioning
                //float position_hori = position_vert + verticalHeight;// - .5f* horizontalHeight;
                //float3 positionE = new float3(position_depth + position.x, position_hori, 0.0f + position.z);
                //float3 positionF = new float3(0.0f + position.x, position_hori, -position_depth + position.z);
                //float3 positionG = new float3(-position_depth + position.x, position_hori, 0.0f + position.z);
                //float3 positionH = new float3(0.0f + position.x, position_hori, position_depth + position.z);

                //var instanceE = EntityManager.Instantiate(buildingData.horizontalthin);
                //EntityManager.SetComponentData(instanceE, new Translation { Value = positionE });
                //var instanceF = EntityManager.Instantiate(buildingData.horizontal);
                //EntityManager.SetComponentData(instanceF, new Translation { Value = positionF });
                //var instanceG = EntityManager.Instantiate(buildingData.horizontalthin);
                //EntityManager.SetComponentData(instanceG, new Translation { Value = positionG });
                //var instanceH = EntityManager.Instantiate(buildingData.horizontal);
                //EntityManager.SetComponentData(instanceH, new Translation { Value = positionH });



                if (i > 0)
                {
                    PhysicsJoint jointA = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0.0f, 0.0f));
                    PhysicsJoint jointB = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0.0f, 0.0f));
                    PhysicsJoint jointC = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0.0f, 0.0f));
                    PhysicsJoint jointD = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0.0f, 0.0f));
                    PhysicsJoint jointE = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0f, -3.0f));
                    PhysicsJoint jointF = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0.0f, 3.0f));
                    PhysicsJoint jointG = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(3.0f, 0f, 0.0f));
                    PhysicsJoint jointH = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(-3.0f, 0f, 0.0f));
                    PhysicsJoint jointI = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0f, 3.0f));
                    PhysicsJoint jointJ = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(0.0f, 0f, -3.0f));
                    PhysicsJoint jointK = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(-3.0f, 0f, 0.0f));
                    PhysicsJoint jointL = PhysicsJoint.CreateBallAndSocket(new float3(0.0f, verticalHeight, 0.0f), new float3(3.0f, 0f, 0.0f));
                    //RigidTransform rigid = new RigidTransform(new quaternion(0.0f, 0.0f, 0.0f, 1.0f), new float3(0.0f, 0.0f, 0.0f));
                    //PhysicsJoint jointE = PhysicsJoint.CreateFixed(new BodyFrame(rigid), new BodyFrame(rigid)); 
                    //PhysicsJoint jointF = jointE;
                    //PhysicsJoint jointG = jointE;
                    //PhysicsJoint jointH = jointE;
                    //PhysicsJoint jointI = jointE;
                    //PhysicsJoint jointJ = jointE;
                    //PhysicsJoint jointK = jointE;
                    //PhysicsJoint jointL = jointE;

                    Entity jointentityA = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    Entity jointentityB = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    Entity jointentityC = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    Entity jointentityD = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityE = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityF = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityG = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityH = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityI = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityJ = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityK = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });
                    //Entity jointentityL = EntityManager.CreateEntity(new ComponentType[] { typeof(PhysicsConstrainedBodyPair), typeof(PhysicsJoint), typeof(JointBreakDistance) });


                    EntityManager.SetComponentData(jointentityA, new PhysicsConstrainedBodyPair(previousEntityA, instanceA, true));
                    EntityManager.SetComponentData(jointentityA, jointA);
                    EntityManager.SetComponentData(jointentityA, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    EntityManager.SetComponentData(jointentityB, new PhysicsConstrainedBodyPair(previousEntityB, instanceB, true));
                    EntityManager.SetComponentData(jointentityB, jointB);
                    EntityManager.SetComponentData(jointentityB, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    EntityManager.SetComponentData(jointentityC, new PhysicsConstrainedBodyPair(previousEntityC, instanceC, true));
                    EntityManager.SetComponentData(jointentityC, jointC);
                    EntityManager.SetComponentData(jointentityC, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    EntityManager.SetComponentData(jointentityD, new PhysicsConstrainedBodyPair(previousEntityD, instanceD, true));
                    EntityManager.SetComponentData(jointentityD, jointD);
                    EntityManager.SetComponentData(jointentityD, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityE, new PhysicsConstrainedBodyPair(previousEntityA, instanceE, true));
                    //EntityManager.SetComponentData(jointentityE, jointE);
                    //EntityManager.SetComponentData(jointentityE, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityF, new PhysicsConstrainedBodyPair(previousEntityD, instanceE, true));
                    //EntityManager.SetComponentData(jointentityF, jointF);
                    //EntityManager.SetComponentData(jointentityF, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityG, new PhysicsConstrainedBodyPair(previousEntityC, instanceF, true));
                    //EntityManager.SetComponentData(jointentityG, jointG);
                    //EntityManager.SetComponentData(jointentityG, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityH, new PhysicsConstrainedBodyPair(previousEntityD, instanceF, true));
                    //EntityManager.SetComponentData(jointentityH, jointH);
                    //EntityManager.SetComponentData(jointentityH, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityI, new PhysicsConstrainedBodyPair(previousEntityB, instanceG, true));
                    //EntityManager.SetComponentData(jointentityI, jointI);
                    //EntityManager.SetComponentData(jointentityI, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityJ, new PhysicsConstrainedBodyPair(previousEntityC, instanceG, true));
                    //EntityManager.SetComponentData(jointentityJ, jointJ);
                    //EntityManager.SetComponentData(jointentityJ, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityK, new PhysicsConstrainedBodyPair(previousEntityA, instanceH, true));
                    //EntityManager.SetComponentData(jointentityK, jointK);
                    //EntityManager.SetComponentData(jointentityK, new JointBreakDistance { Value = buildingData.jointBreakDistance });
                    //EntityManager.SetComponentData(jointentityL, new PhysicsConstrainedBodyPair(previousEntityB, instanceH, true));
                    //EntityManager.SetComponentData(jointentityL, jointL);
                    //EntityManager.SetComponentData(jointentityL, new JointBreakDistance { Value = buildingData.jointBreakDistance });


                }

                previousEntityA = instanceA;
                previousEntityB = instanceB;
                previousEntityC = instanceC;
                previousEntityD = instanceD;

            }
            EntityManager.DestroyEntity(entity);
        }).Run();
        
    }
}
