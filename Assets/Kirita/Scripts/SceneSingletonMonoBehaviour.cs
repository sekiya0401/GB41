using UnityEngine;
using System;

/// <summary>
/// MonoBehaviourを継承したシーンスコープのシングルトンパターン
/// </summary>
/// <typeparam name="T">継承先の型</typeparam>
abstract public class SceneSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T i = FindFirstObjectByType<T>();
                if (i)
                {
                    instance = i;
#if UNITY_EDITOR
                    Debug.Log($"{typeof(T)}のインスタンスが見つかりました: {instance.name}");
#endif
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"シーン内に{typeof(T)}のインスタンスが見つかりませんでした。新しいインスタンスを生成します。");
#endif
                    Type t = typeof(T);
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// インスタンスが存在するか確認します。
    /// </summary>
    /// <returns></returns>
    static public bool IsInstance()
    {
        return instance != null;
    }

    /// <summary>
    /// インスタンスを破棄します。
    /// </summary>
    static void DestroyInstance()
    {
        if (instance != null)
        {
#if UNITY_EDITOR
            Debug.Log($"{typeof(T)}のインスタンスが破棄されました。");
#endif
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    virtual protected void Awake()
    {
        //既に生成されているか調べる
        if (CheckInstance())
        {
#if UNITY_EDITOR
            Debug.Log($"{typeof(T)}のインスタンスは既にあります");
#endif
        }
    }
    virtual protected void OnDestroy()
    {
        if (instance == this)
        {
#if UNITY_EDITOR
            Debug.Log($"{typeof(T)} のインスタンスが破棄されました。");
#endif
            instance = null;
        }
    }

    /// <summary>
    /// インスタンスが存在するか確認し、存在しない場合は新しいインスタンスを生成します。
    /// </summary>
    /// <returns>instanceがある場合true</returns>
    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else if (instance == this)
        {
            return true;
        }

#if UNITY_EDITOR
        Debug.LogError($"{typeof(T)}のインスタンスが既に存在します。{this.gameObject.name}を破棄します。");
#endif
        Destroy(this.gameObject);
        return false;
    }
}
