using UnityEngine;
using UnityEngine.Events;

namespace Prototype.ScriptableObjects
{
    /// <summary>
    /// Float�^�̃C�x���g�`���l��
    /// HACK: �l��ۊǂł���悤�ɂ��������ǂ�����
    /// </summary>
    [CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Scriptable Objects/FloatEventChannelScriptableObject")]
    public class FloatEventChannelScriptableObject : ScriptableObject
    {
        //HACK: UnityEvent���g�����ǂ����͗v����(System.Action�̕����K���Ă��邩��)
        private event UnityAction<float> m_Event;

        /// <summary>
        /// �C�x���g�̔���
        /// </summary>
        /// <param name="value">�X�V�l</param>
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