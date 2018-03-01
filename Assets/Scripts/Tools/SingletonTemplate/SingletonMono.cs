// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-02-27 18:14
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using System;
using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
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
            (_instance as SingletonMono<T>).OnSingletonInit();
            DontDestroyOnLoad(_instance.gameObject);
            return;
        }

        if (_instance != null && _instance != this)
        {
            _duplicateToDestroy = true;
            Destroy(gameObject);
            return;
        }

        (_instance as SingletonMono<T>).OnSingletonInit();
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