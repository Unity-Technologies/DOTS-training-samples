using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tests
{
    [TestFixture]
    public class ContextObjectSerializationTests
    {
        public Type[] GetIContextObjectTypes()
        {
            var blacklist = new[]
            {
                typeof(BuildCallbacks), typeof(Unity5PackedIdentifiers), typeof(PrefabPackedIdentifiers), typeof(LinearPackedIdentifiers), typeof(BuildCache),
                typeof(ProgressTracker), typeof(ProgressLoggingTracker), typeof(BuildInterfacesWrapper)
            };

            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "Unity.ScriptableBuildPipeline.Editor");
            return assembly.GetTypes().Where(x => typeof(IContextObject).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Where(x => !blacklist.Contains(x)).ToArray();
        }

        [Test]
        public void IContextObjects_SupportSerialization()
        {
            // This is just a generic catch all to ensure we properly setup C# serialization on IContextTypes
            // More explicit tests should be written per type to validate proper serialization in / out
            var types = GetIContextObjectTypes();
            foreach (var type in types)
                IContextObject_SupportSerialization(type);
        }

        static T SerializedAndDeserializeObject<T>(T obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                var obj2 = (T)formatter.Deserialize(stream);
                return obj2;
            }
        }

        static void IContextObject_SupportSerialization(Type type)
        {
            var instance1 = (IContextObject)Activator.CreateInstance(type, true);
            var instance2 = SerializedAndDeserializeObject(instance1);
            Assert.NotNull(instance2);
            Assert.AreEqual(type, instance2.GetType());
        }
    }
}
