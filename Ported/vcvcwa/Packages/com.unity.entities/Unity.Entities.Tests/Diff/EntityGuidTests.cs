using System.Linq;
using NUnit.Framework;

namespace Unity.Entities.Tests
{
    class EntityGuidTests
    {
        [Test]
        public void Ctor_StoresValuesPacked()
        {
            var g0 = new EntityGuid(1, 2, 3);
            var g1 = new EntityGuid(-1, 0xF0, 0x89ABCDEF);

            Assert.That(g0.OriginatingId, Is.EqualTo(1));
            Assert.That(g0.NamespaceId, Is.EqualTo(2));
            Assert.That(g0.Serial, Is.EqualTo(3));

            Assert.That(g1.OriginatingId, Is.EqualTo(-1));
            Assert.That(g1.NamespaceId, Is.EqualTo(0xF0));
            Assert.That(g1.Serial, Is.EqualTo(0x89ABCDEF));
        }

        [Test]
        public void ToString_ExtractsPackedValues()
        {
            var g0 = new EntityGuid(1, 2, 3);
            var g1 = new EntityGuid(-1, 0xF0, 0x89ABCDEF);

            Assert.That(g0.ToString(), Is.EqualTo("1:02:00000003"));
            Assert.That(g1.ToString(), Is.EqualTo("-1:f0:89abcdef"));
        }

        [Test]
        public void Comparisons()
        {
            var guids = new[]
            {
                new EntityGuid(1, 2, 3),
                new EntityGuid(1, 2, 3),
                new EntityGuid(1, 2, 2),
                new EntityGuid(1, 1, 2),
                new EntityGuid(2, 1, 2),
                new EntityGuid(1, 2, 3),
            };

            var range = Enumerable.Range(0, guids.Length - 1).Select(i => (a: guids[i], b: guids[i + 1])).ToList();

            var equalsOp       = range.Select(v => v.a == v.b);
            var notEqualsOp    = range.Select(v => v.a != v.b);
            var equals         = range.Select(v => v.a.Equals(v.b));
            var hashCodeEquals = range.Select(v => v.a.GetHashCode() == v.b.GetHashCode());
            var compareTo      = range.Select(v => v.a.CompareTo(v.b));

            Assert.That(equalsOp,       Is.EqualTo(new[] { true, false, false, false, false }));
            Assert.That(notEqualsOp,    Is.EqualTo(new[] { false, true, true, true, true }));
            Assert.That(equals,         Is.EqualTo(new[] { true, false, false, false, false }));
            Assert.That(hashCodeEquals, Is.EqualTo(new[] { true, false, false, false, false }));
            Assert.That(compareTo,      Is.EqualTo(new[] { 0, 1, 1, -1, 1 }));
        }
    }
}
