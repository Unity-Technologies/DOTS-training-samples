using System;

namespace Unity.Entities.Tests
{
    [Serializable]
    public struct MockSharedDisallowMultiple : ISharedComponentData
    {
        public int Value;
    }

    [UnityEngine.DisallowMultipleComponent]
    [UnityEngine.AddComponentMenu("")]
    public class MockSharedDisallowMultipleProxy : SharedComponentDataProxy<MockSharedDisallowMultiple>
    {

    }
}
