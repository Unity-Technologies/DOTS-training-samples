namespace Unity.Properties.Tests
{
    public interface IConstructInterface
    {
        
    }

    public abstract class AbstractConstructibleBaseType : IConstructInterface
    {
        
    }
    
    public class ConstructibleBaseType : AbstractConstructibleBaseType
    {
        public float Value;
        public ConstructibleBaseType()
        {
            Value = 25.0f;
        }
    }
    
    public class ConstructibleDerivedType : ConstructibleBaseType
    {
        public float SubValue;

        public ConstructibleDerivedType()
            :base()
        {
            SubValue = 50.0f;
        }
    }
    
    public class NonConstructibleDerivedType : ConstructibleBaseType
    {
        public NonConstructibleDerivedType(float a)
        {
        }
    }

    public class NoConstructorType
    {
        
    }

    public class ParameterLessConstructorType
    {
        public float Value; 
        
        public ParameterLessConstructorType()
        {
            Value = 25.0f;
        }
    }
    
    public class ParameterConstructorType
    {
        public float Value;
        
        public ParameterConstructorType(float a)
        {
            Value = a;
        }
    }

    public class ScriptableObjectType : UnityEngine.ScriptableObject
    {
    }
}