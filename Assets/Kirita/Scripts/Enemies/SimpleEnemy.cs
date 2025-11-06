using UnityEngine;
using UnityEngine.AI;
using MS.SO.Variable;

namespace MS.Games
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimpleEnemy : MonoBehaviour
    {
        [SerializeField]
        private FloatVariable m_DespawnSpeedThreshold;
        [SerializeField]
        private FloatVariable m_DespawnDelay;
        private float m_DespawnTime = 0f;
        private NavMeshAgent m_Agent;
        private Transform m_Target;
        private BlockSpawnPoint m_SpawnPoint;

        public void Spawned(Transform target, BlockSpawnPoint spawnPoint)
        {
            m_Target = target;
            m_SpawnPoint = spawnPoint;

            m_Agent.SetDestination(m_Target.position);
        }

        private void Update()
        {
            if (m_Agent.velocity.magnitude < m_DespawnSpeedThreshold.Value)
            {
                m_DespawnTime += Time.deltaTime;
                if (m_DespawnTime >= m_DespawnDelay.Value)
                {
                    SelfDespawn();
                }
            }
            else
            {
                m_DespawnTime = 0f;
            }

            if (m_Agent.hasPath && m_Agent.remainingDistance < m_Agent.stoppingDistance)
            {
                SelfDespawn();
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

        private void SelfDespawn()
        {
            Debug.Log("SelfDespawn");
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            m_SpawnPoint.Remove(this);
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying)
            {
                return; 
            }

            Gizmos.color = Color.magenta;
            for (int i = 0; i < m_Agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(m_Agent.path.corners[i], m_Agent.path.corners[i + 1]);
            }
            Gizmos.DrawLine(m_Agent.path.corners[m_Agent.path.corners.Length - 1], m_Agent.pathEndPosition);
        }
    }
}