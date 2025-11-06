using UnityEngine;
using System.Collections.Generic;

namespace MS.Games
{
    public class SimpleSpawner : MonoBehaviour
    {
        [Header("基本設定")]
        [Tooltip("スポーンさせる間隔")]
        [SerializeField, Min(0)]
        private int m_Interval = 0;
        [Tooltip("起動時にスポーンさせるかどうか")]
        [SerializeField]
        private bool m_StartSpawn = false;
        [Tooltip("実際にスポーンを担当するスポーンブロックの配列")]
        [SerializeField]
        private BlockSpawnPoint[] m_BlockSpawnPoints;
        [Tooltip("スポーンさせる限界数、0にするとスポーン停止")]
        [SerializeField, Range(0, 128)]
        private int m_MaxSpawnCount = 60;

        [Header("デバック")]
        [Tooltip("優先的にスポーンさせる番号(-1もしくは配列のサイズより大きい場合は通常のランダムスポーンになる)")]
        [SerializeField, Min(-1)]
        private int m_PrioritySpawn = -1;
        [Tooltip("配列順にスポーンさせるかどうか")]
        [SerializeField]
        private bool m_IsOrder = false;

        private float m_SpawnTime;
        private int m_OrderIndex = 0;

        public int MaxSpawnCount => m_MaxSpawnCount;
        public int TotalSpawnObjectCount
        {
            get;
            set;
        }

        [ContextMenu("GetBlockSpawnPointChildren")]
        private void GetSpawnPointChildren()
        {
            List<BlockSpawnPoint> children = new List<BlockSpawnPoint>();
            for(int i = 0; i < transform.childCount ; i++)
            {
                if(transform.GetChild(i).TryGetComponent(out BlockSpawnPoint point))
                {
                    children.Add(point);
                    point.Spawner = this;
                }
            }

            if(m_BlockSpawnPoints.Length > 0)
            {
                m_BlockSpawnPoints = null;
            }

            m_BlockSpawnPoints = children.ToArray();
        }

        private void Start()
        {
            foreach(var block in m_BlockSpawnPoints)
            {
                block.Spawner = this;
            }

            if (m_StartSpawn)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            BlockSpawnPoint block = null;
            if (m_PrioritySpawn < 0 || m_PrioritySpawn >= m_BlockSpawnPoints.Length)
            {
                if(m_IsOrder)
                {
                    block = m_BlockSpawnPoints[m_OrderIndex];
                    m_OrderIndex = m_OrderIndex + 1 >= m_BlockSpawnPoints.Length ? 0 : m_OrderIndex + 1;
                }
                else
                {
                    block = m_BlockSpawnPoints[UnityEngine.Random.Range(0, m_BlockSpawnPoints.Length)];
                }
            }
            else
            {
                block = m_BlockSpawnPoints[m_PrioritySpawn];
            }


            block.Spawn();
        }

        private void Update()
        {
            if(m_MaxSpawnCount == 0)
            {
                m_SpawnTime = 0;
                return;
            }

            m_SpawnTime += Time.deltaTime;
            if (m_SpawnTime > m_Interval)
            {
                m_SpawnTime = 0;

                Spawn();
            }
        }

        private void OnGUI()
        {
            GUI.color = Color.cyan;
            GUI.skin.label.fontSize = 28;
            GUILayout.Label($"Total Object: {TotalSpawnObjectCount}");
        }
    }
}
