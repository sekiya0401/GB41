using UnityEngine;


namespace SisimaiProt
{
    public class FryEnemyShot : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_EnemyBullet;
        public void Shot(Transform TargetPos)
        {
            Vector3 targetVec = TargetPos.position - transform.position;
            this.transform.rotation = Quaternion.LookRotation(targetVec); 
            GameObject bullet = Instantiate(m_EnemyBullet, transform.position, transform.rotation);
            
        }
    }
}
