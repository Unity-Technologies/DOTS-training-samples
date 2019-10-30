using System;
using System.Linq;
using NUnit.Framework;
using Random = System.Random;

namespace Unity.Entities.Tests.Types
{
    public class Hash128Tests
    {
        [Test]
        public void Equals_WithBoxedHash128_Throws()
        {
            var h0 = new Hash128();
            var h1 = new Hash128();
            
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var ex = Assert.Throws<InvalidOperationException>(() => Equals(h0, h1));
            Assert.That(ex.Message, Contains.Substring("inadvertent boxing"));
        }
        
        [Test]
        public void Equals_Matches()
        {
            var h0 = new Hash128(0, 1, 2, 3);
            var h1 = new Hash128(0, 1, 2, 3);
            var h2 = new Hash128(0, 1, 2, 4);
            
            Assert.True(h0.Equals(h1));
            Assert.True(h0 == h1);
            Assert.False(h0 != h1);
            
            Assert.False(h1.Equals(h2));
            Assert.False(h1 == h2);
            Assert.True(h1 != h2);
        }
        
        [Test]
        public void ToString_Basics()
        {
            Assert.That(new Hash128().ToString(),
                Is.EqualTo("00000000000000000000000000000000"));
            Assert.That(new Hash128(1, 2, 3, 4).ToString(),
                Is.EqualTo("10000000200000003000000040000000"));
            Assert.That(new Hash128(0xdeadbeef, 0, 0, 0xbaadf00d).ToString(),
                Is.EqualTo("feebdaed0000000000000000d00fdaab")); // vegan hash
        }
        
        [Test]
        public void ToString_ReturnsSameAsFramework()
        {
            string FrameworkToString(in Hash128 hash)
                => new string((
                        hash.Value[3].ToString("x8") +
                        hash.Value[2].ToString("x8") +
                        hash.Value[1].ToString("x8") +
                        hash.Value[0].ToString("x8"))
                    .Reverse().ToArray());

            var r = new Random(1);
            for (var i = 0; i < 10; ++i)
            {
                var h = new Hash128((uint)r.Next(), (uint)r.Next(), (uint)r.Next(), (uint)r.Next());
                var s = h.ToString();
                var f = FrameworkToString(h);
                Assert.That(s, Is.EqualTo(f));
            }
        }
        
        [Test]
        public void Comparisons()
        {
            var h0 = new Hash128(0, 0, 0, 0);                
            var h1 = new Hash128(1, 0, 0, 0);                
            var h2 = new Hash128(0, 1, 0, 0);                
            var h3 = new Hash128(0, 0, 1, 0);                
            
            Assert.False(h0 < default(Hash128));
            Assert.True(h0 < h1);
            Assert.True(h1 < h2);
            Assert.True(h2 < h3);
            
            Assert.False(h0 > default(Hash128));
            Assert.False(h0 > h1);
            Assert.False(h1 > h2);
            Assert.False(h2 > h3);
            
            Assert.That(h0.CompareTo(h0), Is.EqualTo(0));
            Assert.That(h1.CompareTo(h1), Is.EqualTo(0));
            Assert.That(h2.CompareTo(h2), Is.EqualTo(0));
            Assert.That(h3.CompareTo(h3), Is.EqualTo(0));
            
            Assert.That(h0.CompareTo(h1), Is.EqualTo(-1));
            Assert.That(h1.CompareTo(h2), Is.EqualTo(-1));
            Assert.That(h2.CompareTo(h3), Is.EqualTo(-1));
            
            Assert.That(h1.CompareTo(h0), Is.EqualTo(1));
            Assert.That(h2.CompareTo(h1), Is.EqualTo(1));
            Assert.That(h3.CompareTo(h2), Is.EqualTo(1));
        }
        
        #if UNITY_EDITOR
        [Test]
        public void Conversions_MatchUnityEditor()
        {
            var hashEntities = new Hash128(1, 2, 3, 4);
            var hashUnity = new UnityEditor.GUID("10000000200000003000000040000000");
            
            var hashUnityToEntities = (Hash128)hashUnity;
            var hashEntitiesToUnity = (UnityEditor.GUID)hashEntities;
            
            Assert.That(hashEntities, Is.EqualTo(hashUnityToEntities));
            Assert.That(hashUnity, Is.EqualTo(hashEntitiesToUnity));
        }
        #endif // UNITY_EDITOR

        #if !NET_DOTS
        [Test]
        public void Conversions_MatchUnityEngine()
        {
            var hashEntities = new Hash128(1, 2, 3, 4);
            var hashUnity = new UnityEngine.Hash128(1, 2, 3, 4);
            
            var hashUnityToEntities = (Hash128)hashUnity;
            var hashEntitiesToUnity = (UnityEngine.Hash128)hashEntities;
            
            Assert.That(hashEntities, Is.EqualTo(hashUnityToEntities));
            Assert.That(hashUnity, Is.EqualTo(hashEntitiesToUnity));
        }
        #endif
        
        [Test]
        public void StringToHash()
        {
            var h1 = new Hash128(1, 2, 3, 4);
            Assert.AreEqual(h1, new Hash128(h1.ToString()));

            var h2 = new Hash128(uint.MaxValue, uint.MinValue, 903290, 0);
            Assert.AreEqual(h2, new Hash128(h2.ToString()));
            
            Assert.AreEqual(new Hash128(), new Hash128("99"));
            Assert.AreEqual(new Hash128(), new Hash128("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"));
        }
    }
}
