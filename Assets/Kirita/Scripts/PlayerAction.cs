using UnityEngine;

namespace Prototype.Games
{
    /// <summary>
    /// プレイヤーアクションインタフェース
    /// </summary>
    public interface IPlayerAction
    {
        void Action(Player _player);
    }
}