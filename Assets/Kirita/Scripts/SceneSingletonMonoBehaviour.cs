using UnityEngine;
using System;

/// <summary>
/// MonoBehaviour���p�������V�[���X�R�[�v�̃V���O���g���p�^�[��
/// </summary>
/// <typeparam name="T">�p����̌^</typeparam>
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
                    Debug.Log($"{typeof(T)}�̃C���X�^���X��������܂���: {instance.name}");
#endif
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"�V�[������{typeof(T)}�̃C���X�^���X��������܂���ł����B�V�����C���X�^���X�𐶐����܂��B");
#endif
                    Type t = typeof(T);
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// �C���X�^���X�����݂��邩�m�F���܂��B
    /// </summary>
    /// <returns></returns>
    static public bool IsInstance()
    {
        return instance != null;
    }

    /// <summary>
    /// �C���X�^���X��j�����܂��B
    /// </summary>
    static void DestroyInstance()
    {
        if (instance != null)
        {
#if UNITY_EDITOR
            Debug.Log($"{typeof(T)}�̃C���X�^���X���j������܂����B");
#endif
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    virtual protected void Awake()
    {
        //���ɐ�������Ă��邩���ׂ�
        if (CheckInstance())
        {
#if UNITY_EDITOR
            Debug.Log($"{typeof(T)}�̃C���X�^���X�͊��ɂ���܂�");
#endif
        }
    }
    virtual protected void OnDestroy()
    {
        if (instance == this)
        {
#if UNITY_EDITOR
            Debug.Log($"{typeof(T)} �̃C���X�^���X���j������܂����B");
#endif
            instance = null;
        }
    }

    /// <summary>
    /// �C���X�^���X�����݂��邩�m�F���A���݂��Ȃ��ꍇ�͐V�����C���X�^���X�𐶐����܂��B
    /// </summary>
    /// <returns>instance������ꍇtrue</returns>
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
        Debug.LogError($"{typeof(T)}�̃C���X�^���X�����ɑ��݂��܂��B{this.gameObject.name}��j�����܂��B");
#endif
        Destroy(this.gameObject);
        return false;
    }
}
