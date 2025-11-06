using UnityEngine;
using UnityEngine.UI;

namespace MS.Games.UI
{
    public class PlayerStateIcon : MonoBehaviour
    {
        [SerializeField]
        private Image m_PlayerIcon;
        [SerializeField]
        private Image m_DeadIcon;

        public Color PlayerIconColor
        {
            get => m_PlayerIcon.color;
            set => m_PlayerIcon.color = value;  
        }

        private void Start()
        {
            m_DeadIcon.gameObject.SetActive(false);
        }

        public void Revive()
        {
            m_DeadIcon.gameObject.SetActive(false);
        }

        public void Dead()
        {
            m_DeadIcon.gameObject.SetActive(true);
        }
    }
}