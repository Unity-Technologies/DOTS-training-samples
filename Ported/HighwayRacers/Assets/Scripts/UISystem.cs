using Unity.Burst;
using Unity.Entities;

namespace HighwayRacers
{
    [BurstCompile]
    public partial struct UISystem : ISystem
    {
        bool shouldUpdateUI;
        bool shouldUpdate;
        TrackConfig trackConfigToUpdate;
        public void OnCreate(ref SystemState state)
        {
            // the defaults we're baked with are fine
            shouldUpdate = false;
            // ... but we need to tell the UI about the defaults.
            shouldUpdateUI = true;
            trackConfigToUpdate = new()
            {
                highwaySize = 0,
                numberOfCars = 0,
                dirty = false
            };

            state.RequireForUpdate<TrackConfigMinMax>();
            state.RequireForUpdate<TrackConfig>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        // can't use burst because accessing UI extern. is that okay? probly no impact on performance.
        public void OnUpdate(ref SystemState state)
        {
            if (shouldUpdate)
            {
                TrackConfig tc = SystemAPI.GetSingleton<TrackConfig>();

                if (tc.numberOfCars != trackConfigToUpdate.numberOfCars)
                {
                    tc.numberOfCars = trackConfigToUpdate.numberOfCars;
                    tc.dirty = true;
                }
                if (tc.highwaySize != trackConfigToUpdate.highwaySize)
                {
                    tc.highwaySize = trackConfigToUpdate.highwaySize;
                    tc.dirty = true;
                }

                SystemAPI.SetSingleton(tc);

                var trackGenerationSystem = state.World.GetExistingSystem<TrackGenerationSystem>();
                trackGenerationSystem.Struct.RegenerateTrack(state.EntityManager);

                var carSpawningSystem = state.World.GetExistingSystem<CarSpawningSystem>();
                carSpawningSystem.Struct.RespawnCars(state.EntityManager);

                shouldUpdate = false;
                shouldUpdateUI = true;
            }
            if (shouldUpdateUI)
            {
                // update the UI vis. two way coupling bad but fine for here. If these props get modified anywhere else this'll need 
                // to be revised. This also prevents burst compile, but since it's only singleton, we're probably ok. 
                HighwayOptions.instance.UpdateSliderValues(SystemAPI.GetSingleton<TrackConfigMinMax>(), SystemAPI.GetSingleton<TrackConfig>());
                shouldUpdateUI = false;
            }
        }

        /// <summary>
        /// Called by UI code to change the settings
        /// </summary>
        public void UpdateTrackConfiguration(TrackConfig configuration)
        {
            shouldUpdate = true;
            trackConfigToUpdate = configuration;
        }
    }

}