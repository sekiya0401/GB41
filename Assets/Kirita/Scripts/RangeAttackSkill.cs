using System.Collections;
using UnityEngine;

namespace Prototype.Games
{
    public class RangeAttackSkill : SkillMonoBehaviour
    {
        [SerializeField]
        private Collider m_Collider;
        [SerializeField,Range(0.1f,1.0f)]
        private float m_AttackTime;
        [SerializeField]
        private int m_Damage = 100;

        public override void Activate(Player _player)
        {
            transform.SetParent(null);
            StartCoroutine(Attack());
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IDamagable damagable))
            {
                if(damagable is Player)
                {
                    return;
                }

                damagable.Damage(m_Damage);
            }
        }

        private IEnumerator Attack()
        {
            m_Collider.enabled = true;
            yield return new WaitForSeconds(m_AttackTime);
            m_Collider.enabled = false;
            Debug.Log($"Finish Skill{name}");
            Destroy(gameObject);
        }
    }
}
