using UnityEngine;

namespace MS.Games
{
    public class AttackHandler : MonoBehaviour
    {
        [SerializeField,Range(0,10)]
        private int m_Damage = 1;
        private Collider m_AttackArea;

        private void Awake()
        {
            m_AttackArea = GetComponent<Collider>();
            m_AttackArea.enabled = false;
        }

        public void Attack()
        {
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