using UnityEngine;

public abstract class CallbackView : MonoBehaviour
{
    [SerializeField]
    protected CallbackView[] m_CallbackViewes = new CallbackView[0];

    public abstract void Callback<T>(T value);
}
