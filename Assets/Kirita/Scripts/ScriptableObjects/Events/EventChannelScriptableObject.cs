using UnityEngine;
using UnityEngine.Events;

namespace MS.SO.EventChannel
{
    public abstract class EventChannelScriptableObject<T> : ScriptableObject
    {
        private event UnityAction<T> m_ChangedValueAction;

        public event UnityAction<T> ChangedValue
        {
            add => m_ChangedValueAction += value;
            remove => m_ChangedValueAction -= value;
        }

        public void Invoke(T value)
        {
            m_ChangedValueAction?.Invoke(value);
        }
    }
}