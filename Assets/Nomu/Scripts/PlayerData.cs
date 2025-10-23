using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
	// NOTE:
	// Entryに値を追加した場合、Parameterクラスにも同じ項目を追加と
	// ProcessJson()内(約95行目)の格納処理も追記する必要あり。


	/// <summary>
	/// JSONからの読み込み用構造体
	/// </summary>
	[System.Serializable]
	private struct Entry
	{
		public string Name;
		public string HP;
		public string Attack;
	}

	/// <summary>
	/// 読み込んだデータの格納用クラス
	/// </summary>
	[System.Serializable]
	public class Parameter
	{
		public string m_Name;
		public int m_HP;
		public int m_Attack;
	}

	[Header("スプレッドシートURL")]
	[Tooltip("https://opensheet.elk.sh/スプレッドシートID/シート名")]
	public string m_SheetUrl;

	[Tooltip("パラメーター群")]
	public List<Parameter> m_Parameters;


	/// <summary>
	/// 指定した名前を持つパラメータの取得
	/// </summary>
	/// <param name="name">検索する名前</param>
	/// <returns>一致するParameter(見つからない場合null)</returns>
	public Parameter ParameterFind(string name)
	{
		foreach (Parameter entry in m_Parameters)
		{
			if (entry.m_Name == name)
			{
				return entry;
			}
		}
		return null;
	}

	/// <summary>
	/// 引数で指定した名前のパラメーター群の取得
	/// </summary>
	/// <param name="name">検索する名前</param>
	/// <returns>一致するParameterリスト(見つからない場合空リスト)</returns>
	public List<Parameter> ParametersFind(string name)
	{
		List<Parameter> result = new List<Parameter>();
		foreach (Parameter entry in m_Parameters)
		{
			if (entry.m_Name == name)
			{
				result.Add(entry);
			}
		}
		return result;
	}


#if UNITY_EDITOR
	/// <summary>
	/// JSON文字列を読み込み、データを格納
	/// </summary>
	/// <param name="json">取得したJSON文字列</param>
	public void ProcessJson(string json)
	{
		Entry[] loaded = JsonHelper.FromJson<Entry>(json);
		
		m_Parameters = new List<Parameter>();

		for (int i = 0; i < loaded.Length; i++)
		{
			Parameter param = new Parameter
			{ 
				m_Name = loaded[i].Name,
				m_HP = int.Parse(loaded[i].HP),
				m_Attack = int.Parse(loaded[i].Attack)
			};

			m_Parameters.Add(param);

			Debug.Log($"Name:{param.m_Name} HP:{param.m_HP} ATK:{param.m_Attack}");
		}

		// ScriptableObjectの変更をエディタに通知(保存対象にする)
		EditorUtility.SetDirty(this);
	}
#endif
}
