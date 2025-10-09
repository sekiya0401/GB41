using UnityEngine;
using UnityEngine.Events;

namespace Prototype.ScriptableObjects
{
    /// <summary>
    /// Float型のイベントチャネル
    /// HACK: 値を保管できるようにした方が良いかも
    /// </summary>
    [CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Scriptable Objects/FloatEventChannelScriptableObject")]
    public class FloatEventChannelScriptableObject : ScriptableObject
    {
        //HACK: UnityEventを使うかどうかは要検討(System.Actionの方が適しているかも)
        private event UnityAction<float> m_Event;

        /// <summary>
        /// イベントの発火
        /// </summary>
        /// <param name="value">更新値</param>
        public void Raise(float value)
        {
            m_Event?.Invoke(value);
        }

        public event UnityAction<float> Event
        {
            add => m_Event += value;
            remove => m_Event -= value;
        }
    }
}