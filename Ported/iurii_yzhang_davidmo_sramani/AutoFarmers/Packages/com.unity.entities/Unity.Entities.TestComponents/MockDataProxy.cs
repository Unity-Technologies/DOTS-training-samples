using System;
using UnityEngine;

namespace Unity.Entities.Tests
{
    [Serializable]
    public struct MockData : IComponentData
    {
        public int Value;

        public MockData(int value) => Value = value;
        
        public override string ToString() => Value.ToString();
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class MockDataProxy : ComponentDataProxy<MockData>
    {
    }
}
