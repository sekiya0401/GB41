using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MS.Extensions;

namespace MS.Games
{
    public class SampleLobby : MonoBehaviour
    {
        private const string HOST_SERVER_IP = "0.0.0.0";

        [SerializeField, Range(0, 10), EditAvailability(EditAvailabilityMode.EditModeOnly)]
        private int m_MaxConnections = 4;

        [Header("Buttons")]
        [SerializeField] 
        private Button m_HostButton;
        [SerializeField] 
        private Button m_ClientButton;
        [SerializeField] 
        private Button m_ServerButton;
        [SerializeField] 
        private Button m_StartButton;

        [Header("Network Settings")]
        [SerializeField] 
        private Toggle m_IsUseIP;
        [SerializeField] 
        private TMP_InputField m_IPInputField;
        [SerializeField]
        private TMP_InputField m_PortInputField;

        [Header("UI")]
        [SerializeField] 
        private Image m_LobbyHUD;
        [SerializeField] 
        private Image m_HostServerStandbyHUD;
        [SerializeField]
        private TextMeshProUGUI m_ConnectionPlayerCountText;
        [SerializeField] 
        private Image m_ClientStandbyHUD;

#if UNITY_EDITOR
        [Header("Scene")]
        [SerializeField]
        private SceneAsset m_LoadScene;
#endif
        [SerializeField,HideInInspector]
        private string m_LoadSceneName;

        private void Start()
        {
            // 待機UIの初期化
            m_HostServerStandbyHUD.gameObject.SetActive(false);
            m_ClientStandbyHUD.gameObject.SetActive(false);

            // ボタンイベント登録
            m_HostButton?.onClick.AddListener(OnStartHost);
            m_ClientButton?.onClick.AddListener(OnStartClient);
            m_ServerButton?.onClick.AddListener(OnStartServer);
            m_StartButton?.onClick.AddListener(OnStartSceneLoad);
        }


        /// <summary>
        /// ホストとしてセッションを構築
        /// </summary>
        private void OnStartHost()
        {
            if (!CheckConnectionPreparation(false)) return;

            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            NetworkManager.Singleton.StartHost();

            // クライアント接続監視
            NetworkManager.Singleton.OnClientConnectedCallback += OnCountConnection;
            CountConnection();

            // UI更新
            m_LobbyHUD.gameObject.SetActive(false);
            m_HostServerStandbyHUD.gameObject.SetActive(true);
        }

        /// <summary>
        /// クライアントとしてセッションに参加
        /// </summary>
        private void OnStartClient()
        {
            if (!CheckConnectionPreparation(true)) return;

            NetworkManager.Singleton.StartClient();

            // UI更新
            m_LobbyHUD.gameObject.SetActive(false);
            m_ClientStandbyHUD.gameObject.SetActive(true);
        }

        /// <summary>
        /// サーバー専用モードでセッションを構築
        /// </summary>
        private void OnStartServer()
        {
            if (!CheckConnectionPreparation(false)) return;

            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            NetworkManager.Singleton.StartServer();

            // クライアント接続監視
            NetworkManager.Singleton.OnClientConnectedCallback += OnCountConnection;
            CountConnection();

            // UI更新
            m_LobbyHUD.gameObject.SetActive(false);
            m_HostServerStandbyHUD.gameObject.SetActive(true);
        }


        /// <summary>
        /// ホスト/サーバがゲームシーンを開始する
        /// </summary>
        private void OnStartSceneLoad()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnCountConnection;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    // サーバ自身はプレイヤー生成対象外
                    if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost && clientId == NetworkManager.ServerClientId)
                    {
                        continue;
                    }

                    var client = NetworkManager.Singleton.ConnectedClients[clientId];
                    if (client.PlayerObject != null)
                    {
                        Debug.Log($"Client {clientId} は既にプレイヤーが生成されています。");
                        continue;
                    }

                    GameObject playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
                    if (playerPrefab == null)
                    {
                        Debug.LogError("PlayerPrefab が NetworkManager に設定されていません。");
                        continue;
                    }

                    // スポーン位置設定
                    Vector2 randXZ = Random.insideUnitCircle * 10f;
                    Vector3 position = new Vector3(randXZ.x, 2f, randXZ.y);

                    // プレイヤー生成 & クライアントに紐付け
                    var playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
                    playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                }
            }

            // シーン遷移（全クライアントへ同期）
            NetworkManager.Singleton.SceneManager.LoadScene(m_LoadSceneName, LoadSceneMode.Single);
        }


        private void OnCountConnection(ulong obj) => CountConnection();

        private void CountConnection()
        {
            m_ConnectionPlayerCountText.text = $"参加人数: {NetworkManager.Singleton.ConnectedClients.Count}/{m_MaxConnections}";
        }


        /// <summary>
        /// 接続に必要な設定が揃っているか確認
        /// </summary>
        private bool CheckConnectionPreparation(bool isClient)
        {
            // シーン名がnullもしくは空の場合処理を抜ける
            if (string.IsNullOrEmpty(m_LoadSceneName))
            {
                return false;
            }

            // NOTE:IPを使わない場合PC内接続なので、ベースの設定をそのままつかう
            if (!m_IsUseIP.isOn)
            {
                return true;
            }

            // トランスポートチェック
            if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport unityTransport)
            {
                if (!isClient)
                {
                    unityTransport.ConnectionData.Address = HOST_SERVER_IP;
                    return true;
                }

                if (string.IsNullOrEmpty(m_IPInputField.text))
                {
                    return true;
                }

                if (IsFirstCharOtherThanNumbers())
                {
                    return false;
                }

                //ポートが設定されていたら
                if(!string.IsNullOrEmpty(m_PortInputField.text))
                {
                    unityTransport.ConnectionData.Port = ushort.Parse(m_PortInputField.text);
                }

                unityTransport.ConnectionData.Address = m_IPInputField.text;
                return true;
            }

            Debug.LogWarning("NetworkManager の Transport には UnityTransport を設定してください。");
            return false;
        }

        /// <summary>
        /// IPアドレスの1文字目が数字以外か判定
        /// </summary>
        private bool IsFirstCharOtherThanNumbers()
        {
            char firstChar = m_IPInputField.text[0];
            if (firstChar == '-' || firstChar == '.')
            {
                Debug.LogWarning($"1文字目が '{firstChar}' のため、IPとして無効です。");
                return true;
            }
            return false;
        }


        /// <summary>
        /// 接続承認処理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Pending = true;

            // 最大接続数チェック
            if (NetworkManager.Singleton.ConnectedClients.Count >= m_MaxConnections)
            {
                response.Approved = false;
                response.Pending = false;
                return;
            }

            // 接続許可
            response.Approved = true;

            // プレイヤープレハブの自動生成設定
            // HACK: 今回はシーン遷移後に生成してほしいのでfalse
            response.CreatePlayerObject = false;

            // プレイヤープレハブのハッシュ値設定
            // NOTE: nullの場合NetworkManagerに登録したプレハブが使用される
            response.PlayerPrefabHash = null;

            response.Pending = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_LoadScene == null)
            {
                m_LoadSceneName = string.Empty;
                return;
            }
            m_LoadSceneName = m_LoadScene.name;
        }
#endif
    }
}