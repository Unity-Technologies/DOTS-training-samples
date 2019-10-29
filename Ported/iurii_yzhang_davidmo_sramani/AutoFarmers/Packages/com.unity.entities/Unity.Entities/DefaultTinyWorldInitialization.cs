using System;
using UnityEngine;

namespace Unity.Entities
{
    public delegate void ConfigInit(World world);

    public static class DefaultTinyWorldInitialization
    {
        /// <summary>
        /// Initialize the Tiny World with all the boilerplate that needs to be done.
        /// ComponentSystems will be created and sorted into the high level ComponentSystemGroups.
        /// </summary>
        /// <remarks>
        /// The simple use case is:
        /// <code>
        /// world = DefaultTinyWorldInitialization.InitializeWorld("main");
        /// </code>
        /// However, it's common to need to set initialization data. That can be
        /// done with the following code:
        ///
        /// <code>
        ///   world = DefaultTinyWorldInitialization.InitializeWorld("main");
        ///   TinyEnvironment env = world.TinyEnvironment();
        ///   // set configuration variables...
        ///   DefaultTinyWorldInitialization.InitializeSystems(world);
        /// </code>
        /// </remarks>
        /// <seealso cref="InitializeWorld"/>
        /// <seealso cref="InitializeSystems"/>
        public static World Initialize(string worldName)
        {
            World world = InitializeWorld(worldName);
            InitializeSystems(world);
            // Note that System sorting is done by the individual ComponentSystemGroups, as needed.
            return world;
        }

        /// <summary>
        /// Initialize the World object. See <see cref="Initialize"/> for use.
        /// </summary>
        public static World InitializeWorld(string worldName)
        {
            var world = new World(worldName);
            World.DefaultGameObjectInjectionWorld = world;
            return world;
        }

        static void IterateAllAutoSystems(World world, Action<World, Type> Action)
        {
            InitializationSystemGroup initializationSystemGroup = world.GetExistingSystem<InitializationSystemGroup>();
            SimulationSystemGroup simulationSystemGroup = world.GetExistingSystem<SimulationSystemGroup>();
            PresentationSystemGroup presentationSystemGroup = world.GetExistingSystem<PresentationSystemGroup>();

            foreach (var systemType in TypeManager.GetSystems())
            {
                if (TypeManager.GetSystemAttributes(systemType, typeof(DisableAutoCreationAttribute)).Length > 0)
                    continue;
                if (systemType == initializationSystemGroup.GetType() ||
                    systemType == simulationSystemGroup.GetType() ||
                    systemType == presentationSystemGroup.GetType())
                {
                    continue;
                }

                Action(world, systemType);
            }
        }

        /// <summary>
        /// Initialize the ComponentSystems. See <see cref="Initialize"/> for use.
        /// </summary>
        public static void InitializeSystems(World world)
        {
            var allSystemTypes = TypeManager.GetSystems();
            if (allSystemTypes.Length == 0)
            {
                throw new InvalidOperationException("DefaultTinyWorldInitialization: No Systems found.");
            }

            // Create top level presentation system and simulation systems.
            InitializationSystemGroup initializationSystemGroup = new InitializationSystemGroup();
            world.AddSystem(initializationSystemGroup);

            SimulationSystemGroup simulationSystemGroup = new SimulationSystemGroup();
            world.AddSystem(simulationSystemGroup);

            PresentationSystemGroup presentationSystemGroup = new PresentationSystemGroup();
            world.AddSystem(presentationSystemGroup);

            // Create the working set of systems.
            // The full set of Systems must be created (and initialized with the World) before
            // they can be placed into SystemGroup. Else you get the problem that a System may
            // be put into a SystemGroup that hasn't been created.
            IterateAllAutoSystems(world, (World w, Type systemType) =>
            {
                // Need the if check because game/test code may have auto-constructed a System already.
                if (world.GetExistingSystem(systemType) == null)
                {
                    AddSystem(world, TypeManager.ConstructSystem(systemType), false);
                }
            });

            IterateAllAutoSystems(world, (World w, Type systemType) =>
            {
                AddSystemToGroup(world, world.GetExistingSystem(systemType));
            });
        }

        /// <summary>
        /// Call this to add a System that was manually constructed; normally these
        /// Systems are marked with [DisableAutoCreation].
        /// </summary>
        /// <param name="addSystemToGroup"></param> If true, the System will also be added to the correct
        /// SystemGroup (and the SystemGroup must already exist.) Otherwise, AddSystemToGroup() needs
        /// to be called separately, if needed.
        public static void AddSystem(World world, ComponentSystemBase system, bool addSystemToGroup)
        {
            if (world.GetExistingSystem(system.GetType()) != null)
                throw new ArgumentException("AddSystem: Error to add a duplicate system.");

            world.AddSystem(system);
            if (addSystemToGroup)
                AddSystemToGroup(world, system);
        }


        private static void AddSystemToGroup(World world, ComponentSystemBase system)
        {
            var groups = TypeManager.GetSystemAttributes(system.GetType(), typeof(UpdateInGroupAttribute));
            if (groups.Length == 0)
            {
                var simulationSystemGroup = world.GetExistingSystem<SimulationSystemGroup>();
                simulationSystemGroup.AddSystemToUpdateList(system);
            }

            for (int g = 0; g < groups.Length; ++g)
            {
                var groupType = groups[g] as UpdateInGroupAttribute;
                var groupSystem = world.GetExistingSystem(groupType.GroupType) as ComponentSystemGroup;
                if (groupSystem == null)
                    throw new Exception("AddSystem failed to find existing SystemGroup.");

                groupSystem.AddSystemToUpdateList(system);
            }
        }
    }
}
