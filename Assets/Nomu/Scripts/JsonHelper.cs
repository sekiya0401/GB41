using UnityEngine;

public static class JsonHelper
{
	/// <summary>
	/// JSON文字列から指定した型の配列の作成
	/// </summary>
	/// <typeparam name="T">配列の要素の型</typeparam>
	/// <param name="json">JSON文字列</param>
	/// <returns>JSONから作ったT型の配列</returns>
	public static T[] FromJson<T>(string json)
	{
		string wrapped = "{\"m_Items\":" + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
		return wrapper.m_Items;
	}

	/// <summary>
	/// 配列データを一時的に格納するクラス。
	/// </summary>
	/// <typeparam name="T">配列の要素の型</typeparam>
	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] m_Items;
	}
}