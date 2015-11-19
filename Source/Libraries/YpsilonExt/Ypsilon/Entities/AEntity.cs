﻿using System;
using Ypsilon.PlayerState;
using System.Collections.Generic;

namespace Ypsilon.Entities
{
    abstract class AEntity : IDisposable
    {
        protected EntityManager Manager
        {
            get;
            private set;
        }

        public Serial Serial
        {
            get;
            private set;
        }

        public bool IsPlayerEntity
        {
            get
            {
                return Serial == Vars.PlayerSerial;
            }
        }

        public AEntity(EntityManager manager, Serial serial)
        {
            Manager = manager;
            Serial = serial;
        }

        public virtual void Update(float frameSeconds)
        {
            if (m_Components != null)
            {
                foreach (AEntityComponent c in m_Components.Values)
                    c.Update(frameSeconds);
            }
        }

        #region Serialize/Unserialize Support
        public virtual void Serialize()
        {

        }

        protected virtual void Unserialize()
        {

        }
        #endregion

        #region Component Support
        private static Dictionary<Type, AEntityComponent> m_Components;

        public void AddComponent<T>(AEntityComponent component) where T : AEntityComponent
        {
            Type type = typeof(T);

            if (m_Components == null)
                m_Components = new Dictionary<Type, AEntityComponent>();
            m_Components.Add(type, component);
        }

        public T GetComponent<T>() where T : AEntityComponent
        {
            if (m_Components == null)
                return default(T);

            Type type = typeof(T);

            if (m_Components.ContainsKey(type))
                return (T)m_Components[type];
            else
                return default(T);
        }

        public void RemoveComponent<T>() where T : AEntityComponent
        {
            if (m_Components == null)
                return;

            Type type = typeof(T);

            if (m_Components.ContainsKey(type))
            {
                m_Components.Remove(type);
            }
        }
        #endregion

        #region IDisposable Support
        private bool m_IsDisposedValue = false; // To detect redundant calls

        public bool IsDisposed
        {
            get
            {
                return m_IsDisposedValue;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_IsDisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                m_IsDisposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AEntity() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
