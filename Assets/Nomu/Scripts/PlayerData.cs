using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
	[System.Serializable]
	public struct Entry
	{
		public string m_Name;
		public string m_HP;
		public string m_Attack;
	}

	[Header("スプレッドシートURL")]
	[Tooltip("https://opensheet.elk.sh/スプレッドシートID/シート名")]
	public string m_SheetUrl;

	[Tooltip("パラメーター群")]
	public List<Entry> m_Parameters;


#if UNITY_EDITOR
	/// <summary>
	/// Jsonファイルを読み込んでデータを格納
	/// </summary>
	public void ProcessJson(string json)
	{
		Entry[] loaded = JsonHelper.FromJson<Entry>(json);

		m_Parameters = new List<Entry>();

		for (int i = 0; i < loaded.Length; i++)
		{
			Entry entry = loaded[i];

			m_Parameters.Add(entry);

			Debug.Log($"Name:{entry.m_Name} HP:{entry.m_HP} ATK:{entry.m_Attack}");
		}

		EditorUtility.SetDirty(this);
	}
#endif

}
