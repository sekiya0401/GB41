using Prototype.ScriptableObjects;
using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// プレイヤーの攻撃処理
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateSO m_PlayerState;

        //HACK: Triggerに入ったオブジェクトに対してダメージまたは回復を与える簡易的な実装
        private void OnTriggerEnter(Collider other)
        {
            // NOTE: 対象が子オブジェクトの場合も考慮して親オブジェクトと対象オブジェクト両方からIDamageableがないか探す

            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                player.Heal(m_PlayerState.Heal);
                return;
            }

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(m_PlayerState.Damage);
            }
        }
    }
}