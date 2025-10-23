using UnityEngine;

public static class JsonHelper
{
	/// <summary>
	/// JSON�����񂩂�w�肵���^�̔z��̍쐬
	/// </summary>
	/// <typeparam name="T">�z��̗v�f�̌^</typeparam>
	/// <param name="json">JSON������</param>
	/// <returns>JSON��������T�^�̔z��</returns>
	public static T[] FromJson<T>(string json)
	{
		string wrapped = "{\"m_Items\":" + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
		return wrapper.m_Items;
	}

	/// <summary>
	/// �z��f�[�^���ꎞ�I�Ɋi�[����N���X�B
	/// </summary>
	/// <typeparam name="T">�z��̗v�f�̌^</typeparam>
	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] m_Items;
	}
}