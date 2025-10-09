using System;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

namespace SisimaiProt 
{
    public class EnemySponer : MonoBehaviour
    {

        [SerializeField]
        private float m_CountDown = 180;

        [SerializeField]
        private float m_SponTimer = 0;
        [SerializeField]
        private float m_SponInterval = 30;

        [SerializeField]
        private float m_Range = 180;

        public NavMeshSurface m_Surface;
        private Transform m_Core;

        [SerializeField]
        private GameObject[] m_Enemys;
        [SerializeField]
        private int m_SponCount = 5;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_SponTimer = m_SponInterval;
            m_Core = this.gameObject.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_CountDown > 0)
            {
                m_CountDown -= Time.deltaTime;
                m_SponTimer -= Time.deltaTime;

                var span = new TimeSpan(0, 0, (int)m_CountDown);
                var timer = span.ToString(@"hh\:mm\:ss");
                //Debug.Log(timer);

                if(m_SponTimer <= 0)
                {
                    for (int i =0;i< m_SponCount;i++)
                    {
                        //Enemyê∂ê¨
                        Vector3 sponPos = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0.0f, UnityEngine.Random.Range(-1.0f, 1.0f)).normalized * m_Range;
                        sponPos = new Vector3(sponPos.x, 0.9f, sponPos.z);

                        var number = UnityEngine.Random.Range(0, m_Enemys.Length);
                        GameObject enemy = Instantiate(m_Enemys[number], sponPos, Quaternion.identity);

                        enemy.gameObject.GetComponent<INavMeshRecivable>().SetNavMeshSurface(m_Surface, m_Core);

                    }

                    m_SponInterval -= 0.2f;
                    m_SponTimer = m_SponInterval;

                }

            }
            else
            {
                Debug.Log("Ç®ÇÌÇË");
                //ÉVÅ[Éìà»ç~ÇÃèàóù

            }

        }
    }

}