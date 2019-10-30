namespace Unity.Properties.Tests
{
    public static class TestData
    {
        public static void InitializePropertyBags()
        {
            PropertyBagResolver.Register(new TestNestedContainerPropertyBag());
            PropertyBagResolver.Register(new TestPrimitiveContainerPropertyBag());
            PropertyBagResolver.Register(new TestArrayContainerPropertyBag());
            PropertyBagResolver.Register(new TestListContainerPropertyBag());
            PropertyBagResolver.Register(new CustomDataFooPropertyBag());
            PropertyBagResolver.Register(new CustomDataBarPropertyBag());
        }
    }
}