using UnityEngine;

public static class JsonHelper
{
	public static T[] FromJson<T>(string json)
	{
		string wrapped = "{\"m_Items\":" + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
		return wrapper.m_Items;
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] m_Items;
	}
}