using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerHUD : MonoBehaviour
{
    [SerializeField]
    private Button m_HostButton;
    [SerializeField]
    private Button m_ClientButton;
    [SerializeField]
    private Button m_ServerButton;
    [SerializeField]
    private Button m_ShutdownButton;

    private void Start()
    {
        m_HostButton?.onClick.AddListener(OnStartHost);
        m_ClientButton?.onClick.AddListener(OnStartClient);
        m_ServerButton?.onClick.AddListener(OnStartServer);
        m_ShutdownButton?.onClick.AddListener(OnShutdown);

        Shutdown();
    }

    private void OnStartHost()
    {
        NetworkManager.Singleton.StartHost();
        Startup();
    }
    private void OnStartClient()
    {
        NetworkManager.Singleton.StartClient();
        Startup();
    }

    private void OnStartServer()
    {
        NetworkManager.Singleton.StartServer();
        Startup();
    }
    private void OnShutdown()
    {
        NetworkManager.Singleton.Shutdown();
        Shutdown();
    }

    private void Startup()
    {
        if(m_HostButton)
            m_HostButton.interactable = false;
        if(m_ClientButton)
            m_ClientButton.interactable = false;
        if(m_ServerButton)
            m_ServerButton.interactable = false;
        if(m_ShutdownButton)
            m_ShutdownButton.interactable = true;
    }

    private void Shutdown()
    {
        if (m_HostButton)
            m_HostButton.interactable = true;
        if (m_ClientButton)
            m_ClientButton.interactable = true;
        if (m_ServerButton)
            m_ServerButton.interactable = true;
        if (m_ShutdownButton)
            m_ShutdownButton.interactable = false;
    }
}
