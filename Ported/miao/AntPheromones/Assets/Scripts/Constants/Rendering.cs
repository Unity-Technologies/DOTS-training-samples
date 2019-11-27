using UnityEngine;

namespace AntPheromones_ECS
{
    public static class Rendering
    {
        public const int MaxNumMeshesPerDrawCall = 1023;
        public static readonly int ColourId = Shader.PropertyToID("_Color"); 
    }
}