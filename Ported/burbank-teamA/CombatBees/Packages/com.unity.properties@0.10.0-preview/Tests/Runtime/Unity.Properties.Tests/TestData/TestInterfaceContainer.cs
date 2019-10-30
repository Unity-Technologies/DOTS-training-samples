namespace Unity.Properties.Tests
{
    public interface ICustomData
    {
        
    }

    public class CustomDataFoo : ICustomData
    {
        public int Test;
        public int Foo;
    }
    
    public class CustomDataBar : ICustomData
    {
        public int Test;
        public int Bar;
    }
    
    public struct TestInterfaceContainer
    {
        public ICustomData CustomData;
    }

    public class CustomDataFooPropertyBag : PropertyBag<CustomDataFoo>
    {
        static readonly ValueProperty<CustomDataFoo, int> s_Test = new ValueProperty<CustomDataFoo, int>(
            nameof(CustomDataFoo.Test), 
            (ref CustomDataFoo container) => container.Test,
            (ref CustomDataFoo container, int value) => container.Test = value
        );

        static readonly ValueProperty<CustomDataFoo, int> s_Foo = new ValueProperty<CustomDataFoo, int>(
            nameof(CustomDataFoo.Foo), 
            (ref CustomDataFoo container) => container.Foo,
            (ref CustomDataFoo container, int value) => container.Foo = value
        );
        
        public override void Accept<TVisitor>(ref CustomDataFoo container, ref TVisitor visitor, ref ChangeTracker changeTracker) 
        {
            visitor.VisitProperty<ValueProperty<CustomDataFoo, int>, CustomDataFoo, int>(s_Test, ref container, ref changeTracker);
            visitor.VisitProperty<ValueProperty<CustomDataFoo, int>, CustomDataFoo, int>(s_Foo, ref container, ref changeTracker);
        }
        
        public override bool FindProperty<TAction>(string name, ref CustomDataFoo container, ref ChangeTracker changeTracker, ref TAction action)
        {
            if (string.Equals(name, s_Test.GetName()))
            {
                action.VisitProperty<ValueProperty<CustomDataFoo, int>, int>(s_Test, ref container, ref changeTracker);
                return true;
            }
            
            if (string.Equals(name, s_Foo.GetName()))
            {
                action.VisitProperty<ValueProperty<CustomDataFoo, int>, int>(s_Foo, ref container, ref changeTracker);
                return true;
            }

            return false;
        }
    }
    
    public class CustomDataBarPropertyBag : PropertyBag<CustomDataBar>
    {
        static readonly ValueProperty<CustomDataBar, int> s_Test = new ValueProperty<CustomDataBar, int>(
            nameof(CustomDataBar.Test), 
            (ref CustomDataBar container) => container.Test,
            (ref CustomDataBar container, int value) => container.Test = value
        );

        static readonly ValueProperty<CustomDataBar, int> s_Bar = new ValueProperty<CustomDataBar, int>(
            nameof(CustomDataBar.Bar), 
            (ref CustomDataBar container) => container.Bar,
            (ref CustomDataBar container, int value) => container.Bar = value
        );
        
        public override void Accept<TVisitor>(ref CustomDataBar container, ref TVisitor visitor, ref ChangeTracker changeTracker) 
        {
            visitor.VisitProperty<ValueProperty<CustomDataBar, int>, CustomDataBar, int>(s_Test, ref container, ref changeTracker);
            visitor.VisitProperty<ValueProperty<CustomDataBar, int>, CustomDataBar, int>(s_Bar, ref container, ref changeTracker);
        }
        
        public override bool FindProperty<TAction>(string name, ref CustomDataBar container, ref ChangeTracker changeTracker, ref TAction action)
        {
            if (string.Equals(name, s_Test.GetName()))
            {
                action.VisitProperty<ValueProperty<CustomDataBar, int>, int>(s_Test, ref container, ref changeTracker);
                return true;
            }
            
            if (string.Equals(name, s_Bar.GetName()))
            {
                action.VisitProperty<ValueProperty<CustomDataBar, int>, int>(s_Bar, ref container, ref changeTracker);
                return true;
            }

            return false;
        }
    }
    
    public class TestInterfaceContainerPropertyBag : PropertyBag<TestInterfaceContainer>
    {
        static readonly ValueProperty<TestInterfaceContainer, ICustomData> s_CustomData = new ValueProperty<TestInterfaceContainer, ICustomData>(
            nameof(CustomDataBar.Test), 
            (ref TestInterfaceContainer container) => container.CustomData,
            (ref TestInterfaceContainer container, ICustomData value) => container.CustomData = value
        );

        public override void Accept<TVisitor>(ref TestInterfaceContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
        {
            visitor.VisitProperty<ValueProperty<TestInterfaceContainer, ICustomData>, TestInterfaceContainer, ICustomData>(s_CustomData, ref container, ref changeTracker);
        }

        public override bool FindProperty<TAction>(string name, ref TestInterfaceContainer container, ref ChangeTracker changeTracker, ref TAction action)
        {
            if (string.Equals(name, s_CustomData.GetName()))
            {
                action.VisitProperty<ValueProperty<TestInterfaceContainer, ICustomData>, ICustomData>(s_CustomData, ref container, ref changeTracker);
                return true;
            }
            
            return false;
        }
    }
}
