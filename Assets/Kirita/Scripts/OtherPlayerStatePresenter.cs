using UnityEngine;

namespace Prototype.Games.UI
{
    /// <summary>
    /// 他プレイヤーのステータスとUIの制御
    /// </summary>
    public class OtherPlayerStatePresenter : MonoBehaviour
    {
        [SerializeField]
        private OtherPlayerStateView[] m_OtherPlayerView;

        public void ConnectPlayer(Player player)
        {
            foreach(var view in m_OtherPlayerView)
            {
                if(view.gameObject.activeSelf)
                {
                    continue;
                }    

                view.gameObject.SetActive(true);
                view.Attach(player);
                return;

            }
        }

        public void Disconnect(Player player)
        {
            foreach (var view in m_OtherPlayerView)
            {
                if(view.gameObject.activeSelf && view.Player.Id == player.Id)
                {
                    view.Detach();
                    view.gameObject.SetActive(false);
                }
            }
        }
    }
}