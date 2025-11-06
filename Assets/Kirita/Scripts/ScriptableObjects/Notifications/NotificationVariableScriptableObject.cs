using UnityEngine.Events;
using MS.SO.Variable;

namespace MS.SO.Notification
{
    public abstract class NotificationVariableScriptableObject<T> : VariableScriptableObject<T>
    {
        public override T Value
        {
            get => m_Value;
            set
            {
                m_ChangedValueAction?.Invoke(m_Value, value);
                m_Value = value;
            }
        }

        private event UnityAction<T, T> m_ChangedValueAction;

        public event UnityAction<T, T> ChangedValue
        {
            add => m_ChangedValueAction += value;
            remove => m_ChangedValueAction -= value;
        }
    }
}