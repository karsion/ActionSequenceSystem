// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Copyright (c) 2018 Karsion
//  * 
//  * https://github.com/karsion
//  * Date: 2018-03-01 17:32
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

        private void Stop(Component id)
        {
            //StopSequence by id
            bool isSomeSequenceStoped = false;
            for (int i = 0; i < listSequence.Count; i++)
            {
                if (id == listSequence[i].id)
                {
                    listSequence[i].Stop();
                    isSomeSequenceStoped = true;
                }
            }

            //RemoveFinshedSequence
            if (isSomeSequenceStoped)
            {
                listSequence.RemoveAll(seq => seq.isFinshed);
            }
        }

        public static void StopSequence(Component id)
        {
            instance.Stop(id);
        }

        private static ActionSequence GetSequence()
        {
            ActionSequence seq = ActionSequence.GetInstance();
            instance.listSequence.Add(seq);
            return seq;
        }

        #region 无ID启动（注意要手动关闭无限循环的序列，不然机器就会爆炸……）
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