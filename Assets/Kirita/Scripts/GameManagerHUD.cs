using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace MS.Games
{
    public class GameManagerHUD : MonoBehaviour
    {
        [Header("Host & Server")]
        [SerializeField]
        private Button m_GameStartButton;
        [SerializeField]
        private Button m_GameStopButton;
        [SerializeField]
        private Button m_GameRestartButton;
        [Header("ALL")]
        [SerializeField]
        private Button m_ShutdownButton;

        private async void Start()
        {
            await WaitForNetworkReady();

            // サーバもしくはホストでは無い場合、ボタンを非表示にする
            if(!NetworkManager.Singleton.IsServer)
            {
                m_GameStartButton.gameObject.SetActive(false);
                m_GameStopButton.gameObject.SetActive(false);
                m_GameRestartButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ネットワークマネージャーのインスタンスが出来るまで待機する処理
        /// </summary>
        /// <returns></returns>
        private async Task WaitForNetworkReady()
        {
            while (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                await Task.Yield();
            }
        }
    }

}