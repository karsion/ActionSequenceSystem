// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-20 11:39
// ***************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealM
{
    //开动作序列扩展
    public class ActionSequenceSystem : SingletonMonoAuto<ActionSequenceSystem>
    {
        private readonly List<ActionSequence> listSequenceAlive = new List<ActionSequence>(64);

        #region UNITY_EDITOR
#if UNITY_EDITOR
        public List<ActionSequence> ListSequenceAlive { get { return listSequenceAlive; } }

        private void Reset()
        {
            name = "ActionSequenceSystem";
        }
#endif
        #endregion

        //Get a sequence
        internal static ActionSequence GetSequence(Component component = null)
        {
            ActionSequence seq = ActionSequence.GetInstance(component);
            instance.listSequenceAlive.Add(seq);
            return seq;
        }

        private void Update()
        {
            //Update Sequence(Auto release)
            bool isSomeSequenceStoped = false;
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < listSequenceAlive.Count; i++)
            {
                //It's stopped when Update return false and release self
                isSomeSequenceStoped |= !listSequenceAlive[i].Update(deltaTime);
            }

            //Remove Finshed Sequence(Finshed is Released)
            if (isSomeSequenceStoped)
            {
                for (int i = 0; i < listSequenceAlive.Count; i++)
                {
                    ActionSequence actionSequence = listSequenceAlive[i];
                    if (actionSequence.isFinshed)
                    {
                        actionSequence.Release();
                    }
                }

                listSequenceAlive.RemoveAll(seq => seq.isFinshed);
            }
        }

        //最终停止的会在Update中批量Release回Pool，并同步List
        public static void StopSequence(Component id)
        {
            List<ActionSequence> listSequenceAlive = instance.listSequenceAlive;
            for (int i = 0; i < listSequenceAlive.Count; i++)
            {
                if (id == listSequenceAlive[i].id)
                {
                    listSequenceAlive[i].Stop();
                }
            }
        }

        public static void StopAll()
        {
            List<ActionSequence> listSequenceAlive = instance.listSequenceAlive;
            for (int i = 0; i < listSequenceAlive.Count; i++)
            {
                listSequenceAlive[i].Stop();
            }
        }

        //#region 无ID启动（注意要用Handle手动关闭无限循环的序列，不然机器就会爆炸……）
        public static ActionSequence Sequence()
        {
            return GetSequence();
        }

        public static ActionSequence Delayer(float delay, Action action)
        {
            return GetSequence().Interval(delay).Action(action);
        }

        public static ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return isActionAtStart ?
                GetSequence().Action(action).Interval(interval).Loop(loopTime) :
                GetSequence().Interval(interval).Action(action).Loop(loopTime);
        }

        public static ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ?
                GetSequence().Action(action).Interval(interval).Loop(loopTime) :
                GetSequence().Interval(interval).Action(action).Loop(loopTime);
        }

        public static ActionSequence Delayer(ActionSequenceHandle handle, float delay, Action action)
        {
            return Delayer(delay, action).SetHandle(handle);
        }

        public static ActionSequence Looper(ActionSequenceHandle handle, float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return Looper(interval, loopTime, isActionAtStart, action).SetHandle(handle);
        }

        public static ActionSequence Looper(ActionSequenceHandle handle, float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return Looper(interval, loopTime, isActionAtStart, action).SetHandle(handle);
        }
        //#endregion
    }
}