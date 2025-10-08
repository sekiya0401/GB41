using UnityEngine;
using Prototype.ScriptableObjects;
using UnityEngine.UI;
using TMPro;

namespace Prototype.Games
{
    /// <summary>
    /// Float型のゲージを表示するUI
    /// </summary>
    public class FloatGauge : MonoBehaviour
    {
        [SerializeField]
        private FloatEventChannelScriptableObject m_EventChannel;
        [SerializeField]
        private Image m_Background;
        [SerializeField]
        private Image m_Fill;

        //HACK: Sliderを使わずにImageのfillAmountを使った方が良いかも
        private Slider m_Slider;

        private void Awake()
        {
            TryGetComponent(out m_Slider);
        }

        private void OnEnable()
        {
            if(m_EventChannel)
            {
                m_EventChannel.Event += OnUpdateGauge;
            }
        }

        private void OnDisable()
        {
            if (m_EventChannel)
            {
                m_EventChannel.Event -= OnUpdateGauge;
            }
        }

        /// <summary>
        /// ゲージ更新用のコールバック
        /// </summary>
        /// <param name="value">更新値</param>
        private void OnUpdateGauge(float value)
        {
            if (m_Slider != null)
            {
                m_Slider.value = value;
            }
        }

        /// <summary>
        /// イベントチャネルを差し替える
        /// </summary>
        /// <param name="floatEvent">イベントチャンネル</param>
        public void RetachEvent(FloatEventChannelScriptableObject floatEvent)
        {
            //NOTE: イベントの多重登録を防ぐため、一旦無効化してから再登録
            enabled = false;
            m_EventChannel = floatEvent;
            enabled = true;
        }

        /// <summary>
        /// ゲージの最大値と最小値を設定する
        /// </summary>
        /// <param name="max">最大値</param>
        /// <param name="min">最小値</param>
        public void SetMaxMin(float max, float min)
        {
            if (m_Slider != null)
            {
                m_Slider.maxValue = max;
                m_Slider.minValue = min;
            }
        }

        /// <summary>
        /// 背景色を設定する
        /// </summary>
        /// <param name="color">背景色</param>
        public void SetBackgroundColor(Color color)
        {
            if (m_Background != null)
            {
                m_Background.color = color;
            }
        }

        /// <summary>
        /// ゲージの色を設定する
        /// </summary>
        /// <param name="color">ゲージの色</param>
        public void SetFillColor(Color color)
        {
            if (m_Fill != null)
            {
                m_Fill.color = color;
            }
        }
    }
}