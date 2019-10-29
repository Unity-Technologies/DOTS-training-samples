using System;
using UnityEngine;

namespace Unity.Entities.Tests
{
    [AddComponentMenu("")]
    public class JournalTestAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool ShouldWarn;
        public bool ShouldError;
        public bool ShouldErrorNoContext;
        public bool ShouldThrow;
        
        public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            if (ShouldWarn)
                Debug.LogWarning("JournalTestAuthoring.Convert warning", this);
            if (ShouldError)
                Debug.LogError("JournalTestAuthoring.Convert error", this);
            if (ShouldErrorNoContext)
                Debug.LogError("JournalTestAuthoring.Convert error (no context object)");
            if (ShouldThrow)
                throw new InvalidOperationException("JournalTestAuthoring.Convert exception");
        }
    }
}
