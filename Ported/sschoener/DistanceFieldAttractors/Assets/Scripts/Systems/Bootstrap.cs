using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Systems
{
    public class Bootstrap : ICustomBootstrap
    {
        public List<Type> Initialize(List<Type> systems)
        {
            // this system seems to create garbage
            systems.Remove(typeof(Unity.Rendering.RenderMeshSystemV2));
            systems.Remove(typeof(Unity.Rendering.LightSystem));
            return systems;
        }
    }
}
