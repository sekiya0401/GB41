using UnityEngine;
using System.Collections.Generic;

namespace Prototype.Games
{
    public class BlockSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Transform[] m_SpawnPoints;
        [SerializeField]
        private SimpleEnemy m_SpawnedObjectRef;

        [SerializeField]
        private Transform m_Target;
        private List<SimpleEnemy> m_SpawnedObjectList = new List<SimpleEnemy>();

        [SerializeField]
        private IntReference m_SpawnNumberAtOnce;
        [SerializeField]
        private FloatReference m_Radius;

        public SimpleSpawner Spawner
        {
            get;
            set;
        }

        [ContextMenu("GetSpawnPointChildren")]
        private void GetSpawnPointChildren()
        {
            if(m_SpawnPoints.Length < 1)
            {
                m_SpawnPoints = new Transform[transform.childCount];
            }
            else
            {
                m_SpawnPoints = null;
                m_SpawnPoints = new Transform[transform.childCount];
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                m_SpawnPoints[i] = transform.GetChild(i);
            }
        }

        public void Spawn()
        {
            int spawnedCount = m_SpawnNumberAtOnce.Value * m_SpawnPoints.Length;
            if(spawnedCount + Spawner.TotalSpawnObjectCount > Spawner.MaxSpawnCount)
            {
                return;
            }

            foreach (Transform t in m_SpawnPoints)
            {
                for(int i = 0; i < m_SpawnNumberAtOnce.Value; i++)
                {
                    var pos = t.position + Random.insideUnitSphere * m_Radius.Value;
                    pos.y = t.position.y;

                    var enemy = Instantiate(m_SpawnedObjectRef, pos, Quaternion.identity);
                    enemy.Spawned(m_Target, this);
                    m_SpawnedObjectList.Add(enemy);
                }
            }

            Spawner.TotalSpawnObjectCount += spawnedCount;
        }

        public void Remove(SimpleEnemy enemy)
        {
            m_SpawnedObjectList.Remove(enemy);
            Spawner.TotalSpawnObjectCount--;
        }

        private void OnDrawGizmos()
        {
            if (m_SpawnPoints.Length < 1)
            {
                return;
            }

            Gizmos.color = Color.green;
            foreach (var t in m_SpawnPoints)
            {
                Gizmos.DrawWireSphere(t.position, m_Radius.Value);
            }
        }
    }
}
