using Fusion;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype.Games
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimpleEnemy : NetworkBehaviour,IDamagable
    {
        [SerializeField]
        private FloatVariableScriptableObject m_DespawnSpeedThreshold;
        [SerializeField]
        private FloatVariableScriptableObject m_DespawnDelay;
        private float m_DespawnTimer = 0f;
        private NavMeshAgent m_Agent;
        private Transform m_Target;

        public override void Spawned()
        {
            if(!HasStateAuthority)
            {
                m_Agent.enabled = false;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if(!HasStateAuthority)
            {
                return;
            }

            // NavMeshAgent の速度をチェック
            if (m_Agent.velocity.magnitude < m_DespawnSpeedThreshold.Value)
            {
                m_DespawnTimer += Runner.DeltaTime;
                if (m_DespawnTimer >= m_DespawnDelay.Value)
                {
                    SelfDespawn();
                }
            }
            else
            {
                m_DespawnTimer = 0f;
            }
        }

        private void Awake()
        {
            TryGetComponent(out m_Agent); 
        }

        public void Init(Transform target)
        {
            m_Target = target;
            m_Agent.SetDestination(target.position);
        }

        public void Damage(int damage)
        {
            SelfDespawn();
        }

        private void SelfDespawn()
        {
            if (HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("GameController"))
            {
                return;
            }

            IDamagable damagable = other.GetComponentInParent<IDamagable>();

            if(damagable is not null)
            {
                damagable.Damage(1);
                SelfDespawn();
            }
        }
    }
}