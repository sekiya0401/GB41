using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ImageBasedBlockMapGenerator : EditorWindow
{
    [System.Serializable]
    public class ColorPrefabPair
    {
        public Color Color;
        public GameObject Prefab;
        public Vector3 Scale = Vector3.one;
    }

    public enum PIVOT_POINT
    {
        Center,
        Bottom,
        Top
    }

    Texture2D m_MapImage;
    Vector3 m_StartPosition = Vector3.zero;
    PIVOT_POINT m_PivotPoint = PIVOT_POINT.Bottom;

    enum ScaleMode { ScaleFactor, FixedSize }
    ScaleMode m_ScaleMode = ScaleMode.ScaleFactor;
    float m_MapScale = 1f;
    float m_MapWidth = 10f;
    float m_MapHeight = 10f;

    bool m_IsUseHeightFromBrightness = false;
    float m_HeightMultiplier = 5f;
    int m_PixelStep = 1;
    bool m_IsMergeBlocks = true;

    float m_ColorMergeThreshold = 30f; // 0〜255 スライダー値
    float ColorMergeThresholdNormalized => m_ColorMergeThreshold / 255f;
    float m_AlphaClip = 0.5f;

    List<ColorPrefabPair> m_MappingList = new List<ColorPrefabPair>();
    List<Color> m_DetectedColorList = new List<Color>();
    Dictionary<Color, int> m_ColorInstanceDictionary = new Dictionary<Color, int>();

    Vector2 m_MainScroll;
    Vector2 m_MappingScroll;

    [MenuItem("Tools/Image Based Block Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<ImageBasedBlockMapGenerator>("Image Based Block Map Generator");
    }

    void OnGUI()
    {
        m_MainScroll = EditorGUILayout.BeginScrollView(m_MainScroll);

        GUILayout.Label("🧱 イメージベースブロックマップ生成ツール", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        m_MapImage = (Texture2D)EditorGUILayout.ObjectField("マップイメージ", m_MapImage, typeof(Texture2D), false);
        if (EditorGUI.EndChangeCheck())
        {
            m_DetectedColorList.Clear();
            m_MappingList.Clear();
            Debug.Log("🧹 マップ画像が変更");
        }

        GUILayout.Label("ピクセル読み込み設定", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("この値が大きいほど、近い色を同一色としてまとめる(30以上推奨)", MessageType.Info);
        m_ColorMergeThreshold = EditorGUILayout.Slider("しきい値 (0–255)", m_ColorMergeThreshold, 0f, 255f);
        if (m_ColorMergeThreshold < 30f)
        {
            EditorGUILayout.HelpBox("画像によっては全ピクセル分の色をサンプルすることになるため、長時間かかる可能性あります！", MessageType.Warning);
        }
        m_AlphaClip = EditorGUILayout.Slider("アルファクリップ (0-1)", m_AlphaClip, 0f, 1f);
        m_PixelStep = EditorGUILayout.IntSlider("ピクセルステップ", m_PixelStep, 1, 16);
        GUI.Label(new Rect(0, 40, 100, 40), GUI.tooltip);

        GUILayout.Space(8);
        GUILayout.Label("座標設定", EditorStyles.boldLabel);
        m_StartPosition = EditorGUILayout.Vector3Field("開始座標(中心座標)", m_StartPosition);
        m_IsUseHeightFromBrightness = EditorGUILayout.Toggle("明度による高さ設定", m_IsUseHeightFromBrightness);
        if (m_IsUseHeightFromBrightness)
        {
            m_HeightMultiplier = EditorGUILayout.FloatField("高さ倍率", m_HeightMultiplier);
        }
        EditorGUILayout.HelpBox("オブジェクトを配置する際の基準位置を選択\nCenter=中央, Bottom=下端, Top=上端", MessageType.Info);
        m_PivotPoint = (PIVOT_POINT)EditorGUILayout.EnumPopup("ピボットポイント", m_PivotPoint);

        GUILayout.Space(8);
        GUILayout.Label("マップサイズ設定", EditorStyles.boldLabel);
        m_ScaleMode = (ScaleMode)EditorGUILayout.EnumPopup("スケールモード", m_ScaleMode);

        if (m_ScaleMode == ScaleMode.ScaleFactor)
        {
            EditorGUILayout.HelpBox("倍率指定：画像ピクセル数 × MapScale でマップ全体サイズを決定", MessageType.Info);
            m_MapScale = EditorGUILayout.Slider("倍率 (0-10)", m_MapScale, 0.1f, 10f);
        }
        else
        {
            EditorGUILayout.HelpBox("固定サイズ：マップ全体の幅と高さを直接指定", MessageType.Info);
            m_MapWidth = EditorGUILayout.FloatField("横幅 (X軸)", m_MapWidth);
            m_MapHeight = EditorGUILayout.FloatField("縦幅 (Z軸)", m_MapHeight);
        }


        GUILayout.Space(8);
        GUILayout.Label("ブロックサイズ設定", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("隣同士のピクセルで色がしきい値以下のものを1つのブロックにまとめるかどうか (原則ON)", MessageType.Info);
        m_IsMergeBlocks = EditorGUILayout.Toggle("同じ色の領域を結合", m_IsMergeBlocks);
        if(!m_IsMergeBlocks)
        {
            EditorGUILayout.HelpBox("膨大な数のブロックを生成するため、時間とメモリに相当な余裕が無い場合はOFFにしてください", MessageType.Warning);
        }

        EditorGUILayout.Space();

        if (m_MapImage != null && GUILayout.Button("🔍 色を検出する"))
        {
            EnsureTextureReadable(m_MapImage);
            AnalyzeColors();
        }

        if (m_DetectedColorList.Count > 0)
        {
            GUILayout.Label("🎨 検出された色", EditorStyles.boldLabel);
            DrawColorPalette();
        }

        EditorGUILayout.Space();
        GUILayout.Label("🎯 カラーマップ", EditorStyles.boldLabel);
        int removeIndex = -1;
        for (int i = 0; i < m_MappingList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");
            GUI.enabled = false;
            m_MappingList[i].Color = EditorGUILayout.ColorField(m_MappingList[i].Color, GUILayout.Width(80));
            GUI.enabled = true;
            m_MappingList[i].Prefab = (GameObject)EditorGUILayout.ObjectField(m_MappingList[i].Prefab, typeof(GameObject), false);

            if(m_IsMergeBlocks)
            {
                m_MappingList[i].Scale.y = EditorGUILayout.FloatField("ブロックの高さ", m_MappingList[i].Scale.y);
            }
            else
            {
                m_MappingList[i].Scale = EditorGUILayout.Vector3Field("ブロックスケール", m_MappingList[i].Scale);
            }

            m_MappingList[i].Scale = Vector3.Max(Vector3.one, m_MappingList[i].Scale);

            if (GUILayout.Button("🗑️", GUILayout.Width(30)))
            {
                removeIndex = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
            m_MappingList.RemoveAt(removeIndex);

        EditorGUILayout.Space();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🚀 ブロックマップを生成する", GUILayout.Height(40)))
        {
            if (m_MapImage != null)
                GenerateMap();
            else
                EditorUtility.DisplayDialog("Error", "マップ画像を割り当ててください", "OK");
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndScrollView();
    }

    // === カラー解析 ===
    void AnalyzeColors()
    {
        List<Color> allDetectedColorList = new List<Color>();
        var pixels = m_MapImage.GetPixels();
        HashSet<Color> unique = new HashSet<Color>(new ColorComparer(ColorMergeThresholdNormalized));

        foreach (var p in pixels)
        {
            if (p.a < m_AlphaClip) continue;
            unique.Add(RoundColor(p, 2));
        }

        allDetectedColorList.AddRange(unique);
        allDetectedColorList.Sort((a, b) => a.grayscale.CompareTo(b.grayscale));

        // マッピング可能カラーリストを生成
        m_DetectedColorList.Clear();
        foreach (var col in allDetectedColorList)
        {
            if (!m_DetectedColorList.Exists(c => ApproximatelyEqualColors(c, col)))
                m_DetectedColorList.Add(col);
        }

        Debug.Log($"🎨 検出された全ての色: {allDetectedColorList.Count}色 (しきい値: {ColorMergeThresholdNormalized})");
        Debug.Log($"🧩 マッピング可能な色の数: {m_DetectedColorList.Count}");
    }

    void DrawColorPalette()
    {
        int columns = 8;
        int rows = Mathf.CeilToInt(m_DetectedColorList.Count / (float)columns);
        float dynamicHeight = Mathf.Clamp(rows * 26f, 24f, 160f);
        m_MappingScroll = EditorGUILayout.BeginScrollView(m_MappingScroll, GUILayout.Height(dynamicHeight));
        for (int r = 0; r < rows; r++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < columns; c++)
            {
                int i = r * columns + c;
                if (i >= m_DetectedColorList.Count) break;
                var col = m_DetectedColorList[i];
                DrawColorBox(col);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("⚙️ すべての色をインポート"))
        {
            foreach (var col in m_DetectedColorList)
                AddMapping(col);
        }
    }

    void DrawColorBox(Color col, float padding = 2f)
    {
        Rect rect = GUILayoutUtility.GetRect(24, 24, GUILayout.Width(24), GUILayout.Height(24));
        Rect inner = new Rect(rect.x + padding, rect.y + padding, rect.width - padding * 2f, rect.height - padding * 2f);
        EditorGUI.DrawRect(inner, col);

        if (Event.current.type == EventType.MouseDown && inner.Contains(Event.current.mousePosition))
        {
            AddMapping(col);
            Event.current.Use();
        }
    }

    // === マップ生成 ===
    void GenerateMap()
    {
        EnsureTextureReadable(m_MapImage);
        m_ColorInstanceDictionary.Clear();

        int width = m_MapImage.width;
        int height = m_MapImage.height;
        var pixels = m_MapImage.GetPixels();

        float scaleX, scaleZ;
        if (m_ScaleMode == ScaleMode.ScaleFactor)
        {
            scaleX = scaleZ = m_MapScale;
        }
        else
        {
            scaleX = m_MapWidth / width;
            scaleZ = m_MapHeight / height;
        }

        Vector3 offset = new Vector3(width / 2f * scaleX, 0, height / 2f * scaleZ);
        GameObject parent = new GameObject("Generated_Block_Map");

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Generate 3D Block Map");

        bool[,] visited = new bool[width, height];

        for (int y = 0; y < height; y += m_PixelStep)
        {
            for (int x = 0; x < width; x += m_PixelStep)
            {
                if (visited[x, y]) continue;
                Color col = pixels[y * width + x];
                var match = FindMatch(col);
                if (match == null || match.Prefab == null) continue;

                Vector3 blockScale = m_IsMergeBlocks ? new Vector3(1f, match.Scale.y, 1f) : match.Scale;

                float yPos = m_IsUseHeightFromBrightness ? col.grayscale * m_HeightMultiplier : 0f;

                yPos += m_PivotPoint switch
                {
                    PIVOT_POINT.Bottom => blockScale.y * 0.5f,
                    PIVOT_POINT.Top => -blockScale.y * 0.5f,
                    PIVOT_POINT.Center => 0f,
                    _ => throw new InvalidOperationException($"未知の値が設定されています: {nameof(m_PivotPoint)}")
                };

                if (m_IsMergeBlocks)
                {
                    List<Vector2Int> area = new List<Vector2Int>();
                    Stack<Vector2Int> stack = new Stack<Vector2Int>();
                    stack.Push(new Vector2Int(x, y));
                    visited[x, y] = true;

                    while (stack.Count > 0)
                    {
                        Vector2Int p = stack.Pop();
                        area.Add(p);

                        foreach (var dir in new Vector2Int[]
                        {
                            new Vector2Int(1,0), new Vector2Int(-1,0),
                            new Vector2Int(0,1), new Vector2Int(0,-1)
                        })
                        {
                            int nx = p.x + dir.x * m_PixelStep;
                            int ny = p.y + dir.y * m_PixelStep;
                            if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                            if (visited[nx, ny]) continue;

                            Color nc = pixels[ny * width + nx];
                            if (ApproximatelyEqualColors(nc, col))
                            {
                                visited[nx, ny] = true;
                                stack.Push(new Vector2Int(nx, ny));
                            }
                        }
                    }

                    int minX = int.MaxValue, maxX = int.MinValue;
                    int minY = int.MaxValue, maxY = int.MinValue;
                    foreach (var p in area)
                    {
                        minX = Mathf.Min(minX, p.x);
                        maxX = Mathf.Max(maxX, p.x);
                        minY = Mathf.Min(minY, p.y);
                        maxY = Mathf.Max(maxY, p.y);
                    }

                    float sizeX = ((maxX - minX) / (float)m_PixelStep + 1) * scaleX;
                    float sizeZ = ((maxY - minY) / (float)m_PixelStep + 1) * scaleZ;

                    Vector3 center = m_StartPosition +
                        new Vector3((minX + maxX) / 2f * scaleX, yPos, (minY + maxY) / 2f * scaleZ) - offset;

                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(match.Prefab);
                    obj.transform.position = center;
                    obj.transform.localScale = new Vector3(sizeX * blockScale.x, blockScale.y, sizeZ * blockScale.z);
                    obj.transform.SetParent(parent.transform);
                    obj.name = GenerateInstanceName(match.Prefab.name, col);
                    Undo.RegisterCreatedObjectUndo(obj, "Placed Merged Block");
                }
                else
                {
                    Vector3 pos = m_StartPosition + new Vector3(x * scaleX, yPos, y * scaleZ) - offset;
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(match.Prefab);
                    obj.transform.position = pos;
                    obj.transform.localScale = Vector3.Scale(Vector3.one, blockScale) * Mathf.Max(scaleX, scaleZ);
                    obj.transform.SetParent(parent.transform);
                    obj.name = GenerateInstanceName(match.Prefab.name, col);
                    Undo.RegisterCreatedObjectUndo(obj, "Placed Block");
                    visited[x, y] = true;
                }
            }
        }

        EditorUtility.DisplayDialog("Success!", "ブロックマップの生成に成功しました 🎉", "OK");
    }

    // === ユーティリティ ===
    string GenerateInstanceName(string baseName, Color color)
    {
        if (!m_ColorInstanceDictionary.ContainsKey(color))
            m_ColorInstanceDictionary[color] = 1;
        else
            m_ColorInstanceDictionary[color]++;
        return $"{baseName}_{m_ColorInstanceDictionary[color]:D3}";
    }

    void AddMapping(Color color)
    {
        if (!m_MappingList.Exists(m => ApproximatelyEqualColors(m.Color, color)))
            m_MappingList.Add(new ColorPrefabPair { Color = color });
    }

    ColorPrefabPair FindMatch(Color c)
    {
        foreach (var m in m_MappingList)
            if (ApproximatelyEqualColors(m.Color, c)) return m;
        return null;
    }

    bool ApproximatelyEqualColors(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < ColorMergeThresholdNormalized &&
               Mathf.Abs(a.g - b.g) < ColorMergeThresholdNormalized &&
               Mathf.Abs(a.b - b.b) < ColorMergeThresholdNormalized;
    }

    Color RoundColor(Color c, int digits)
    {
        float f = Mathf.Pow(10, digits);
        return new Color(Mathf.Round(c.r * f) / f, Mathf.Round(c.g * f) / f, Mathf.Round(c.b * f) / f, 1f);
    }

    void EnsureTextureReadable(Texture2D tex)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
            Debug.Log($"✅ テクスチャの読み書きを有効化: {tex.name}");
        }
    }

    class ColorComparer : IEqualityComparer<Color>
    {
        private readonly float m_Tolerance;

        public ColorComparer(float tolerance)
        {
            m_Tolerance = tolerance;
        }

        public bool Equals(Color x, Color y)
        {
            return Mathf.Abs(x.r - y.r) < m_Tolerance &&
                   Mathf.Abs(x.g - y.g) < m_Tolerance &&
                   Mathf.Abs(x.b - y.b) < m_Tolerance;
        }
        public int GetHashCode(Color c)
        {
            int r = Mathf.RoundToInt(c.r * 255);
            int g = Mathf.RoundToInt(c.g * 255);
            int b = Mathf.RoundToInt(c.b * 255);
            return (r << 16) ^ (g << 8) ^ b;
        }
    }
}
