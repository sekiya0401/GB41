using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// ダメージフィールド
    /// HACK: Triggerに入ったオブジェクトに対してダメージを与える簡易的な実装
    /// </summary>
    public class DamageField : MonoBehaviour
    {
        [SerializeField, Range(-100, 100)]
        private int m_Damage = 10;

        private void OnTriggerEnter(Collider other)
        {
            //NOTE: 対象が子オブジェクトの場合も考慮して親オブジェクトと対象オブジェクト両方からIDamagableがないか探す
            var damageable = other.GetComponentInParent<IDamagable>();
            if (damageable != null)
            {
                damageable.Damage(m_Damage);
            }
        }
    }
}