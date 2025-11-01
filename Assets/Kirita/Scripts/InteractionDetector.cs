using UnityEngine;
using System.Collections.Generic;
using MS.Systems;
using System.Linq;
using UnityEngine.WSA;
using UnityEngine.UIElements;

namespace MS.Games
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Origin;
        //private List<GameObject> m_ActivatableObjectList = new();
        private IActivatable m_ActivatableTarget;

        //Debug
        private RaycastHit m_RaycastHit;
        private float m_Radius = 3f;

        public bool IsActivatableTarget()
        {
            //return m_ActivatableObjectList.Count > 0 ? true : false;

            return m_ActivatableTarget == null ? false : true;
        }

        public void EnableActivatableTarget()
        {
            //m_ActivatableObjectList.OrderBy(go => Vector3.Distance(go.transform.position, this.transform.position));

            //IActivatable target = m_ActivatableObjectList[0].GetComponent<IActivatable>();
            //target.Enable();
        }

        private void FixedUpdate()
        {
            Transform origin = m_Origin == null ? transform : m_Origin;


            if (Physics.SphereCast(origin.position, m_Radius, origin.forward, out m_RaycastHit,10f))
            {
                if(m_RaycastHit.collider.TryGetComponent(out IActivatable activatable) &&
                    activatable.IsDisabled())
                {
                    m_ActivatableTarget = activatable;
                }
            }
            else
            {
                m_ActivatableTarget = null;
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.TryGetComponent(out IActivatable activatable) &&
        //        activatable.IsDisabled())
        //    {
        //        m_ActivatableObjectList.Add(other.gameObject);
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    m_ActivatableObjectList.Remove(other.gameObject);
        //}

        private void OnDrawGizmos()
        {
            if (m_ActivatableTarget != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(transform.position, transform.forward * m_RaycastHit.distance);
                Gizmos.DrawWireSphere(transform.position + transform.forward * (m_RaycastHit.distance), m_Radius);
            }
        }
    }
}