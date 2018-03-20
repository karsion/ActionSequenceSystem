// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-20 11:39
// ***************************************************************************

using System;
using UnityEngine;

public abstract class SingletonMonoAuto<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private bool _duplicateToDestroy;

    public static T instance
    {
        get
        {
            if (!_instance)
            {
                Type type = typeof(T);
                new GameObject(type.Name, type);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = GetComponent<T>();
            (_instance as SingletonMonoAuto<T>).OnSingletonInit();
            DontDestroyOnLoad(_instance.gameObject);
            return;
        }

        if (_instance != null && _instance != this)
        {
            _duplicateToDestroy = true;
            Destroy(gameObject);
            return;
        }

        (_instance as SingletonMonoAuto<T>).OnSingletonInit();
        DontDestroyOnLoad(_instance.gameObject);
    }

    /// <summary>
    ///     初始化
    /// </summary>
    protected virtual void OnSingletonInit()
    {
    }

    private void OnDestroy()
    {
        if (_duplicateToDestroy)
        {
            return;
        }

        if (_instance == this)
        {
            _instance = null;
        }
    }
}