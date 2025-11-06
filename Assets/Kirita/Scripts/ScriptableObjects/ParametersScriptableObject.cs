using System;
using UnityEngine;

namespace MS.Games
{
    public abstract class Parameters : ScriptableObject
    {
        public Action SycnCallback
        {
            get;
            set;
        }

        public void SyncParameters()
        {
            SycnCallback?.Invoke();
        }
    }
}