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

    public class ActionSequenceSystem : SingletonMono<ActionSequenceSystem>
    {
        private readonly List<ActionSequence> listSequence = new List<ActionSequence>(64);

#if UNITY_EDITOR
        public List<ActionSequence> ListSequence { get { return listSequence; } }
        private void Reset()
        {
            name = "ActionSequenceSystem";
        }
#endif

        //Start a sequence
        internal static ActionSequence GetSequence(Component component)
        {
            ActionSequence seq = ActionSequence.GetInstance(component);
            instance.listSequence.Add(seq);
            return seq;
        }

        // Update is called once per frame
        private void Update()
        {
            //UpdateSequence
            bool isSomeSequenceStoped = false;
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < listSequence.Count; i++)
            {
                //It's stopped when Update return false 
                isSomeSequenceStoped |= !listSequence[i].Update(deltaTime);
            }

            //RemoveFinshedSequence
            if (isSomeSequenceStoped)
            {
                listSequence.RemoveAll(seq => seq.isFinshed);
            }
        }

        private void StopSequenceByID(Component id)
        {
            for (int i = 0; i < listSequence.Count; i++)
            {
                if (id == listSequence[i].id)
                {
                    listSequence[i].Stop();
                }
            }
        }

        public static void SetStopSequenceID(Component id)
        {
            instance.StopSequenceByID(id);
        }

        private static ActionSequence GetSequence()
        {
            ActionSequence seq = ActionSequence.GetInstance();
            instance.listSequence.Add(seq);
            return seq;
        }

        #region 无ID启动（注意要手动关闭循环的，不然机器就会爆炸……）
        public static ActionSequence Sequence()
        {
            ActionSequence seq = GetSequence();
            return seq;
        }

        public static ActionSequence Delayer(float delay, Action action)
        {
            ActionSequence seq = GetSequence();
            seq.Interval(delay).Action(action);
            return seq;
        }

        public static ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action action)
        {
            ActionSequence seq = GetSequence();
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

        public static ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            ActionSequence seq = GetSequence();
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
        #endregion
    }
}