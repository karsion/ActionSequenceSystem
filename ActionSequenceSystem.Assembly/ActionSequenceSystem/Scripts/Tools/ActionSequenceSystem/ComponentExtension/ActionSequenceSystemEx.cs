// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 9:34
// ***************************************************************************

using System;
using UnityEngine;

namespace UnrealM
{
    /// <summary>
    /// Methods that extend known Unity components and allow to directly create and control timer from their instances
    /// </summary>
    public static class ActionSequenceSystemEx
    {
        #region Start Stop
        /// <summary>
        /// 用Component作为ID开序列
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ActionSequence Sequence(this Component id)
        {
            return ActionSequenceSystem.Sequence(id);
        }

        /// <summary>
        /// 用Component作为ID停止序列
        /// </summary>
        /// <param name="id"></param>
        public static void StopSequence(this Component id)
        {
            ActionSequenceSystem.StopSequence(id);
        }

        /// <summary>
        /// 用Component作为ID停止指定的序列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sequence">停止指定的ActionSequence</param>
        public static void StopSequence(this Component id, ActionSequence sequence)
        {
            if (sequence != null)
            {
                sequence.Stop(id);
            }
        }
        #endregion

        #region Shower Hider
        /// <summary>
        /// 延迟激活ID的GameObject
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delay">延迟</param>
        /// <returns></returns>
        public static ActionSequence Shower(this Component id, float delay)
        {
            return Sequence(id).Interval(delay).Show();
        }

        /// <summary>
        /// 延迟反激活ID的GameObject
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delay">延迟</param>
        /// <returns></returns>
        public static ActionSequence Hider(this Component id, float delay)
        {
            return Sequence(id).Interval(delay).Hide();
        }
        #endregion

        #region Delayer Looper WaitFor
        /// <summary>
        /// 延迟调用函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Delayer(this Component id, float delay, Action action)
        {
            return Sequence(id).Interval(delay).Action(action);
        }

        /// <summary>
        /// 延迟调用函数，循环次数作为参数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, float interval, Action<int> action)
        {
            return Sequence(id).Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 无限循环调用函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, float interval, Action action)
        {
            return Sequence(id).Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, float interval, int loopTime, Action action)
        {
            return Sequence(id).Interval(interval).Action(action).Loop(loopTime);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, float interval, int loopTime, Action<int> action)
        {
            return Sequence(id).Interval(interval).Action(action).Loop(loopTime);
        }

        /// <summary>
        /// 循环调用函数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return isActionAtStart ?
                id.Sequence().Action(action).Interval(interval).Loop(loopTime) :
                id.Looper(interval, loopTime, action);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ?
                id.Sequence().Action(action).Interval(interval).Loop(loopTime) :
                id.Looper(interval, loopTime, action);
        }

        /// <summary>
        /// 等待条件判断成功后调用函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="condition">判断函数，函数返回true，调用action</param>
        /// <param name="action">调用函数</param>
        /// <returns></returns>
        public static ActionSequence WaitFor(this Component id, Func<bool> condition, Action action)
        {
            return id.Sequence().WaitFor(condition).Action(action);
        }
        #endregion

        #region Start Delayer Looper with handle
        /// <summary>
        /// 用Component作为ID开序列，并使用“控制句柄”
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle">控制句柄</param>
        /// <returns></returns>
        public static ActionSequence Sequence(this Component id, ActionSequenceHandle handle)
        {
            return ActionSequenceSystem.Sequence(id).SetHandle(handle);
        }

        /// <summary>
        /// 延迟调用函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle">控制句柄</param>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Delayer(this Component id, ActionSequenceHandle handle, float delay, Action action)
        {
            return Delayer(id, delay, action).SetHandle(handle);
        }

        /// <summary>
        /// 无限循环调用函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle">控制句柄</param>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, ActionSequenceHandle handle, float interval, Action action)
        {
            return Looper(id, interval, action).SetHandle(handle);
        }

        /// <summary>
        /// 无限循环调用函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle">控制句柄</param>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, ActionSequenceHandle handle, float interval, Action<int> action)
        {
            return Looper(id, interval, action).SetHandle(handle);
        }

        /// <summary>
        /// 循环调用函数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle">控制句柄</param>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, ActionSequenceHandle handle, float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return Looper(id, interval, loopTime, isActionAtStart, action).SetHandle(handle);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handle">控制句柄</param>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public static ActionSequence Looper(this Component id, ActionSequenceHandle handle, float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return Looper(id, interval, loopTime, isActionAtStart, action).SetHandle(handle);
        }
        #endregion
    }
}