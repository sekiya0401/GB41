#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
	private EnemyData m_Loader;
	private static readonly HttpClient m_Client = new HttpClient();

	public async override void OnInspectorGUI()
	{
		// デフォルトのInspector表示
		DrawDefaultInspector();

		m_Loader = (EnemyData)target;

		// 更新ボタンの表示
		GUILayout.Space(5);
		if (GUILayout.Button("🔄データを更新", GUILayout.Height(30)))
		{
			if(m_Loader == null)
			{
				Debug.LogWarning("⚠'EnemyData'が存在しません");
				return;
			}

			if (string.IsNullOrEmpty(m_Loader.m_SheetUrl))
			{
				Debug.LogWarning("❌スプレッドシートのURLが設定されていません");
				return;
			}

			Debug.Log("🔄データ取得中...");
			await GetAndProcess(m_Loader);
		}
	}

	public static async Task GetAndProcess(EnemyData loader)
	{
		try
		{
			string json = await m_Client.GetStringAsync(loader.m_SheetUrl);
			if (!string.IsNullOrEmpty(json) && json != "[]")
			{
				loader.ProcessJson(json);
				Debug.Log("✅データ取得完了");
			}
			else
			{
				Debug.LogWarning("❌シートに有効なデータがありません");
			}
		}
		catch (HttpRequestException e)
		{
			Debug.LogWarning($"HTTPエラー: {e.Message}");
		}
	}
}
#endif
