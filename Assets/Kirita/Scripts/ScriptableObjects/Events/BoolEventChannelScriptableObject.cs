using UnityEngine;

namespace MS.SO.EventChannel
{
    [CreateAssetMenu(fileName = "BoolEventChannel", menuName = "Scriptable Objects/EventChannel/Bool")]
    public class BoolEventChannel : EventChannelScriptableObject<bool> { }
}