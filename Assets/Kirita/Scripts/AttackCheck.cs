using System.Collections;
using TMPro;
using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// ダメージチェック
    /// </summary>
    public class AttackCheck : MonoBehaviour,IDamagable
    {
        [SerializeField,Min(0)]
        private int m_Health = 100;
        [SerializeField,Min(0)]
        private int m_MaxHealth = 100;
        [SerializeField]
        private Color m_LowHealthColor = Color.red;
        [SerializeField]
        private Color m_HighHealthColor = Color.green;
        [SerializeField]
        private TextMeshPro m_HealthField = null;
        private MeshRenderer m_MeshRenderer;
        private Coroutine m_ResetHealthCoroutine;

        private void Awake()
        {
            TryGetComponent(out m_MeshRenderer);

            UpdateState();
        }

        void IDamagable.Damage(int damage)
        {
            m_Health = Mathf.Clamp(m_Health - damage, 0, m_MaxHealth);
            UpdateState();

            //NOTE: ヘルスリセット前に再度ダメージを受けた場合、コルーチンを停止して再度開始する
            if (m_ResetHealthCoroutine == null)
            {
                m_ResetHealthCoroutine = StartCoroutine(ResetHealth());
            }
            else
            {
                StopCoroutine(m_ResetHealthCoroutine);
                m_ResetHealthCoroutine = StartCoroutine(ResetHealth());
            }
        }

        /// <summary>
        /// 状態の更新
        /// </summary>
        private void UpdateState()
        {
            Color color = Color.Lerp(m_LowHealthColor, m_HighHealthColor, (float)m_Health / m_MaxHealth);
            if (m_MeshRenderer != null)
            {
                m_MeshRenderer.material.color = color;
            }

            m_HealthField.text = $"HP:{m_Health}/{m_MaxHealth}";
        }

        /// <summary>
        /// ヘルスのリセット
        /// </summary>
        /// <returns></returns>
        IEnumerator ResetHealth()
        {
            yield return new WaitForSeconds(3);
            m_Health = m_MaxHealth;
            UpdateState();

            m_ResetHealthCoroutine = null;
        }
    }
}