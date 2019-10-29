using System;
using NUnit.Framework;
using Unity.Entities.Conversion;
using UnityEngine;

namespace Unity.Entities.Tests.Conversion
{
    class ConversionJournalDataTests : ConversionTestFixtureBase
    {
        ConversionJournalData m_JournalData = new ConversionJournalData();

        [SetUp]
        public void SetUp() => m_JournalData.Init();
        [TearDown]
        public new void TearDown() => m_JournalData.Dispose();

        [Test]
        public void AssertIsEquivalentTo_WithJournalDataDebugs_MatchesExpected()
        {
            var events = new[]
            {
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "warning" }),
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "error" })
            };

            Assert.That(events, Is.EquivalentTo(new[]
            {
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "warning" }),
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "error" })
            }));

            Assert.That(events, Is.EquivalentTo(new[]
            {
                 // reversed
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "error" }),
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "warning" })
            }));

            Assert.That(events, Is.Not.EquivalentTo(new[]
            {
                // part reversed
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "error" }),
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "warning" })
            }));
        }

        [Test]
        public void AssertIsEquivalentTo_WithIJournalDataDebugs_MatchesExpected()
        {
            var events = new IJournalDataDebug[]
            {
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "warning" }),
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "error" })
            };

            Assert.That(events, Is.EquivalentTo(new[]
            {
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "warning" }),
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "error" })
            }));

            Assert.That(events, Is.EquivalentTo(new[]
            {
                // reversed
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "error" }),
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "warning" })
            }));

            Assert.That(events, Is.Not.EquivalentTo(new[]
            {
                // part reversed
                JournalDataDebug.Create(1, new LogEventData { Type = LogType.Warning, Message = "error" }),
                JournalDataDebug.Create(2, new LogEventData { Type = LogType.Error, Message = "warning" })
            }));
        }

        [Test]
        public void RecordLogEvent_WithNullGameObject_IgnoresAndReturnsFalse()
        {
            var recorded0 = m_JournalData.RecordLogEvent(default, LogType.Error, "test error");

            Assert.That(recorded0, Is.False);
            Assert.That(m_JournalData.SelectJournalDataDebug(), Is.Empty);

            var recorded1 = m_JournalData.RecordLogEvent(default, LogType.Error, "test error");

            Assert.That(recorded1, Is.False);
            Assert.That(m_JournalData.SelectJournalDataDebug(), Is.Empty);
        }

        [Test]
        public void RecordLogEvent_WithUnknownGameObject_IgnoresAndReturnsFalse()
        {
            var go = CreateGameObject();

            var recorded = m_JournalData.RecordLogEvent(go, LogType.Error, "test error");

            Assert.That(recorded, Is.False);
            Assert.That(m_JournalData.SelectJournalDataDebug(), Is.Empty);
        }

        [Test]
        public void RecordEvent_WithCreatePrimaryEntity_CapturesTwoEvents()
        {
            var go = CreateGameObject();
            var entity = new Entity { Index = 0, Version = 1 };
            m_JournalData.RecordPrimaryEntity(go.GetInstanceID(), entity);

            var recorded = m_JournalData.RecordLogEvent(go, LogType.Log, "test log");

            Assert.That(recorded, Is.True);
            Assert.That(m_JournalData.SelectJournalDataDebug(), Is.EqualTo(new IJournalDataDebug[]
            {
                JournalDataDebug.Create(go.GetInstanceID(), entity),
                JournalDataDebug.Create(go.GetInstanceID(), new LogEventData { Type = LogType.Log, Message = "test log" }),
            }));
        }

        [Test]
        public void SelectLogEvents_WithHeadIdIndexButNoHead_DoesNotInclude()
        {
            var go0 = CreateGameObject();
            var go1 = CreateGameObject();

            // get two entries into the head index
            m_JournalData.RecordPrimaryEntity(go0.GetInstanceID(), default);
            m_JournalData.RecordPrimaryEntity(go1.GetInstanceID(), default);

            // but only record one of them for log entires
            m_JournalData.RecordLogEvent(go0, default, "log");

            // should only return the one entry, and not think that the second slot is a head
            Assert.That(m_JournalData.SelectLogEventsFast(), Is.EqualTo(new[]
            {
                (go0.GetInstanceID(), new LogEventData { Message = "log" })
            }));
        }

        [Test]
        public void SelectEvents_ReturnsMatchingEvents()
        {
            var go0 = CreateGameObject();
            var go1 = CreateGameObject();
            var go2 = CreateGameObject();

            var e0 = new Entity { Index = 1, Version = 2 };
            var e1 = new Entity { Index = 3, Version = 4 };

            m_JournalData.RecordPrimaryEntity(go0.GetInstanceID(), e0);
            m_JournalData.RecordPrimaryEntity(go1.GetInstanceID(), e1);
            m_JournalData.RecordLogEvent(go1, LogType.Assert, "test assert1");
            m_JournalData.RecordLogEvent(go0, LogType.Assert, "test assert0");

            Assert.That(m_JournalData.SelectLogEventsFast(go0), Is.EqualTo(new[]
                { new LogEventData { Type = LogType.Assert, Message = "test assert0" } }));
            Assert.That(m_JournalData.SelectLogEventsOrdered(go0), Is.EqualTo(new[]
                { new LogEventData { Type = LogType.Assert, Message = "test assert0" } }));
            Assert.That(m_JournalData.SelectEntities(go1), Is.EqualTo(new[]
                { e1 } ));
            Assert.That(m_JournalData.SelectLogEventsFast(go2), Is.Empty);
            Assert.That(m_JournalData.SelectLogEventsOrdered(go2), Is.Empty);

            Assert.That(m_JournalData.SelectJournalDataDebug(), Is.EquivalentTo(new IJournalDataDebug[]
            {
                JournalDataDebug.Create(go0.GetInstanceID(), e0),
                JournalDataDebug.Create(go0.GetInstanceID(), new LogEventData { Type = LogType.Assert, Message = "test assert0" }),
                JournalDataDebug.Create(go1.GetInstanceID(), e1),
                JournalDataDebug.Create(go1.GetInstanceID(), new LogEventData { Type = LogType.Assert, Message = "test assert1" }),
            }));
        }

        [Test]
        public void RecordPrimaryEntity_WithExistingPrimaryEntity_Throws()
        {
            m_JournalData.RecordPrimaryEntity(1, new Entity { Index = 1, Version = 2 });
            Assert.Throws<ArgumentException>(() => m_JournalData.RecordPrimaryEntity(1, new Entity { Index = 1, Version = 2 }));
            Assert.Throws<ArgumentException>(() => m_JournalData.RecordPrimaryEntity(1, new Entity { Index = 3, Version = 4 }));
            Assert.DoesNotThrow(() => m_JournalData.RecordPrimaryEntity(2, new Entity { Index = 1, Version = 2 }));
        }

        [Test]
        public void RecordAdditionalEntity_WithNoPrimaryEntity_Throws()
        {
            Assert.Throws<IndexOutOfRangeException>(() => m_JournalData.RecordAdditionalEntityAt(1, new Entity { Index = 1, Version = 2 }));
            m_JournalData.RecordPrimaryEntity(1, new Entity { Index = 1, Version = 2 });
            Assert.DoesNotThrow(() => m_JournalData.RecordAdditionalEntityAt(1, new Entity { Index = 3, Version = 4 }));
        }

        [Test]
        public void Entities_StoredInSerialOrder()
        {
            var go = CreateGameObject();
            var instanceId = go.GetInstanceID();

            var e0 = new Entity { Index = 5, Version = 6 };
            var e1 = new Entity { Index = 1, Version = 2 };
            var e2 = new Entity { Index = 3, Version = 4 };
            var e3 = new Entity { Index = 7, Version = 8 };

            m_JournalData.RecordPrimaryEntity(instanceId, e0);

            var s0 = m_JournalData.ReserveAdditionalEntity(instanceId);
            var s1 = m_JournalData.ReserveAdditionalEntity(instanceId);
            var s2 = m_JournalData.ReserveAdditionalEntity(instanceId);

            m_JournalData.RecordAdditionalEntityAt(s0.id, e1);
            m_JournalData.RecordAdditionalEntityAt(s1.id, e2);
            m_JournalData.RecordAdditionalEntityAt(s2.id, e3);

            Assert.That(new[] { s0.serial, s1.serial, s2.serial }, Is.EqualTo(new[] { 1, 2, 3 }));
            Assert.That(m_JournalData.SelectEntities(go), Is.EqualTo(new[] { e0, e1, e2, e3 }));
        }

        [Test]
        public void Events_StoredInCorrectOrder()
        {
            var go = CreateGameObject();
            var instanceId = go.GetInstanceID();
            m_JournalData.RecordPrimaryEntity(instanceId, default);

            m_JournalData.RecordLogEvent(go, default, "head");
            m_JournalData.RecordLogEvent(go, default, "extra 0");
            m_JournalData.RecordLogEvent(go, default, "extra 1");
            m_JournalData.RecordLogEvent(go, default, "extra 2");

            // with context

            Assert.That(m_JournalData.SelectLogEventsFast(go), Is.EqualTo(new[]
            {
                new LogEventData { Message = "head" },     // head never moves
                new LogEventData { Message = "extra 2" },  // remainder is always inserted after head
                new LogEventData { Message = "extra 1" },
                new LogEventData { Message = "extra 0" }
            }));

            Assert.That(m_JournalData.SelectLogEventsOrdered(go), Is.EqualTo(new[]
            {
                new LogEventData { Message = "head" },     // same order as added
                new LogEventData { Message = "extra 0" },
                new LogEventData { Message = "extra 1" },
                new LogEventData { Message = "extra 2" }
            }));

            // without context

            Assert.That(m_JournalData.SelectLogEventsFast(), Is.EqualTo(new[]
            {
                (instanceId, new LogEventData { Message = "head" }),
                (instanceId, new LogEventData { Message = "extra 2" }),
                (instanceId, new LogEventData { Message = "extra 1" }),
                (instanceId, new LogEventData { Message = "extra 0" })
            }));

            Assert.That(m_JournalData.SelectLogEventsOrdered(), Is.EqualTo(new[]
            {
                (instanceId, new LogEventData { Message = "head" }),
                (instanceId, new LogEventData { Message = "extra 0" }),
                (instanceId, new LogEventData { Message = "extra 1" }),
                (instanceId, new LogEventData { Message = "extra 2" })
            }));
        }
    }
}
