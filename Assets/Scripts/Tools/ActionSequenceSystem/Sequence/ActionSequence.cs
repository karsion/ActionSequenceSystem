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
    //开动作序列类
    public class ActionSequence
    {
        internal static readonly ObjectPool<ActionSequence> opSequences = new ObjectPool<ActionSequence>(64);

        //节点列表，默认把数组容量设为16
        public readonly List<ActionNode> nodes = new List<ActionNode>(16);

        //当前执行的节点索引
        private int curNodeIndex = 0;

        //时间轴
        public float timeAxis { get; private set; }

        //目标组件，组件销毁的时候，本动作序列也相应销毁
        public Component id { get; private set; }
        private bool hasID = false;

        //设置一个句柄用于正确的停止序列
        public ActionSequenceHandle handle { get; private set; }

        //需要循环的次数
        public int loopTime { get; private set; }

        //已经运行的次数
        public int cycles { get; private set; }

        //是否已经运行完
        public bool isFinshed { get; private set; }

        public bool bSetStop { get; private set; }

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
        public ActionSequence Hide()
        {
            nodes.Add(ActionNodeSetActive.Get(false));
            return this;
        }

        public ActionSequence Show()
        {
            nodes.Add(ActionNodeSetActive.Get(true));
            return this;
        }

        public ActionSequence ToggleActive()
        {
            nodes.Add(ActionNodeSetActive.Get(true, true));
            return this;
        }

        //增加一个运行节点
        public ActionSequence Interval(float interval)
        {
            nodes.Add(ActionNodeInterval.Get(interval));
            return this;
        }

        //增加一个动作节点
        public ActionSequence Action(Action action)
        {
            nodes.Add(ActionNodeAction.Get(action));
            return this;
        }

        //增加一个带循环次数的动作节点
        public ActionSequence Action(Action<int> action)
        {
            ActionNodeAction actionNodeAction = ActionNodeAction.Get(action);
            nodes.Add(actionNodeAction);
            return this;
        }

        //增加一个条件节点
        public ActionSequence WaitFor(Func<bool> condition)
        {
            nodes.Add(ActionNodeWaitFor.Get(condition));
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
            handle = null;
            return this;
        }

        private ActionSequence Start()
        {
            hasID = false;
            id = null;
            curNodeIndex = 0;
            isFinshed = false;
            cycles = 0;
            timeAxis = 0;
            loopTime = 0;
            bSetStop = false;
            handle = null;
            return this;
        }

        //外部调用停止，内部执行的时候才会自杀，为了保持运行列表与缓存池同步
        internal void Stop()
        {
            bSetStop = true;
            if (handle != null)
            {
                handle.sequence = null;
                handle = null;
            }
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
        }

        //序列更新
        internal bool Update(float deltaTime)
        {
            //SetStop to kill || 没有id，Auto kill || 开了序列没有加任何节点
            if (bSetStop || (hasID && id == null) || (nodes.Count == 0))
            {
                Kill();
                return false;
            }

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
                    cycles++;
                    curNodeIndex = 0;
                }
            }

            return true;
        }

        //回收序列，回收序列中的节点
        public void Release()
        {
            opSequences.Release(this);
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Release();
            }

            nodes.Clear();
        }

        internal void UpdateTimeAxis(float deltaTime)
        {
            timeAxis += deltaTime;
        }

        internal ActionSequence SetHandle(ActionSequenceHandle handle)
        {
            //可能有人控制这个Sequence，将会丢弃前任，启用新人
            if (this.handle != null)
            {
                Debug.LogWarning("try set handle, but this sequence is already controlled by a handle! Original handle lose control of this sequence and input handle take control.");
                this.handle.sequence = null;
            }

            //可能句柄还在控制其他Sequence，丢失上一个Sequence的控制权，控制当前的Sequence
            if (handle.sequence != null)
            {
                //Debug.LogWarning("try set handle, but the input handle already controls a sequence! Lose control of the original sequence and take control this sequence");
                handle.sequence.handle = null;
            }

            //设置句柄
            this.handle = handle;
            handle.sequence = this;
            return this;
        }
    }
}