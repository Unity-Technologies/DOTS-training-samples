using System;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class ParticleManagerAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Mesh ParticleMesh;
        public Material ParticleMaterial;

        public Color SurfaceColor;
        public Color InteriorColor;
        public Color ExteriorColor;

        [Range(1f, 20f)]
        public float SpeedStretch = 5;
        [Range(1f, 20f)]
        public float Jitter = 10;
        [Range(1f, 5f)]
        public float Attraction = 3;

        [Range(0.1f, 20f)]
        public float ExteriorColorDist = 5;

        [Range(0.1f, 20f)]
        public float InteriorColorDist = 1.5f;

        [Range(0f, 1f)]
        public float ColorStiffness = .5f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddSharedComponentData(entity, new ParticleSetupComponent
                {
                    Mesh = ParticleMesh,
                    Material = ParticleMaterial,
                    SurfaceColor = SurfaceColor,
                    InteriorColor = InteriorColor,
                    ExteriorColor = ExteriorColor,
                    SpeedStretch = SpeedStretch,
                    Jitter = Jitter * 0.0001f,
                    Attraction = Attraction * 0.001f,
                    ExteriorColorDist = ExteriorColorDist,
                    InteriorColorDist = InteriorColorDist,
                    ColorStiffness = ColorStiffness
                }
            );
        }
    }
}
