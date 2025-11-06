using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace MS.Games
{
    /// <summary>
    /// ゲーム起動制御
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField]
        private CameraRoot m_CameraRoot;
        [SerializeField]
        private TargetCamera m_TargetCamera;
        [SerializeField]
        private CinemachineCamera m_ServerCamera;

        private void Start()
        {
            StartCoroutine(AttachCamera());
        }

        private void OnDestroy()
        {
            if (IsNetworkManager)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        /// <summary>
        /// ネットワークマネージャーがある場合true
        /// </summary>
        private bool IsNetworkManager => NetworkManager.Singleton != null;

        /// <summary>
        /// カメラのあったい処理
        /// </summary>
        /// <returns></returns>
        private IEnumerator AttachCamera()
        {
            // NetworkManagerの存在を確認
            if (!IsNetworkManager)
            {
                Debug.LogWarning("NetworkManager が存在しません。");
                yield break;
            }

            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.ServerIsHost)
            {
                CinemachineBrain brain = FindAnyObjectByType<CinemachineBrain>();
                if (brain != null)
                {
                    brain.UpdateMethod = CinemachineBrain.UpdateMethods.SmartUpdate;
                }
                m_ServerCamera.Priority = 100;
                yield break;
            }

            //所有権を持ったプレイヤーを探す
            var localClient = NetworkManager.Singleton.LocalClient;
            if (localClient == null)
            {
                yield break;
            }

            yield return new WaitUntil(() => localClient.PlayerObject != null);

            //プレイヤーオブジェクトの取得
            GameObject localPlayer = localClient.PlayerObject.gameObject;

            //カメラのアタッチ
            m_CameraRoot.transform.SetParent(localPlayer.transform, false);
            m_TargetCamera.transform.SetParent(localPlayer.transform, false);
        }
    }
}
