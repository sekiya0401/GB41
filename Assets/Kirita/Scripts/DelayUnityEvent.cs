using System.Collections;
using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// �w�肵�����Ԃ̒x�����UnityEvent�𔭉΂���
    /// HACK: �N�����Ǝw�肵�����Ԃ̒x����ɃC�x���g�𔭉΂���ȈՓI�Ȏ���
    /// </summary>
    public class DelayUnityEvent : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Events.UnityEvent m_EnableEvent;
        [SerializeField]
        private UnityEngine.Events.UnityEvent m_DelayEvent;
        [SerializeField, Min(0.0f)]
        private float m_Delay = 1.0f;
        [SerializeField]
        private bool m_UnscaledTime = false;
        private bool m_IsInvoking = false;
        private Coroutine m_Coroutine = null;

        /// <summary>
        /// �C�x���g�̔���
        /// </summary>
        public void Invoke()
        {
            if (m_IsInvoking)
            {
                return;
            }

            m_IsInvoking = true;
            m_Coroutine = StartCoroutine(DelayEvent());
        }

        /// <summary>
        /// �C�x���g�̒�~
        /// </summary>
        public void Stop()
        {
            if(m_Coroutine != null)
            {
                m_IsInvoking = false;
                StopCoroutine(m_Coroutine);
            }
        }

        /// <summary>
        /// �x���C�x���g�̃R���[�`��
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayEvent()
        {
            m_EnableEvent?.Invoke();
            if (m_Delay > 0.0f)
            {
                if (m_UnscaledTime)
                    yield return new WaitForSecondsRealtime(m_Delay);
                else
                    yield return new WaitForSeconds(m_Delay);
            }
            m_DelayEvent?.Invoke();
            m_IsInvoking = false;
        }
    }
}