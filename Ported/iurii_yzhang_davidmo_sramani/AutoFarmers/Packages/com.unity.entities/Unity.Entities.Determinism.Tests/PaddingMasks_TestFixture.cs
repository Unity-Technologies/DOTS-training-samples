using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Determinism.Tests
{
    [TestFixture]
    public class PaddingMasks_TestFixture
    {
        internal PaddingMasks Masks;
        internal MaskedHasher MaskedHasher;
        internal DenseHasher DenseHasher;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TypeManager.Initialize();
            Masks = PaddingMaskBuilder.BuildPaddingMasks(TypeManager.GetAllTypes(), Allocator.Persistent);
            MaskedHasher = new MaskedHasher
            {
                Masks = Masks
            };
            DenseHasher = new DenseHasher();
            
            OnFixtureSetUp();
        }

        protected virtual void OnFixtureSetUp(){}
        protected virtual void OnFixtureTearDown(){}
        

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OnFixtureTearDown();
            Masks.Dispose();
        }

    }
}
