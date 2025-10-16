using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace Prototype.Games
{
    public class SimpleSpawner : NetworkBehaviour
    {
        [SerializeField, Range(1f,10f)]
        private float m_SpawnRadius;
        [SerializeField, Min(1f)]
        private float m_SpawnInterval;
        [SerializeField, Range(1f, 20f)]
        private int m_SpawnAmount;
        [SerializeField]
        private SimpleEnemy m_EnemyPrefab;

        private Transform m_Target;
        private Transform[] m_SpawnPoints;
        private List<SimpleEnemy> m_Enemies;
        private float m_Timer = 0;

        public override void Spawned()
        {
            if (!HasStateAuthority)
            {
                return;
            }

            m_Enemies = new List<SimpleEnemy>();
        }

        public override void FixedUpdateNetwork()
        {
            if(!HasStateAuthority)
            { 
                return; 
            }

            m_Timer += Runner.DeltaTime;
            if(m_Timer > m_SpawnInterval)
            {
                m_Timer = 0;
                SpawnEnemy();
            }
        }

        public void Init(Transform target, Transform[] points)
        {
            m_Target = target;
            m_SpawnPoints = points;
        }

        private void SpawnEnemy()
        {
            if(m_SpawnPoints == null || m_SpawnPoints.Length < 1)
            {
                return;
            }

            for(int i = 0; i<m_SpawnAmount ;i++ )
            {
                var point = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)];
                var position = point.position;
                var rand = UnityEngine.Random.insideUnitCircle * m_SpawnRadius;
                position.x += rand.x;
                position.y += rand.y;

                var enemy = Runner.Spawn(m_EnemyPrefab, position, Quaternion.identity);
                enemy.Init(m_Target);
                m_Enemies.Add(enemy);
            }
        }
    }
}
