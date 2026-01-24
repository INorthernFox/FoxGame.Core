using System;
using Core.SceneManagers.Data;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.ResourceManagement.Load.Data
{
    public struct HandleEntry
    {
        public AsyncOperationHandle Handle;
        public int ReferenceCount;
        public SceneType? OwnerScene;
        public DateTime LoadedAt;

        public HandleEntry(AsyncOperationHandle handle, SceneType? ownerScene = null)
        {
            Handle = handle;
            ReferenceCount = 1;
            OwnerScene = ownerScene;
            LoadedAt = DateTime.UtcNow;
        }

        public HandleEntry IncrementReference()
        {
            ReferenceCount++;
            return this;
        }

        public HandleEntry DecrementReference()
        {
            ReferenceCount--;
            return this;
        }

        public bool IsValid => Handle.IsValid();
        public bool CanRelease => ReferenceCount <= 0;
    }
}
