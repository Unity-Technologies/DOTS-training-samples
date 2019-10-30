using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Unity.Properties
{
    public class VisitResult : IDisposable
    {
        static readonly Pool<VisitResult> s_Pool = new Pool<VisitResult>(() => new VisitResult());
        readonly List<VisitEvent> m_Events = new List<VisitEvent>();
        public bool Succeeded => m_Events.All(evt => evt.IsLog);
        
        public static VisitResult GetPooled() => s_Pool.Get();

        public IEnumerable<VisitEvent> AllEvents => m_Events;
        public IEnumerable<VisitEvent> Logs => m_Events.Where(evt => evt.IsLog);
        public IEnumerable<VisitEvent> Errors => m_Events.Where(evt => evt.IsError);
        public IEnumerable<VisitEvent> Exceptions => m_Events.Where(evt => evt.IsException);
        
        public void AddLog(string message)
        {
            var pooled = VisitEvent.GetPooled();
            pooled.Type = VisitEventType.Log;
            pooled.Payload = message;
            m_Events.Add(pooled);
        }

        public void AddError(string message)
        {
            var pooled = VisitEvent.GetPooled();
            pooled.Type = VisitEventType.Error;
            pooled.Payload = message;
            m_Events.Add(pooled);
        }
            
        public void AddException(Exception exception)
        {
            var pooled = VisitEvent.GetPooled();
            pooled.Type = VisitEventType.Exception;
            pooled.Payload = exception;
            m_Events.Add(pooled);
        }

        public void TransferEvents(VisitResult result)
        {
            m_Events.AddRange(result.m_Events);
            result.m_Events.Clear();
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void Throw()
        {
            var exceptions = m_Events
                .Where(evt => evt.IsException)
                .Select(evt => (Exception) evt.Payload);
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
        
        public void Dispose()
        {
            foreach (var visitEvent in m_Events)
            {
                visitEvent.Dispose();
            }
            m_Events.Clear();
            s_Pool.Release(this);
        }
    }
}
