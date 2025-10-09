using Fusion;
using TMPro;
using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// Ç®ç’ÇËêßå‰
    /// </summary>
    public class Festival : NetworkBehaviour,IDamagable
    {
        private readonly int MAX_HEALTH = 100;

        [SerializeField]
        private TextMeshPro m_HealthField;

        [Networked, OnChangedRender(nameof(OnHealthChanged)), HideInInspector]
        public int Health { get; set; }

        private void OnHealthChanged()
        {
            m_HealthField.text = $"{Health} / {MAX_HEALTH}";
        }

        void IDamagable.Damage(int damage)
        {
            RPC_Damage(damage);
        }

        [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
        private void RPC_Damage(int damage)
        {
            Health = Mathf.Clamp(Health - damage, 0, MAX_HEALTH);
        }

        override public void Spawned()
        {
            Debug.Log("Festival Spawned");
            Debug.Log($"{Health}");

            if (Object.HasStateAuthority)
            {
                Health = MAX_HEALTH;
            }
            else
            {
                m_HealthField.text = $"{Health} / {MAX_HEALTH}";
            }
        }

        public override void Render()
        {
            m_HealthField.transform.LookAt(Camera.main.transform);
            // 180ìxâÒì]Ç≥ÇπÇƒê≥ñ ÇÉJÉÅÉâÇ…å¸ÇØÇÈ
            m_HealthField.transform.Rotate(0, 180f, 0);
        }
    }
}