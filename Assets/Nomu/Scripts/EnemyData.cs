using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData")]
public class EnemyData : ScriptableObject
{
	// NOTE:
	// Entry�ɒl��ǉ������ꍇ�AParameter�N���X�ɂ��������ڂ�ǉ���
	// ProcessJson()��(��95�s��)�̊i�[�������ǋL����K�v����B


	/// <summary>
	/// JSON����̓ǂݍ��ݗp�\����
	/// </summary>
	[System.Serializable]
	private struct Entry
	{
		public string Name;
		public string HP;
		public string Attack;
	}

	/// <summary>
	/// �ǂݍ��񂾃f�[�^�̊i�[�p�N���X
	/// </summary>
	[System.Serializable]
	public class Parameter
	{
		public string m_Name;
		public int m_HP;
		public int m_Attack;
	}

	[Header("�X�v���b�h�V�[�gURL")]
	[Tooltip("https://opensheet.elk.sh/�X�v���b�h�V�[�gID/�V�[�g��")]
	public string m_SheetUrl;

	[Tooltip("�p�����[�^�[�Q")]
	public List<Parameter> m_Parameters;


	/// <summary>
	/// �w�肵�����O�����p�����[�^�̎擾
	/// </summary>
	/// <param name="name">�������閼�O</param>
	/// <returns>��v����Parameter(������Ȃ��ꍇnull)</returns>
	public Parameter ParameterFind(string name)
	{
		foreach (Parameter entry in m_Parameters)
		{
			if(entry.m_Name == name)
			{
				return entry;
			}
		}
		return null;
	}

	/// <summary>
	/// �����Ŏw�肵�����O�̃p�����[�^�[�Q�̎擾
	/// </summary>
	/// <param name="name">�������閼�O</param>
	/// <returns>��v����Parameter���X�g(������Ȃ��ꍇ�󃊃X�g)</returns>
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
	/// JSON�������ǂݍ��݁A�f�[�^���i�[
	/// </summary>
	/// <param name="json">�擾����JSON������</param>
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

		// ScriptableObject�̕ύX���G�f�B�^�ɒʒm(�ۑ��Ώۂɂ���)
		EditorUtility.SetDirty(this);
	}
#endif
}
