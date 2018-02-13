// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-13
// Time: 14:27
// Author: Karsion

using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; private set; }

    /// <summary>
    ///     第一 初始化
    /// </summary>
    private void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = GetComponent<T>();
        if (!transform.parent)
        {
            DontDestroyOnLoad(gameObject);
        }

        OnSingletonInit();
    }

    /// <summary>
    ///     初始化
    /// </summary>
    protected virtual void OnSingletonInit()
    {
    }
}