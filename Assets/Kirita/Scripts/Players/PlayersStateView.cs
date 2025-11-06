using MS.SO.EventChannel;
using Unity.Netcode;
using UnityEngine;

namespace MS.Games.UI
{
    public class PlayersStateView : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateEventChannel m_PlayerStateEventChannel;
        [SerializeField]
        private Color m_DisconnectColor;
        [SerializeField]
        private PlayerStateIcon[] m_PlayerStateIcons;

        private void Start()
        {
            int currentConnectionCount = NetworkManager.Singleton.ConnectedClients.Count;

            for (int i = 0; i < m_PlayerStateIcons.Length; i++)
            {
                if (i < currentConnectionCount)
                {
                    Random.InitState(i);
                    m_PlayerStateIcons[i].PlayerIconColor = Random.ColorHSV();
                }
                else
                {
                    m_PlayerStateIcons[i].PlayerIconColor = m_DisconnectColor;
                }
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
            }
        }

        private void OnEnable()
        {
            m_PlayerStateEventChannel.ChangedValue += OnStateChanged;
        }

        private void OnStateChanged((ulong playerID, bool isDead) info)
        {
            int index = (int)info.playerID;

            if(index >= m_PlayerStateIcons.Length)
            {
                return;
            }

            if(info.isDead)
            {
                m_PlayerStateIcons[index].Dead();
            }
            else
            {
                m_PlayerStateIcons[index].Revive();
            }
        }

        private void OnDisable()
        {
            m_PlayerStateEventChannel.ChangedValue -= OnStateChanged;
        }

        private void OnDestroy()
        {
            if(NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnConnected;
            }
        }

        private void OnConnected(ulong obj)
        {
            int currentConnectionCount = NetworkManager.Singleton.ConnectedClients.Count;
            int playersStateCount = m_PlayerStateIcons.Length;

            if(currentConnectionCount <= playersStateCount)
            {
                int index = currentConnectionCount - 1;
                Random.InitState(index);
                m_PlayerStateIcons[index].PlayerIconColor = Random.ColorHSV();
            }
        }
    }
}