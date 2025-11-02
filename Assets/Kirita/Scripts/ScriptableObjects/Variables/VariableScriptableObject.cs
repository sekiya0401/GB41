using UnityEngine;

public abstract class VariableScriptableObject<T> : ScriptableObject
{
    [SerializeField]
    protected T m_Value;

    public virtual T Value
    {
        get => m_Value;
        set => m_Value = value;
    }
}