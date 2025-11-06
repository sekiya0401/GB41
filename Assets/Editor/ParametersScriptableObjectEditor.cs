#if UNITY_EDITOR
using MS.Games;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Parameters),true)]
public class ParametersScriptableObjectEditor : Editor
{
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
        var so = (Parameters)target;

        if(GUILayout.Button("🔁パラメータの同期"))
        {
            so.SyncParameters();
        }
        EditorGUILayout.Space();
    }
}

#endif