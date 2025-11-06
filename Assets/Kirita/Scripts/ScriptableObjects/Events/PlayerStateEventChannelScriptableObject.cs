using UnityEngine;

namespace MS.SO.EventChannel
{
    [CreateAssetMenu(fileName = "PlayerStateEventChannel", menuName = "Scriptable Objects/EventChannel/PlayerState")]
    public class PlayerStateEventChannel : EventChannelScriptableObject<(ulong playerID,bool idDead)> { }
}
