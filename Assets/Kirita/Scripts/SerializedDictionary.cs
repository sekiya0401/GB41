using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Systems
{
    /// <summary>
    /// Unityでシリアライズ可能なDictionary
    /// HACK: キーの重複を許容しない実装
    /// TODO: キーが0(null)の場合、インスペクターで編集できない問題の対処として、DefaultKeyを用意。カスタムエディタで対応予定。
    /// </summary>
    /// <typeparam name="K">キー</typeparam>
    /// <typeparam name="V">値</typeparam>
    [System.Serializable]
    public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [System.Serializable]
        internal class KeyValue
        {
            public K Key;
            public V Value;

            public KeyValue(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }
        [SerializeField] List<KeyValue> m_list;

        public virtual K DefaultKey => default;
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            foreach (var item in m_list)
            {
                this[ContainsKey(item.Key) ? DefaultKey : item.Key] = item.Value;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_list = new List<KeyValue>(Count);
            foreach (var kvp in this)
            {
                m_list.Add(new KeyValue(kvp.Key, kvp.Value));
            }
        }
    }
    [System.Serializable]
    public class SerializedDictionary<V> : SerializedDictionary<string, V>
    {
        public override string DefaultKey => string.Empty;
    }
    [System.Serializable]
    public class SerializedDictionaryC<K, V> : SerializedDictionary<K, V> where K : new()
    {
        public override K DefaultKey => new();
    }

    /*
     * 参考サイト
     * https://qiita.com/kat_out/items/98420ae6dcdfee58dd07
     */
}