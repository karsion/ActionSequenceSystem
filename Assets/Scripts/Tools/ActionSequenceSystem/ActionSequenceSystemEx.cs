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
    public static class ActionSequenceSystemEx
    {
        #region Start Stop

        //用Component作为ID开序列
        public static ActionSequence Sequence(this Component id)
        {
            return ActionSequenceSystem.GetSequence(id);
        }

        public static ActionSequence Sequence(this Component id, ActionSequenceHandle handle)
        {
            return ActionSequenceSystem.GetSequence(id).SetHandle(handle);
        }

        //用Component作为ID停止序列
        public static void StopSequence(this Component id)
        {
            ActionSequenceSystem.StopSequence(id);
        }
        #endregion

        #region Shower Hider
        public static ActionSequence Shower(this Component id, float delay)
        {
            return Sequence(id).Interval(delay).Show();
        }

        //AutoHide
        public static ActionSequence Hider(this Component id, float delay)
        {
            return Sequence(id).Interval(delay).Hide();
        }
        #endregion

        #region Delayer Looper WaitFor

        //直接延迟动作
        public static ActionSequence Delayer(this Component id, float delay, Action action)
        {
            return Sequence(id).Interval(delay).Action(action);
        }

        public static ActionSequence Infiniter(this Component id, float interval, Action<int> action)
        {
            return Sequence(id).Interval(interval).Action(action).Loop();
        }

        //直接循环动作
        public static ActionSequence Infiniter(this Component id, float interval, Action action)
        {
            return Sequence(id).Interval(interval).Action(action).Loop();
        }

        //直接循环动作
        public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return isActionAtStart ?
                Sequence(id).Action(action).Interval(interval).Loop(loopTime) :
                Sequence(id).Interval(interval).Action(action).Loop(loopTime);
        }

        //直接循环动作带Loop次数
        public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ?
                Sequence(id).Action(action).Interval(interval).Loop(loopTime) :
                Sequence(id).Interval(interval).Action(action).Loop(loopTime);
        }

        //直接延迟动作
        public static ActionSequence WaitFor(this Component id, Func<bool> condition, Action action)
        {
            return Sequence(id).WaitFor(condition).Action(action);
        }
        #endregion

        #region Delayer Looper with handle

        //直接延迟动作
        public static ActionSequence Delayer(this Component id, ActionSequenceHandle handle, float delay, Action action)
        {
            return Delayer(id, delay, action).SetHandle(handle);
        }

        //直接循环动作
        public static ActionSequence Infiniter(this Component id, ActionSequenceHandle handle, float interval, Action action)
        {
            return Infiniter(id, interval, action).SetHandle(handle);
        }

        //直接循环动作
        public static ActionSequence Infiniter(this Component id, ActionSequenceHandle handle, float interval, Action<int> action)
        {
            return Infiniter(id, interval, action).SetHandle(handle);
        }

        //直接循环动作
        public static ActionSequence Looper(this Component id, ActionSequenceHandle handle, float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return Looper(id, interval, loopTime, isActionAtStart, action).SetHandle(handle);
        }

        //直接循环动作带Loop次数
        public static ActionSequence Looper(this Component id, ActionSequenceHandle handle, float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return Looper(id, interval, loopTime, isActionAtStart, action).SetHandle(handle);
        }
        #endregion
    }
}