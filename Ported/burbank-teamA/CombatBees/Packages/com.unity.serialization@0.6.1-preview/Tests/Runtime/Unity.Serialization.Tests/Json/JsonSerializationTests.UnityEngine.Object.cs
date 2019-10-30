using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        const string k_AssetPath = "Assets/Tests/test-image.asset";

        class UnityEngineObjectContainer
        {
            public Texture2D Value;
        }

        class UnityEditorGlobalObjectIdContainer
        {
            public GlobalObjectId Value;
        }

        [SetUp]
        public void CreateAssets()
        {
            var image = new Texture2D(1, 1);
            AssetDatabase.CreateAsset(image, k_AssetPath);
            AssetDatabase.ImportAsset(k_AssetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }

        [TearDown]
        public void DeleteAssets()
        {
            AssetDatabase.DeleteAsset(k_AssetPath);
        }

        [Test]
        public void JsonSerialization_Serialize_UnityEngineObject()
        {
            var src = new UnityEngineObjectContainer { Value = AssetDatabase.LoadAssetAtPath<Texture2D>(k_AssetPath) };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""GlobalObjectId_V\d-\d-[\da-f]{32}-\d{7}-\d"".*"));

            var dst = new UnityEngineObjectContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.Not.Null);
                Assert.That(dst.Value, Is.Not.False);
                Assert.That(AssetDatabase.GetAssetPath(dst.Value), Is.EqualTo(k_AssetPath));
            }
        }

        [Test, Ignore("GlobalObjectIdentifierToObjectSlow currently returns null in this case")]
        public void JsonSerialization_Serialize_UnityEngineObject_DeserializeDeletedAsset()
        {
            var src = new UnityEngineObjectContainer { Value = AssetDatabase.LoadAssetAtPath<Texture2D>(k_AssetPath) };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""GlobalObjectId_V\d-\d-[\da-f]{32}-\d{7}-\d"".*"));

            AssetDatabase.DeleteAsset(k_AssetPath);

            var dst = new UnityEngineObjectContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.Not.Null);
                Assert.That(dst.Value, Is.False);
                Assert.That(dst.Value.GetType(), Is.EqualTo(typeof(Texture2D)));
            }
        }

        [Test]
        public void JsonSerialization_Serialize_UnityEngineObject_FromGlobalObjectId()
        {
            var src = new UnityEditorGlobalObjectIdContainer { Value = GlobalObjectId.GetGlobalObjectIdSlow(AssetDatabase.LoadAssetAtPath<Texture2D>(k_AssetPath)) };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""GlobalObjectId_V\d-\d-[\da-f]{32}-\d{7}-\d"".*"));

            var dst = new UnityEditorGlobalObjectIdContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.Not.EqualTo(new GlobalObjectId()));

                var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(dst.Value);
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj, Is.Not.False);
                Assert.That(AssetDatabase.GetAssetPath(obj), Is.EqualTo(k_AssetPath));
            }
        }
    }
}
