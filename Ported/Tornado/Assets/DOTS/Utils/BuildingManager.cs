using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    public struct AnchorPoint
    {
        public float3 position;
        public float3 oldPosition;
        public int neighbors;
        public bool fixedPoint;
    }

    enum BeamUpdateFlags : int
    {
        None = 0,
        Position = 1 << 1,
        Rotation = 1 << 2
    }
    
    public struct Beam
    {
        public int2 points;
        public float length;
        public float3 position;
        public quaternion rotation;
        public float3 oldDelta;
        public bool fixedBeam;
    }

    public class BuildingManager
    {
        private static BuildingManager instance = null;

        private BuildingManager()
        {
        }
        
        ~BuildingManager()
        {
            if (AnchorPoints.IsCreated) 
                AnchorPoints.Dispose();
            if (Beams.IsCreated) 
                Beams.Dispose();
        }

        public void Init()
        {
            
        }

        public static BuildingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BuildingManager();
                }
                return instance;
            }
        }
        
        public static NativeArray<AnchorPoint> AnchorPoints;
        public static NativeArray<Beam> Beams;
    }
}