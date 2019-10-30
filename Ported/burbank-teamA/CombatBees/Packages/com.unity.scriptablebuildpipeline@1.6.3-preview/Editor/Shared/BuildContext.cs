using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Basic implementation of IBuildContext. Stores data generated during a build.
    /// <seealso cref="IBuildContext"/>
    /// </summary>
    public class BuildContext : IBuildContext
    {
        Dictionary<Type, IContextObject> m_ContextObjects;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BuildContext()
        {
            m_ContextObjects = new Dictionary<Type, IContextObject>();
        }

        /// <summary>
        /// Default constructor, adds the passed in parameters to the context.
        /// </summary>
        /// <param name="buildParams">The set of initial parameters to add to the context.</param>
        public BuildContext(params IContextObject[] buildParams)
        {
            m_ContextObjects = new Dictionary<Type, IContextObject>();

            if (buildParams == null)
                return;

            foreach (var buildParam in buildParams)
            {
                if (buildParam != null)
                    SetContextObject(buildParam);
            }
        }

        /// <inheritdoc />
        public void SetContextObject<T>(IContextObject contextObject) where T : IContextObject
        {
            if (contextObject == null)
                throw new ArgumentNullException("contextObject");

            var type = typeof(T);
            if (!type.IsInterface)
                throw new InvalidOperationException(string.Format("Passed in type '{0}' is not an interface.", type));
            if (!(contextObject is T))
                throw new InvalidOperationException(string.Format("'{0}' is not of passed in type '{1}'.", contextObject.GetType(), type));
            m_ContextObjects[typeof(T)] = contextObject;
        }

        /// <inheritdoc />
        public void SetContextObject(Type type, IContextObject contextObject)
        {
            if (contextObject == null)
                throw new ArgumentNullException("contextObject");

            if (!type.IsInterface)
                throw new InvalidOperationException(string.Format("Passed in type '{0}' is not an interface.", type));
            if (!type.IsInstanceOfType(contextObject))
                throw new InvalidOperationException(string.Format("'{0}' is not of passed in type '{1}'.", contextObject.GetType(), type));
            m_ContextObjects[type] = contextObject;
        }

        /// <inheritdoc />
        public void SetContextObject(IContextObject contextObject)
        {
            if (contextObject == null)
                throw new ArgumentNullException("contextObject");

            var iCType = typeof(IContextObject);
            Type[] iTypes = contextObject.GetType().GetInterfaces();
            foreach (var iType in iTypes)
            {
                if (!iCType.IsAssignableFrom(iType) || iType == iCType)
                    continue;
                m_ContextObjects[iType] = contextObject;
            }
        }

        /// <inheritdoc />
        public bool ContainsContextObject<T>() where T : IContextObject
        {
            return ContainsContextObject(typeof(T));
        }

        /// <inheritdoc />
        public bool ContainsContextObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return m_ContextObjects.ContainsKey(type);
        }

        /// <inheritdoc />
        public T GetContextObject<T>() where T : IContextObject
        {
            return (T)m_ContextObjects[typeof(T)];
        }

        /// <inheritdoc />
        public IContextObject GetContextObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!m_ContextObjects.ContainsKey(type))
                Debug.LogError("Missing type in GetContextObject: " + type);

            return m_ContextObjects[type];
        }

        /// <inheritdoc />
        public bool TryGetContextObject<T>(out T contextObject) where T : IContextObject
        {
            IContextObject cachedContextObject;
            if (m_ContextObjects.TryGetValue(typeof(T), out cachedContextObject) && cachedContextObject is T)
            {
                contextObject = (T)cachedContextObject;
                return true;
            }

            contextObject = default(T);
            return false;
        }

        /// <inheritdoc />
        public bool TryGetContextObject(Type type, out IContextObject contextObject)
        {
            IContextObject cachedContextObject;
            if (m_ContextObjects.TryGetValue(type, out cachedContextObject) && type.IsInstanceOfType(cachedContextObject))
            {
                contextObject = cachedContextObject;
                return true;
            }

            contextObject = null;
            return false;
        }
    }
}