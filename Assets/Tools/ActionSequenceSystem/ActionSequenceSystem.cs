// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-26
// Time: 16:20
// Author: Karsion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealM
{
    //开动作序列扩展
    public static class ActionSequenceSystemEx
    {
        //用Component作为ID开序列
        public static ActionSequence Sequence(this object id)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            return seq;
        }

        //用Component作为ID停止序列
        public static void StopSequence(this object id)
        {
            ActionSequenceSystem.SetStopSequenceID(id);
        }

        //直接延迟动作
        public static ActionSequence Delayer(this object id, float delay, Action action)
        {
            ActionSequence seq = ActionSequenceSystem.GetSequence(id);
            seq.Interval(delay).Action(action);
            return seq;
        }

        //直接循环动作
        public static ActionSequence Looper(this object id, float interval, int loopTime, bool isActionAtStart, Action action)
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
        public static ActionSequence Looper(this object id, float interval, int loopTime, bool isActionAtStart, Action<int> action)
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

    public class ActionSequenceSystem : SingletonMono<ActionSequenceSystem>
    {
        private readonly List<ActionSequence> listSequence = new List<ActionSequence>(64);
        public List<ActionSequence> ListSequence { get { return listSequence; } }

#if UNITY_EDITOR
        private void Reset()
        {
            name = "ActionSequenceSystem";
        }
#endif

        //开动作序列
        public static ActionSequence GetSequence(object component)
        {
            ActionSequence seq = ActionSequence.GetInstance(component);
            instance.listSequence.Add(seq);
            return seq;
        }

        // Update is called once per frame
        private void Update()
        {
            //UpdateSequence
            bool isNeedRemoveSequence = false;
            for (int i = 0; i < listSequence.Count; i++)
            {
                listSequence[i].Update(Time.deltaTime);
                if (listSequence[i].isFinshed)
                {
                    listSequence[i].Release();
                    isNeedRemoveSequence = true;
                }
            }

            //RemoveFinshedSequence
            if (isNeedRemoveSequence)
            {
                listSequence.RemoveAll(seq => seq.isFinshed);
            }
        }

        private void StopSequenceByID(object id)
        {
            for (int i = 0; i < listSequence.Count; i++)
            {
                if (id == listSequence[i].id)
                {
                    listSequence[i].Stop();
                }
            }
        }

        public static void SetStopSequenceID(object id)
        {
            instance.StopSequenceByID(id);
        }
    }
}