using System.Linq;
using NUnit.Framework;
using Unity.Entities.Conversion;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Tests.Conversion
{
    class UnityEngineExtensionsTests : ConversionTestFixtureBase
    {
        class TestScriptableObject : ScriptableObject { }
        class TestUnityComponent : MonoBehaviour { }

        [Test]
        public void IsAssetOrPrefab_MatchesWhatIsCreated()
        {
            var prefab = LoadPrefab("Prefab");
            var instantiated = InstantiateGameObject(prefab);
            var asset = CreateScriptableObject<TestScriptableObject>();
            
            Assert.That(prefab.IsPrefab(), Is.True);
            Assert.That(prefab.IsAsset, Is.False);
            
            Assert.That(instantiated.IsPrefab(), Is.False);
            Assert.That(instantiated.IsAsset, Is.False);
            
            Assert.That(asset.IsAsset, Is.True);
        }

        //@TODO: test IsActiveIgnorePrefab

        [Test]
        public void ComputeEntityHash_WithSeparateSubIDs_DoesNotCollide()
        {
            var go = CreateGameObject();

            var guids = new[]
            {
                go.ComputeEntityGuid(0, 0),
                go.ComputeEntityGuid(1, 0),
                go.ComputeEntityGuid(0, 1),
                go.ComputeEntityGuid(1, 1)
            };

            Assert.That(guids, Is.Unique);
            Assert.That(guids.Select(g => g.OriginatingId), Is.All.EqualTo(go.GetInstanceID()));
        }

        [Test]
        public void ComputeEntityHash_WithGameObjects()
        {
            var g0 = CreateGameObject();
            var g1 = CreateGameObject();

            var u0 = CreateScriptableObject<TestScriptableObject>();
            var u1 = CreateScriptableObject<TestScriptableObject>();

            var h01 = g0.ComputeEntityGuid(0, 0);
            var h02 = g0.ComputeEntityGuid(0, 1);

            var h10 = g1.ComputeEntityGuid(0, 0);
            var h11 = g1.ComputeEntityGuid(0, 1);

            var h20 = u0.ComputeEntityGuid(0, 0);
            var h21 = u0.ComputeEntityGuid(0, 1);

            var h30 = u1.ComputeEntityGuid(0, 0);
            var h31 = u1.ComputeEntityGuid(0, 1);

            Assert.That(new[] { h01, h02, h10, h11, h20, h21, h30, h31 }, Is.Unique);
        }

        [Test]
        public void ComputeEntityHash_WithComponent_UsesGameObject()
        {
            var go = CreateGameObject();
            var component = go.AddComponent<TestUnityComponent>();
            
            Assert.That(go.ComputeInstanceHash(), Is.EqualTo(component.ComputeInstanceHash()));
            Assert.That(go.ComputeEntityGuid(0, 0), Is.EqualTo(component.ComputeEntityGuid(0, 0)));
        }
    }
}
