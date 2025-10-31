using UnityEngine;

namespace MS.Games
{
    /// <summary>
    /// プレイヤーアクションインタフェース
    /// </summary>
    public interface IPlayerAction
    {
        void Action(Player _player);
    }
}