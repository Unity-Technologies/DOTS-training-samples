using System;
using System.Linq;
using NUnit.Framework;
using Unity.Entities.Conversion;
using UnityEngine;
using UnityEngine.TestTools;
using UnityDebug = UnityEngine.Debug;

namespace Unity.Entities.Tests.Conversion
{
    class JournalingUnityLoggerTestFixtureBase : ConversionMappingTestFixtureBase
    {
        ILogHandler m_OriginalLogger;
        JournalingUnityLogger m_JournalingLogger;

        protected ILogHandler OriginalLogger => m_OriginalLogger;
        protected JournalingUnityLogger Logger => m_JournalingLogger;

        [SetUp]
        public void SetUp()
        {
            m_OriginalLogger = UnityDebug.unityLogger.logHandler;
            m_JournalingLogger = new JournalingUnityLogger(m_MappingSystem);
        }
        
        [TearDown]
        public new void TearDown() => UnityDebug.unityLogger.logHandler = m_OriginalLogger;
    }

    class JournalingUnityLoggerHookTests : JournalingUnityLoggerTestFixtureBase
    {
        [Test]
        public void Hook_Unhook()
        {
            var old = UnityDebug.unityLogger.logHandler;
            Assert.That(old, Is.Not.SameAs(Logger));
            Logger.Hook();
            Assert.That(UnityDebug.unityLogger.logHandler, Is.SameAs(Logger));
            Logger.Unhook();
            Assert.That(UnityDebug.unityLogger.logHandler, Is.SameAs(old));
        }
        
        [Test]
        public void Hook_WithAlreadyHooked_Throws()
        {
            Assert.DoesNotThrow(() => Logger.Hook());
            
            var ex = Assert.Throws<InvalidOperationException>(() => Logger.Hook());
            Assert.That(ex.Message, Contains.Substring("double-hook"));
        }

        [Test]
        public void Unhook_WithNotHooked_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Logger.Unhook());
            Assert.That(ex.Message, Contains.Substring("not currently hooked").IgnoreCase);
        }
        
        [Test]
        public void Unhook_WithSomethingElseHookedBetween_Throws()
        {
            Assert.DoesNotThrow(() => Logger.Hook());
            
            UnityDebug.unityLogger.logHandler = OriginalLogger;
            
            var ex = Assert.Throws<InvalidOperationException>(() => Logger.Unhook());
            Assert.That(ex.Message, Contains.Substring("not currently hooked").IgnoreCase);
        }
    }

    class JournalingUnityLoggerTests : JournalingUnityLoggerTestFixtureBase
    {
        [SetUp]
        public new void Setup() => Logger.Hook();
        [TearDown]
        public new void TearDown() => Logger.Unhook();

        [Test]
        public void Log_WithNullContext_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Debug.Log("test log", null));
            Assert.DoesNotThrow(() => Debug.LogWarning("test warning", null));
            Assert.DoesNotThrow(() => Debug.LogError("test error", null));
            Assert.DoesNotThrow(() => Debug.LogException(new InvalidOperationException("test exception"), null));

            try { throw new ArgumentException("test exception2"); }
            catch (Exception x) { Assert.DoesNotThrow(() => Debug.LogException(x, null)); }

            LogAssert.Expect(LogType.Log, "test log");
            LogAssert.Expect(LogType.Warning, "test warning");
            LogAssert.Expect(LogType.Error, "test error");
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: test exception");
            LogAssert.Expect(LogType.Exception, "ArgumentException: test exception2");
        }

        [Test]
        public void Log_WithNullMessage_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Debug.Log(null));
            Assert.DoesNotThrow(() => Debug.LogWarning(null));
            Assert.DoesNotThrow(() => Debug.LogError(null));

            LogAssert.Expect(LogType.Log, "Null");
            LogAssert.Expect(LogType.Warning, "Null");
            LogAssert.Expect(LogType.Error, "Null");
        }

        [Test]
        public void Log_WithComponentContext_RecordsGameObjectContext()
        {
            var go = CreateGameObject();
            var comp = go.AddComponent<JournalTestAuthoring>();

            m_MappingSystem.CreatePrimaryEntity(go);

            Debug.LogError("test error0", go);
            Debug.LogError("test error1", comp);

            var logs = m_MappingSystem.JournalData.SelectJournalDataDebug().EventsOfType<LogEventData>().ToList();
            Assert.That(logs, Is.EqualTo(new[]
            {
                JournalDataDebug.Create(go.GetInstanceID(), new LogEventData { Type = LogType.Error, Message = "test error0"}),
                JournalDataDebug.Create(go.GetInstanceID(), new LogEventData { Type = LogType.Error, Message = "test error1"})
            }));

            LogAssert.Expect(LogType.Error, "test error0");
            LogAssert.Expect(LogType.Error, "test error1");
        }

        [Test]
        public void LogException_WithComponentContext_RecordsGameObjectContext()
        {
            var go = CreateGameObject();
            var comp = go.AddComponent<JournalTestAuthoring>();

            m_MappingSystem.CreatePrimaryEntity(go);

            try { throw new InvalidOperationException("test exception0"); } catch (Exception x) { Debug.LogException(x, go); }
            try { throw new InvalidOperationException("test exception1"); } catch (Exception x) { Debug.LogException(x, comp); }

            var logs = m_MappingSystem.JournalData.SelectJournalDataDebug().EventsOfType<LogEventData>().ToList();
            Assert.That(logs, Is.EqualTo(new[]
            {
                JournalDataDebug.Create(go.GetInstanceID(), new LogEventData { Type = LogType.Exception, Message = "InvalidOperationException: test exception0"}),
                JournalDataDebug.Create(go.GetInstanceID(), new LogEventData { Type = LogType.Exception, Message = "InvalidOperationException: test exception1"})
            }));

            LogAssert.Expect(LogType.Exception, "InvalidOperationException: test exception0");
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: test exception1");
        }
    }
}
