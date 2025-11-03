using UnityEngine;

namespace MS.Games
{
    public class AttackHandler : MonoBehaviour
    {
        private Collider m_AttackArea;
        private int m_Damage = 0;

        private void Awake()
        {
            m_AttackArea = GetComponent<Collider>();
            m_AttackArea.enabled = false;
        }

        public void Attack(int damage)
        {
            m_Damage = damage;
            m_AttackArea.enabled = true;
        }

        public void Finish()
        {
            m_AttackArea.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(m_Damage);
            }
        }
    }
}