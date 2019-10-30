using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Unity.Properties.Tests
{
    public struct MyOwnVector
    {
        public float x;
        public float y;
    }
    
    public struct MyOwnVectorWithFormerlySerializedAs
    {
        [FormerlySerializedAs("x")]
        public float X;
        [FormerlySerializedAs("y")]
        public float Y;
    }
    
    public class FormerlySerializedAsMockData
    {
        public float MyFloat;
        public List<int> SomeList;
        public MyOwnVector MyVector;
    }
    
    public class FormerlySerializedAsData
    {
        [FormerlySerializedAs("MyFloat")]
        public float SomeSimpleFloat = 0;
        [FormerlySerializedAs("TryToFoolYou")]
        [FormerlySerializedAs("SomeList")]
        public List<int> ListOfInts = new List<int>( );
        
        [FormerlySerializedAs("MyVector")]
        public MyOwnVectorWithFormerlySerializedAs MyVectorRenamed;
    }
}