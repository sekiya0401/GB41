using UnityEngine;
using MS.SO.Variable;

[System.Serializable]
public abstract class ReferenceVariable<T>
{
    [SerializeField]
    protected T m_ConstantValue;
    [SerializeField]
    protected VariableScriptableObject<T> m_Variable;
    [SerializeField]
    private bool m_IsUseConstant = true;

    public T Value => m_IsUseConstant ? m_ConstantValue : m_Variable.Value;
}

[System.Serializable]
public class FloatReference : ReferenceVariable<float> { }
[System.Serializable]
public class IntReference : ReferenceVariable<int> { }
[System.Serializable]
public class DoubleReference : ReferenceVariable<double> { }
[System.Serializable]
public class LongReference : ReferenceVariable<long> { }

[System.Serializable]
public class ShortReference : ReferenceVariable<short> { }

[System.Serializable]
public class BoolReference : ReferenceVariable<bool> { }