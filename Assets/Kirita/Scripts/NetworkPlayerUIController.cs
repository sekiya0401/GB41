using UnityEngine;
using Prototype.ScriptableObjects;

namespace Prototype.Games
{
    /// <summary>
    /// �l�b�g���[�N�v���C���[��UI���Ǘ�����R���g���[���[
    /// HACK: �ȈՓI��3�l����UI��p�ӂ��Ă����A�ڑ����ꂽ���Ɋ��蓖�Ă����
    /// </summary>
    public class NetworkPlayerUIController : SceneSingletonMonoBehaviour<NetworkPlayerUIController>
    {
        [SerializeField]
        private NetworkPlayerView[] m_NetworkPlayerViews = new NetworkPlayerView[3];

        public bool ConnectPlayerView(string name, FloatEventChannelScriptableObject channel)
        {
            foreach (var view in m_NetworkPlayerViews)
            {
                if(view != null && !view.gameObject.activeSelf)
                {
                    view.gameObject.SetActive(true);
                    view.SetName(name);
                    view.SetHealthGaugeChannel(channel);
                    return true;
                }
            }

            return false;
        }

        public void DisconnectPlayerView(string name)
        {
            foreach (var view in m_NetworkPlayerViews)
            {
                if (view != null && view.gameObject.activeSelf && view.ID.Contains(name))
                {
                    view.SetName(string.Empty);
                    view.SetHealthGaugeChannel(null);
                    view.gameObject.SetActive(false);
                    return;
                }
            }
        }
    }

}