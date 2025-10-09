using UnityEngine;

namespace SisimaiProt
{
    public class Bullet : MonoBehaviour
    {

        [SerializeField]
        private float m_Force = 5;

        void Start ()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * m_Force,ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                other.gameObject.GetComponent<IDamagable>().Damage(1);
                Destroy(this.gameObject);
            }
        }
    }

}