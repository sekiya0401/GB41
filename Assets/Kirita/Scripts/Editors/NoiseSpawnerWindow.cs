using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class NoiseSpawnerWindow : EditorWindow
{
    GameObject m_Prefab;
    Transform m_ParentTransform;

    Vector3 m_AreaMin = new Vector3(-10, 0, -10);
    Vector3 m_AreaMax = new Vector3(10, 0, 10);
    int m_Density = 20; // サンプリング点数（多いほど密度アップ）

    float m_NoiseScale = 1f;
    float m_Threshold = 0.5f;
    Vector2 m_NoiseOffset = Vector2.zero;

    Vector3 m_StartOffset = Vector3.zero;
    Vector3 m_BaseRotation = Vector3.zero;
    Vector3 m_RandomRotationRange = Vector3.zero;
    Vector3 m_BaseScale = Vector3.one;
    Vector3 m_RandomScaleRange = Vector3.zero;

    bool m_ClearBeforeSpawn = true;

    [MenuItem("Tools/Noise Spawner (Area Mode)")]
    public static void ShowWindow()
    {
        GetWindow<NoiseSpawnerWindow>("Noise Spawner (Area)");
    }

    void OnGUI()
    {
        GUILayout.Label("📦 基本設定", EditorStyles.boldLabel);
        m_Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", m_Prefab, typeof(GameObject), false);
        m_ParentTransform = (Transform)EditorGUILayout.ObjectField("Parent Transform", m_ParentTransform, typeof(Transform), true);

        m_AreaMin = EditorGUILayout.Vector3Field("Area Min", m_AreaMin);
        m_AreaMax = EditorGUILayout.Vector3Field("Area Max", m_AreaMax);
        m_Density = EditorGUILayout.IntSlider("Density", m_Density, 1, 200);

        GUILayout.Space(10);
        GUILayout.Label("🌊 ノイズ設定", EditorStyles.boldLabel);
        m_NoiseScale = EditorGUILayout.FloatField("Noise Scale", m_NoiseScale);
        m_Threshold = EditorGUILayout.Slider("Threshold", m_Threshold, 0f, 1f);
        m_NoiseOffset = EditorGUILayout.Vector2Field("Noise Offset", m_NoiseOffset);

        GUILayout.Space(10);
        GUILayout.Label("🎨 オブジェクト設定", EditorStyles.boldLabel);
        m_StartOffset = EditorGUILayout.Vector3Field("Start Offset", m_StartOffset);
        m_BaseRotation = EditorGUILayout.Vector3Field("Base Rotation", m_BaseRotation);
        m_RandomRotationRange = EditorGUILayout.Vector3Field("Random Rot ±", m_RandomRotationRange);
        m_BaseScale = EditorGUILayout.Vector3Field("Base Scale", m_BaseScale);
        m_RandomScaleRange = EditorGUILayout.Vector3Field("Random Scale ±", m_RandomScaleRange);

        GUILayout.Space(10);
        m_ClearBeforeSpawn = EditorGUILayout.Toggle("Clear Before Spawn", m_ClearBeforeSpawn);

        GUILayout.Space(15);
        if (GUILayout.Button("🚀 Spawn Objects", GUILayout.Height(30)))
        {
            SpawnObjects();
        }
        if (GUILayout.Button("🧹 Clear Spawned", GUILayout.Height(25)))
        {
            ClearSpawned();
        }
    }

    void SpawnObjects()
    {
        if (m_Prefab == null)
        {
            Debug.LogWarning("⚠️ Prefabが設定されていません。");
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        if (m_ClearBeforeSpawn)
            ClearSpawned();

        // 範囲サイズを算出
        float width = m_AreaMax.x - m_AreaMin.x;
        float depth = m_AreaMax.z - m_AreaMin.z;

        for (int i = 0; i < m_Density; i++)
        {
            // 範囲内のランダム座標
            float posX = Random.Range(m_AreaMin.x, m_AreaMax.x);
            float posZ = Random.Range(m_AreaMin.z, m_AreaMax.z);

            float sampleX = (posX + m_NoiseOffset.x) * m_NoiseScale;
            float sampleY = (posZ + m_NoiseOffset.y) * m_NoiseScale;
            float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

            if (noiseValue > m_Threshold)
            {
                Vector3 basePos = new Vector3(posX, m_AreaMin.y, posZ) + m_StartOffset;

                // --- 回転 ---
                Vector3 rot = m_BaseRotation + new Vector3(
                    Random.Range(-m_RandomRotationRange.x, m_RandomRotationRange.x),
                    Random.Range(-m_RandomRotationRange.y, m_RandomRotationRange.y),
                    Random.Range(-m_RandomRotationRange.z, m_RandomRotationRange.z)
                );

                // --- スケール ---
                Vector3 scale = m_BaseScale + new Vector3(
                    Random.Range(-m_RandomScaleRange.x, m_RandomScaleRange.x),
                    Random.Range(-m_RandomScaleRange.y, m_RandomScaleRange.y),
                    Random.Range(-m_RandomScaleRange.z, m_RandomScaleRange.z)
                );

                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(m_Prefab);
                Undo.RegisterCreatedObjectUndo(go, "Spawn Prefab");

                go.transform.position = basePos;
                go.transform.rotation = Quaternion.Euler(rot);
                go.transform.localScale = scale;

                if (m_ParentTransform != null)
                    go.transform.SetParent(m_ParentTransform);
            }
        }

        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        SceneView.RepaintAll();
    }

    void ClearSpawned()
    {
        if (m_ParentTransform != null)
        {
            for (int i = m_ParentTransform.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(m_ParentTransform.GetChild(i).gameObject);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        SceneView.RepaintAll();
    }
}
