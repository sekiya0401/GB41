using MS.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MS.Games
{
    /// <summary>
    /// ゲーム全体制御
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        [Header("設定")]
        [SerializeField,Tooltip("秒")]
        private int m_Time = 300;
        [SerializeField]
        private bool m_IsPlaying = false;

        [Header("UI")]
        [SerializeField]
        private Image m_HUD;
        [SerializeField]
        private TextMeshProUGUI m_TimeTextField;

        [Header("入力")]
        [SerializeField]
        private InputActionReference m_MenuInput;

#if UNITY_EDITOR
        [Header("遷移先設定")]
        [SerializeField]
        private SceneAsset m_LobbyScene;
#endif
        [SerializeField, HideInInspector]
        private string m_LobbySceneName;

        private System.Diagnostics.Stopwatch m_StopWatch = new();
        private readonly List<Player> m_PlayerList = new();

        private NetworkVariable<int> m_ElapsedTime = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (!IsServer)
            {
                return;
            }

            m_ElapsedTime.OnValueChanged -= OnElapsedTime;

            m_MenuInput.action.Disable();
            m_MenuInput.action.RemovePhaseCallbacks(OnMenu, InputActionExtensions.PHASE.STARTED);

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }

        private async void Start()
        {
            // NetworkManagerが利用可能になるまで待機
            await WaitForNetworkReady();

            //コールバックの追加
            m_ElapsedTime.OnValueChanged += OnElapsedTime;

            //UIの初期化
            m_TimeTextField.text = $"{m_Time}";
            m_HUD.gameObject.SetActive(false);

            //メニューにアクセスするための入力処理の初期化
            m_MenuInput.action.Enable();
            m_MenuInput.action.AddPhaseCallbacks(OnMenu, InputActionExtensions.PHASE.STARTED);

            //サーバまたはホスト以外はここで終了
            if (!IsServer)
            {
                return;
            }

            //クライアント接続に関するコールバックの追加
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
 

            // サーバでなければプレイヤー登録
            if (IsServer && !ServerIsHost)
            {
                return;
            }

            RegisterPlayers();
        }

        private void OnMenu(InputAction.CallbackContext context)
        {
            bool invActive = !m_HUD.gameObject.activeSelf;

            m_HUD.gameObject.SetActive(invActive);
        }

        private void OnElapsedTime(int previousValue, int newValue)
        {
            if(m_TimeTextField)
            {
                m_TimeTextField.text = $"{newValue}";
            }
        }

        private void Update()
        {
            if(IsClient || !m_IsPlaying)
            {
                return;
            }

            // 秒単位に丸める
            int elapsedSeconds = (int)m_StopWatch.Elapsed.TotalSeconds;
            int remaining = Mathf.Max(0, m_Time - elapsedSeconds);

            if (remaining <= 0)
            {
                GameStop();
                return;
            }

            // 値が変化したときのみ更新
            if (remaining != m_ElapsedTime.Value)
            {
                m_ElapsedTime.Value = remaining;
            }
        }

        /// <summary>
        /// NetworkManagerが有効になるまで待機
        /// </summary>
        private async Task WaitForNetworkReady()
        {
            while (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// すでにスポーンしているプレイヤーをリストに登録
        /// </summary>
        private void RegisterPlayers()
        {
            m_PlayerList.Clear();
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject != null)
                {
                    var player = client.PlayerObject.GetComponent<Player>();
                    if (player != null)
                    {
                        m_PlayerList.Add(player);
                    }
                }
            }

            Debug.Log($"プレイヤー登録完了: {m_PlayerList.Count}人");
        }

        /// <summary>
        /// クライアント接続時のコールバック
        /// </summary>
        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            Debug.Log($"クライアント接続: {clientId}");

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            {
                if (client.PlayerObject != null)
                {
                    var player = client.PlayerObject.GetComponent<Player>();
                    if (player != null && !m_PlayerList.Contains(player))
                    {
                        m_PlayerList.Add(player);
                        Debug.Log($"プレイヤー追加: {clientId}");
                    }
                }
            }
        }

        /// <summary>
        /// クライアント切断時のコールバック
        /// </summary>
        private void OnClientDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            Debug.Log($"クライアント切断: {clientId}");

            m_PlayerList.RemoveAll(p => p != null && p.OwnerClientId == clientId);
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public void GameStart()
        {
            if(IsServer)
            {
                m_StopWatch.Start();
                m_IsPlaying = true;
            }
        }

        /// <summary>
        /// ゲーム終了
        /// </summary>
        public void GameStop()
        {
            if (IsServer)
            {
                m_StopWatch.Stop();
                m_IsPlaying = false;
            }
        }

        /// <summary>
        /// ゲーム再起動
        /// </summary>
        public void GameRestart()
        {
            if (IsServer)
            {
                m_StopWatch.Restart();
                m_IsPlaying = true;
            }
        }

        /// <summary>
        /// シャットダウン
        /// </summary>
        public async void Shutdown()
        {
            if (IsServer)
            {
                // クライアントに終了通知を送る
                NotifyShutdownRpc();

                // RPC送信のため少し待機（0.2秒ほどでOK）
                await Task.Delay(200);
            }
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(m_LobbySceneName);
        }

        /// <summary>
        /// クライアントにサーバ終了を通知
        /// </summary>
        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyShutdownRpc()
        {
            // サーバ自身では二重に処理しない
            if (IsServer)
            {
                return;
            }

            Debug.Log("サーバからシャットダウン通知を受け取りました。シーン遷移します。");

            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(m_LobbySceneName);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(m_LobbyScene == null)
            {
                return;
            }

            m_LobbySceneName = m_LobbyScene.name;
        }
#endif
    }
}