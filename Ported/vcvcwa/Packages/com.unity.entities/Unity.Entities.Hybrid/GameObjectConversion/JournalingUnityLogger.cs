using System;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;
using UnityLogType = UnityEngine.LogType;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Conversion
{
    class JournalingUnityLogger : ILogHandler
    {
        ILogHandler m_HookedLogger;
        GameObjectConversionMappingSystem m_MappingSystem;
        
        public JournalingUnityLogger(GameObjectConversionMappingSystem mappingSystem) =>
            m_MappingSystem = mappingSystem;

        public void Hook()
        {
            if (m_HookedLogger != null)
                throw new InvalidOperationException("Unexpected double-hook");
            
            m_HookedLogger = UnityDebug.unityLogger.logHandler;
            UnityDebug.unityLogger.logHandler = this;
        }
        
        public void Unhook()
        {
            if (UnityDebug.unityLogger.logHandler != this)
                throw new InvalidOperationException("Not currently hooked into logger");
            
            UnityDebug.unityLogger.logHandler = m_HookedLogger;
            m_HookedLogger = null;
        }

        public void LogFormat(UnityLogType logType, UnityObject context, string format, object[] args)
        {
            if (context is Component component)
                context = component.gameObject;

            m_MappingSystem.JournalData.RecordLogEvent(context, logType, string.Format(format, args));
            m_HookedLogger?.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, UnityObject context)
        {
            if (context is Component component)
                context = component.gameObject;

            m_MappingSystem.JournalData.RecordExceptionEvent(context, exception);
            m_HookedLogger?.LogException(exception, context);
        }
    }
}
