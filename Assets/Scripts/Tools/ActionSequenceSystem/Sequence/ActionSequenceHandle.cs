using System;
using UnityEngine;

namespace UnrealM
{
    //这个类设计为单独控制Sequence的停止，如果你想手动停止的话
    //目前设计为一对一控制，重复SetHandle的话，会丢失对上一个Sequence的控制权
    //考虑到如果设计为一对多的话，内部需要保存一个数组，会浪费内存，一对多的使用情况也很少，故不支持
    public class ActionSequenceHandle
    {
        internal ActionSequence sequence;

        public void StopSequence()
        {
            if (sequence != null)
            {
                sequence.Stop();
            }
        }

        //用Handle开序列
        public ActionSequence Sequence()
        {
            return ActionSequenceSystem.Sequence().SetHandle(this);
        }

        //直接延迟动作
        public ActionSequence Delayer(float delay, Action action)
        {
            return Sequence().Interval(delay).Action(action);
        }

        //直接循环动作
        public ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return isActionAtStart ?
                Sequence().Action(action).Interval(interval).Loop(loopTime) :
                Sequence().Interval(interval).Action(action).Loop(loopTime);
        }

        //直接循环动作带Loop次数
        public ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ?
                Sequence().Action(action).Interval(interval).Loop(loopTime) :
                Sequence().Interval(interval).Action(action).Loop(loopTime);
        }
    }
}