#if !UNITY_DISABLE_MANAGED_COMPONENTS
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Entities
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad] // ensures type manager is initialized on domain reload when not playing
#endif
    static unsafe class AttachToEntityClonerInjection
    {
        // Injection is used to keep everything GameObject related outside of Unity.Entities

        static AttachToEntityClonerInjection()
        {
            InitializeTypeManager();
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeTypeManager()
        {
            ManagedComponentStore.InstantiateHybridComponent = InstantiateHybridComponentDelegate;
        }

        /// <summary>
        /// This method will handle the cloning of Hybrid Components (if any) during the batched instantiation of an Entity
        /// </summary>
        /// <param name="obj">The Hybrid Component. We don't know for sure it's a component to clone or to reference, this method is also about detecting this use case and the return value will give us the info</param>
        /// <param name="srcStore">Source Managed Store of the entity to instantiate</param>
        /// <param name="dstArch">Destination archetype of the entity to instantiate</param>
        /// <param name="dstStore">Destination Managed Store that will own the instances we create</param>
        /// <param name="dstManagedArrayIndex">Index of the Hybrid Component associated with <paramref name="obj"/></param>
        /// <param name="dstChunkCapacity">Chunk capacity</param>
        /// <param name="srcEntity">Entity to instantiate</param>
        /// <param name="dstEntities">Array containing all the entities instantiated</param>
        /// <param name="dstTypeIndex">Destination type index</param>
        /// <param name="dstBaseIndex">Destination base index</param>
        /// <param name="gameObjectInstances">An array that will contain all the cloned Game Object companion. This method will fill this array at the first call for a Hybrid Component to clone and will be used for subsequent ones</param>
        /// <returns><c>true</c> if the <paramref name="obj"/> was meant to be cloned, <c>false</c> if it is meant to be referenced</returns>
        static bool InstantiateHybridComponentDelegate(object obj, ManagedComponentStore srcStore,
            Archetype* dstArch, ManagedComponentStore dstStore, int dstManagedArrayIndex, int dstChunkCapacity,
            Entity srcEntity, NativeArray<Entity> dstEntities, int dstTypeIndex, int dstBaseIndex, ref object[] gameObjectInstances)
        {
            // For now, it only makes sense to support a conversion that happens in the same world
            // If whenever that changes, this assert will warn us it's not supported
            Assert.AreEqual(srcStore, dstStore, "Companion GameObject instancing assumes the src and dst EntityManager are the same, are you instancing across worlds?");

            // This method is only about cloning Hybrid Components: if it's not a component we have nothing to do
            var unityComponent = obj as UnityEngine.Component;
            if (unityComponent == null)
                return false;

            // This method is only about cloning Hybrid Components: if there's no Companion Link it means there's nothing to clone
            // This can still be a valid use case, the entity might be storing references to external Hybrid Components (e.g. first stage of conversion)
            var companionLink = unityComponent.gameObject.GetComponent<CompanionLink>();
            if (companionLink == null)
                return false;

            // The instantiation works in two phases:
            //  1) The first call on this method will clone the GameObject that owns the Hybrid Components for each instance we have to create and make it a Companion Game Object by attaching a CompanionLink component to it
            //  2) The first and all other calls will add the Hybrid Managed Component we cloned to the entity
            
            // 1) We know it's the first call if the array is null, it means we have to clone the GameObject for dstEntities.Length times and make it a Companion Game Object by adding a CompanionLink to it
            //    Cloning the Game Object will also clone its components, which is exactly what we're looking for.
            //    Note that we are relaxed with the content of the Game Object we clone: it may contains more than we need regarding the association with the Entity, but that's what we want,
            //      if the user adds Components to the Game Object that are not used by the entity: so be it, the user will still have these components in the instantiated Game Objects
            if (gameObjectInstances == null)
            {
                gameObjectInstances = new object[dstEntities.Length];

                for (int i = 0; i < dstEntities.Length; ++i)
                {
                    var instance = GameObject.Instantiate(unityComponent.gameObject);
                    instance.name = CompanionLink.GenerateCompanionName(dstEntities[i]);

                    gameObjectInstances[i] = instance;
                    instance.hideFlags |= HideFlags.HideInHierarchy;
                }
            }

            // For each instance we create, we add the cloned Hybrid Component to the entity
            for (int i = 0; i < dstEntities.Length; i++)
            {
                var componentInInstance = ((GameObject)gameObjectInstances[i]).GetComponent(obj.GetType());
                dstStore.SetManagedObject(dstArch, dstManagedArrayIndex, dstChunkCapacity, dstTypeIndex, dstBaseIndex + i, componentInInstance);
            }

            return true;
        }
    }
}
#endif
