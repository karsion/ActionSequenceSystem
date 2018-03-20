// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-20 11:38
// ***************************************************************************

using System;
using UnityEngine;

namespace UnrealM
{
    public static class ActionSequenceSystemEx
    {
        //用Component作为ID开序列
        public static ActionSequence Sequence(this Component id)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            return seq;
        }

        //用Component作为ID停止序列
        public static void StopSequence(this Component id)
        {
            ActionSequenceSystem.StopSequence(id);
        }

        public static void StopSequence(this ActionSequence sequence)
        {
            if (sequence != null)
            {
                sequence.Stop();
            }
        }

        //AutoHide
        public static ActionSequence Hider(this Component id, float delay)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            seq.Interval(delay).Hide();
            return seq;
        }

        //直接延迟动作
        public static ActionSequence Delayer(this Component id, float delay, Action action)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            seq.Interval(delay).Action(action);
            return seq;
        }

        //直接循环动作
        public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action action)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            if (isActionAtStart)
            {
                seq.Action(action).Interval(interval);
            }
            else
            {
                seq.Interval(interval).Action(action);
            }

            seq.Loop(loopTime);
            return seq;
        }

        //直接循环动作
        public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            if (isActionAtStart)
            {
                seq.Action(action).Interval(interval);
            }
            else
            {
                seq.Interval(interval).Action(action);
            }

            seq.Loop(loopTime);
            return seq;
        }
    }
}