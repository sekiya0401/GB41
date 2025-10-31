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

	/// <summary>
	/// InspectorのGUIのカスタマイズ
	/// </summary>
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

	/// <summary>
	/// シートURLを取得しJSONデータを処理
	/// </summary>
	/// <param name="loader">データを適用するEnemyData</param>
	public static async Task GetAndProcess(EnemyData loader)
	{
		try
		{
			// URLからJSON文字列を取得
			string json = await m_Client.GetStringAsync(loader.m_SheetUrl);

			if (!string.IsNullOrEmpty(json) && json != "[]")
			{
				// JSON文字列をEnemyDataのデータ形式に変換・適用
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
			// 通信エラー(ネットワーク、URL不正など)が発生した場合の例外処理
			Debug.LogWarning($"通信エラー: {e.Message}");
		}
	}
}
#endif
