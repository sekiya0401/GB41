using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Systems
{
    /// <summary>
    /// Unity�ŃV���A���C�Y�\��Dictionary
    /// HACK: �L�[�̏d�������e���Ȃ�����
    /// TODO: �L�[��0(null)�̏ꍇ�A�C���X�y�N�^�[�ŕҏW�ł��Ȃ����̑Ώ��Ƃ��āADefaultKey��p�ӁB�J�X�^���G�f�B�^�őΉ��\��B
    /// </summary>
    /// <typeparam name="K">�L�[</typeparam>
    /// <typeparam name="V">�l</typeparam>
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
     * �Q�l�T�C�g
     * https://qiita.com/kat_out/items/98420ae6dcdfee58dd07
     */
}