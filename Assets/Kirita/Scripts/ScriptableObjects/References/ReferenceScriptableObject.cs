using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ReferenceScriptableObject<T> : VariableScriptableObject<T>
{
    public override T Value
    {
        get => m_Value;
        set
        {
            m_Value = value;
            m_ChangedValueAction?.Invoke(m_Value);
        }
    }

    private event UnityAction<T> m_ChangedValueAction;

    public event UnityAction<T> ChangedValue
    {
        add => m_ChangedValueAction += value;
        remove => m_ChangedValueAction -= value;
    }
}