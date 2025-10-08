using System.Collections;
using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// 指定した時間の遅延後にUnityEventを発火する
    /// HACK: 起動時と指定した時間の遅延後にイベントを発火する簡易的な実装
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
        /// イベントの発火
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
        /// イベントの停止
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
        /// 遅延イベントのコルーチン
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