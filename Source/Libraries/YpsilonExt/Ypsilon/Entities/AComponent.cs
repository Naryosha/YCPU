﻿namespace Ypsilon.Entities
{
    /// <summary>
    /// Components manage state-based update for an entity.
    /// They do not manage an entity's intrinsic variables.
    /// </summary>
    public class AComponent
    {
        public bool IsInitialized
        {
            get;
            private set;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public AComponent()
        {

        }

        public void Initialize(AEntity entity)
        {
            if (IsInitialized)
                return;

            OnInitialize(entity);

            IsInitialized = true;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDipose();

            IsDisposed = true;
        }

        protected virtual void OnInitialize(AEntity entity)
        {

        }

        protected virtual void OnDipose()
        {

        }

        public virtual void Update(AEntity entity, float frameSeconds)
        {

        }
    }
}
