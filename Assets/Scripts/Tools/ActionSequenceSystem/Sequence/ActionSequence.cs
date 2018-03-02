// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 9:34
// ***************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealM
{
    //开动作序列类
    public class ActionSequence
    {
        internal static readonly ObjectPool<ActionSequence> opSequences = new ObjectPool<ActionSequence>(64);

        //节点列表，默认把数组容量设为8
        public readonly List<ActionNode> nodes = new List<ActionNode>(8);

        //当前执行的节点索引
        private int curNodeIndex = 0;

        //时间轴
        public float timeAxis { get; private set; }

        //目标组件，组件销毁的时候，本动作序列也相应销毁
        public Component id { get; private set; }
        private bool hasID = false;

        //需要循环的次数
        public int loopTime { get; private set; }

        //已经运行的次数
        public int cycles { get; private set; }

        //是否已经运行完
        public bool isFinshed { get; private set; }

        private bool bSetStop = false;

#if UNITY_EDITOR
        public static void GetObjectPoolInfo(out int countActive, out int countAll)
        {
            countActive = opSequences.countActive;
            countAll = opSequences.countAll;
        }
#endif

        internal static ActionSequence GetInstance(Component component)
        {
            return opSequences.Get().Start(component);
        }

        internal static ActionSequence GetInstance()
        {
            return opSequences.Get().Start();
        }

        #region Chaining
        //private ActionSequence StratFirstNode()
        //{
        //    if (nodes.Count == 1)
        //    {
        //        nodes[0].Start(this);
        //    }

        //    return this;
        //}

        //增加一个运行节点
        public ActionSequence Interval(float interval)
        {
            nodes.Add(ActionNodeInterval.Get(interval));
            //return StratFirstNode();
            return this;
        }

        //增加一个动作节点
        public ActionSequence Action(Action action)
        {
            nodes.Add(ActionNodeAction.Get(action));
            //return StratFirstNode();
            return this;
        }

        //增加一个带循环次数的动作节点
        public ActionSequence Action(Action<int> action)
        {
            ActionNodeAction actionNodeAction = ActionNodeAction.Get(action);
            nodes.Add(actionNodeAction);
            //return StratFirstNode();
            return this;
        }

        //增加一个条件节点
        public ActionSequence Condition(Func<bool> condition)
        {
            nodes.Add(ActionNodeCondition.Get(condition));
            //return StratFirstNode();
            return this;
        }

        //设置循环
        public ActionSequence Loop(int loopTime = -1)
        {
            if (loopTime > 0)
            {
                this.loopTime = loopTime - 1;
                return this;
            }

            this.loopTime = loopTime;
            return this;
        }
        #endregion

        //开启序列
        private ActionSequence Start(Component id)
        {
            this.id = id;
            hasID = true;

            curNodeIndex = 0;
            isFinshed = false;
            cycles = 0;
            timeAxis = 0;
            loopTime = 0;
            bSetStop = false;
            //isStopTimeAxis = false;
            return this;
        }

        private ActionSequence Start()
        {
            hasID = false;

            curNodeIndex = 0;
            isFinshed = false;
            cycles = 0;
            timeAxis = 0;
            loopTime = 0;
            bSetStop = false;
            //isStopTimeAxis = false;
            return this;
        }

        //外部调用停止，内部执行的时候才会自杀，为了保持运行列表与缓存池同步
        internal void Stop()
        {
            bSetStop = true;
        }

        //序列自杀
        private void Kill()
        {
            if (isFinshed)
            {
                return;
            }

            id = null;
            hasID = false;

            curNodeIndex = 0;
            isFinshed = true;
            cycles = 0;
            timeAxis = 0;
            loopTime = 0;
            bSetStop = false;
            //isStopTimeAxis = false;
            Release();
        }

        //internal bool isStopTimeAxis = false;
        //序列更新
        internal bool Update(float deltaTime)
        {
            //SetStop to kill || 没有id，Auto kill || 开了序列没有加任何节点
            if (bSetStop || (hasID && id == null) || (nodes.Count == 0))
            {
                Kill();
                return false;
            }

            //if (!isStopTimeAxis)
            //{
            //}

            //用索引更新节点
            if (nodes[curNodeIndex].Update(this, deltaTime))
            {
                curNodeIndex++;
                if (curNodeIndex >= nodes.Count)
                {
                    //运行次数>=循环次数了，就停止
                    if (loopTime > -1 && cycles >= loopTime)
                    {
                        Kill();
                        return false;
                    }

                    //无限循环的节点 或 有限，运行次数++
                    NextLoop();
                }

                //nodes[curNodeIndex].Start(this);
            }

            return true;
        }

        //回收序列，回收序列中的节点
        private void Release()
        {
            opSequences.Release(this);
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Release();
            }

            nodes.Clear();
        }

        //重启序列
        private void NextLoop()
        {
            cycles++;
            curNodeIndex = 0;
            //timeAxis = 0;
        }

        internal void UpdateTimeAxis(float deltaTime)
        {
            timeAxis += deltaTime;
        }
    }
}