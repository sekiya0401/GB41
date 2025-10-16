using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prototype.Games.UI;
using UnityEngine.SceneManagement;

namespace Prototype.Games
{
    /// <summary>
    /// ゲームの起動とネットワークランナーの管理を行うランチャー
    /// HACK: 簡易的にSharedモードで起動する実装
    /// </summary>
    public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private NetworkRunner m_NetworkRunner;
        [SerializeField]
        private NetworkPrefabRef m_PlayerPrefab;
        [SerializeField]
        private NetworkPrefabRef m_SimpleSpawnerPrefab;
        [SerializeField]
        private Transform[] m_SpawnPoints;
        [SerializeField]
        private NetworkPrefabRef m_FestivalPrefab;
        [SerializeField]
        private Transform m_FestivalPoint;

        private async void Start()
        {
            //ネットワークランナーの生成
            var networkRunner = Instantiate(m_NetworkRunner);

            //GameLauncherをNetworkRunnerのコールバック対象に追加
            networkRunner.AddCallbacks(this);


            //ゲームスタート
            var result = await networkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared
            });
        }

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
        {
            if(player == runner.LocalPlayer)
            {
                //[0,0]を中心に半径5の円内部にランダムなポイントを設定
                var rand = UnityEngine.Random.insideUnitCircle * 5.0f;
                var spawnPoint = new Vector3(rand.x, 2f, rand.y);

                //プレイヤーの生成
                runner.Spawn(m_PlayerPrefab, spawnPoint, Quaternion.identity);

                if(runner.IsSharedModeMasterClient)
                {
                    var festival = runner.Spawn(m_FestivalPrefab, m_FestivalPoint.position, Quaternion.identity);
                    var spawner = runner.Spawn(m_SimpleSpawnerPrefab);
                    if(spawner.TryGetComponent(out SimpleSpawner simpleSpawner))
                    {
                        simpleSpawner.Init(festival.transform, m_SpawnPoints);
                    }
                }
            }
        }
        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    }
}