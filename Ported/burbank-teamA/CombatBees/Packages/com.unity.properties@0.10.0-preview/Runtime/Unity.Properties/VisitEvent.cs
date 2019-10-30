using System;

namespace Unity.Properties
{
    enum VisitEventType
    {
        Log = 0,
        Error = 1,
        Exception = 2
    }
    
    public class VisitEvent : IDisposable
    {
        static readonly Pool<VisitEvent> s_Pool = new Pool<VisitEvent>(() => new VisitEvent());
        internal VisitEventType Type { get; set; }
        internal object Payload;

        public static VisitEvent GetPooled() => s_Pool.Get();
        public bool IsLog => Type == VisitEventType.Log;
        public bool IsError => Type == VisitEventType.Error;
        public bool IsException => Type == VisitEventType.Exception;

        public override string ToString() => Payload.ToString();

        public void Throw()
        {
            if (IsException)
            {
                throw (Exception) Payload;
            }
        }

        public void Dispose()
        {
            s_Pool.Release(this);
        }
    }
}