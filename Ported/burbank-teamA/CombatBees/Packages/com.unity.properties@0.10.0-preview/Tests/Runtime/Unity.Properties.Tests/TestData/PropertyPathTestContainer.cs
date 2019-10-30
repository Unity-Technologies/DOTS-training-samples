using System.Collections.Generic;

namespace Unity.Properties.Tests
{
    public class SuperNested
    {
        public byte Byte;

        public List<double> Doubles = new List<double>{1.0, 2.0, 3.0, 4.0, 5.0};
    }
    
    public class PropertyPathTestContainer
    {
        public struct NestedPropertyPathTestContainer
        {
            public int Int;

            public List<double> Doubles;

            public SuperNested VeryNested;
        }
        
        public float Float = 5.0f;
        public List<string> Strings = new List<string>
        {
            "one", "two", "three"
        };

        public NestedPropertyPathTestContainer Nested = new NestedPropertyPathTestContainer
        {
            Int = 15,
            Doubles = new List<double> {1.0, 2.0, 3.0},
            VeryNested = new SuperNested()
        };

        public NestedPropertyPathTestContainer[] MultiNested = new NestedPropertyPathTestContainer[]
        {
            new NestedPropertyPathTestContainer
            {
                Int = 15,
                Doubles = new List<double> {1.0, 2.0, 3.0},
                VeryNested = new SuperNested()
            },
            new NestedPropertyPathTestContainer
            {
                Int = 15,
                Doubles = new List<double> {1.0, 2.0, 3.0},
                VeryNested = new SuperNested()
            }
        };
    }
}