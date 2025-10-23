using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData")]
public class EnemyData : ScriptableObject
{
	[System.Serializable]
	public struct Entry
	{
		public string Name;
		public string HP;
		public string Attack;
	}

	[Header("�X�v���b�h�V�[�gURL")]
	[Tooltip("https://opensheet.elk.sh/�X�v���b�h�V�[�gID/�V�[�g��")]
	public string m_SheetUrl;

	[Tooltip("�p�����[�^�[�Q")]
	public List<Entry> m_Parameters;


#if UNITY_EDITOR
	/// <summary>
	/// Json�t�@�C����ǂݍ���Ńf�[�^���i�[
	/// </summary>
	public void ProcessJson(string json)
	{
		Entry[] loaded = JsonHelper.FromJson<Entry>(json);

		m_Parameters = new List<Entry>();

		for (int i = 0; i < loaded.Length; i++)
		{
			Entry entry = loaded[i];

			m_Parameters.Add(entry);

			Debug.Log($"Name:{entry.Name} HP:{entry.HP} ATK:{entry.Attack}");
		}

		EditorUtility.SetDirty(this);
	}
#endif
}
