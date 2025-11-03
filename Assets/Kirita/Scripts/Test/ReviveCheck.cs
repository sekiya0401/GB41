using Fusion;
using MS.Systems;
using System.Collections;
using TMPro;
using UnityEngine;

namespace MS.Games
{
    public class ReviveCheck : MonoBehaviour, IActivatable
    {
        [SerializeField]
        private Color m_LiveColor;
        [SerializeField]
        private Color m_DeadColor;
        [SerializeField]
        private TextMeshPro m_TextMeshPro;
        private MeshRenderer m_MeshRenderer;
        private Coroutine m_Coroutine;

        private void Awake()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();

            Disable();
        }

        public void Disable()
        {
            m_MeshRenderer.material.color = m_DeadColor;

            m_TextMeshPro.text = "DEAD";

            enabled = false;
        }

        public void Enable()
        {
            m_MeshRenderer.material.color= m_LiveColor;

            m_TextMeshPro.text = "LIVE";

            if (m_Coroutine is not null)
            {
                StopCoroutine(m_Coroutine);
            }

            m_Coroutine = StartCoroutine(Death());

            enabled = true;
        }

        public bool IsDisabled()
        {
            return enabled ? false : true;
        }

        public bool IsEnabled()
        {
            return enabled ? true : false;
        }

        private IEnumerator Death()
        {
            yield return new WaitForSeconds(1f);
            Disable();
        }
    }
}