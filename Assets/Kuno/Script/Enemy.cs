using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace SisimaiProt
{
    public class Enemy : MonoBehaviour,IDamagable,INavMeshRecivable
    {

        [SerializeField]
        private NavMeshAgent m_Agent;
        [SerializeField]
        private NavMeshSurface m_Surface;
        [SerializeField]
        private Transform m_Target;
        private Transform m_Core;

        //private bool m_AttackFrag = false;

        [SerializeField]
        private int m_hp = 1;

        [SerializeField]
        private float m_MaxDistance = 2;
        private float m_RepathInterval = 0.1f;
        float m_RepathTimer = 0;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(m_Surface == null)
            {

                //m_Surface.navMeshData = new NavMeshData(m_Surface.agentTypeID);
                m_Surface.AddData();
                m_Surface.collectObjects = CollectObjects.All;

            }
            if (m_Core == null)
            {
                m_Core = m_Target;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if(m_hp <= 0)
            {
                Destroy(this.gameObject);
            }


            if (m_Agent.remainingDistance == 0)
            {
                //攻撃ステートに移行
                //m_AttackFrag = true;
            }

            m_RepathTimer += Time.deltaTime;
            if (m_RepathTimer >= m_RepathInterval)
            {
                m_RepathTimer = 0;
                m_Surface.UpdateNavMesh(m_Surface.navMeshData);

                Chase();
            }


        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Finish"))
            {
                collision.gameObject.GetComponent<IDamagable>().Damage(1);
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("誰だお前は！");
                m_Target = other.gameObject.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("もう行かなきゃ");
                m_Target = m_Core.transform;
            }
        }

        void Chase()
        {
            if (!m_Agent.isOnNavMesh)
            {
                Debug.LogWarning("isOnNavMeshがfalseです。");
                return;
            }

            if (NavMesh.SamplePosition(m_Target.position, out var hit, m_MaxDistance, NavMesh.AllAreas))
            {
                m_Agent.ResetPath();
                m_Agent.isStopped = false;
                m_Agent.SetDestination(hit.position);
            }
        }

        public void Damage(int damage)
        {
            m_hp -= damage;
        }
        public void SetNavMeshSurface(NavMeshSurface navMeshSurface, Transform transform)
        {
            m_Surface = navMeshSurface;
            m_Target = transform;
            m_Core = m_Target;
        }
    }

}

