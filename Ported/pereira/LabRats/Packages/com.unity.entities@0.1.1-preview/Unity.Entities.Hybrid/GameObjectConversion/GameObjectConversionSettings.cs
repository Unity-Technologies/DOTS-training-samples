using System;
using System.IO;
using Object = UnityEngine.Object;
using ConversionFlags = Unity.Entities.GameObjectConversionUtility.ConversionFlags;

namespace Unity.Entities
{
    public class GameObjectConversionSettings
    {
        public World                    DestinationWorld;
        public Hash128                  SceneGUID;
        public ConversionFlags          ConversionFlags;
        public Type[]                   ExtraSystems = Array.Empty<Type>();
        public Action<World>            ConversionWorldCreated;        // get a callback right after the conversion world is created and systems have been added to it (good for tests that want to inject something)
        public Action<World>            ConversionWorldPreDispose;     // get a callback right before the conversion world gets disposed (good for tests that want to validate world contents)

        public GameObjectConversionSettings() { }

        
        // ** CONFIGURATION **
        
        public GameObjectConversionSettings(World destinationWorld, ConversionFlags conversionFlags)
        {
            DestinationWorld = destinationWorld;
            ConversionFlags = conversionFlags;
        }

        public GameObjectConversionSettings(World destinationWorld, Hash128 sceneGUID, ConversionFlags conversionFlags)
        {
            DestinationWorld = destinationWorld;
            SceneGUID = sceneGUID;
            ConversionFlags = conversionFlags;
        }

        public static implicit operator GameObjectConversionSettings(World destinationWorld)
            => new GameObjectConversionSettings { DestinationWorld = destinationWorld };

        public static implicit operator GameObjectConversionSettings(Hash128 sceneGuid)
            => new GameObjectConversionSettings { SceneGUID = sceneGuid };

        #if UNITY_EDITOR
        public static implicit operator GameObjectConversionSettings(UnityEditor.GUID sceneGuid)
            => new GameObjectConversionSettings { SceneGUID = sceneGuid };
        #endif

        // use this to inject systems into the conversion world (good for testing)
        public GameObjectConversionSettings WithExtraSystems(params Type[] extraSystems)
        {
            if (ExtraSystems != null && ExtraSystems.Length > 0)
                throw new InvalidOperationException($"{nameof(ExtraSystems)} already initialized");

            ExtraSystems = extraSystems;
            return this;
        }

        public GameObjectConversionSettings WithExtraSystem<T>()
            => WithExtraSystems(typeof(T));

        public GameObjectConversionSettings WithExtraSystems<T1, T2>()
            => WithExtraSystems(typeof(T1), typeof(T2));

        public GameObjectConversionSettings WithExtraSystems<T1, T2, T3>()
            => WithExtraSystems(typeof(T1), typeof(T2), typeof(T3));

        
        // ** CONVERSION **
        
        public World CreateConversionWorld()
            => GameObjectConversionUtility.CreateConversionWorld(this);

        
        // ** EXPORTING **
        
        public bool SupportsExporting
            => GetType() == typeof(GameObjectConversionSettings); 
        
        public virtual Guid GetGuidForAssetExport(Object uobject)
        {
            if (uobject == null)
                throw new ArgumentNullException(nameof(uobject));

            return Guid.Empty;
        }

        public virtual Stream TryCreateAssetExportWriter(Object uobject)
        {
            if (uobject == null)
                throw new ArgumentNullException(nameof(uobject));

            return null;
        }
    }
}
