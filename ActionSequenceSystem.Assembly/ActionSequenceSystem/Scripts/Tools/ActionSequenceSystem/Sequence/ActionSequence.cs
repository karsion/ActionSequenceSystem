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
    /// <summary>
    /// 行动序列类
    /// </summary>
    public class ActionSequence
    {
        private ActionSequence()
        {
        }

        internal static ActionSequence Get()
        {
            return new ActionSequence();
        }

        internal static readonly ObjectPool<ActionSequence> opNodes = new ObjectPool<ActionSequence>(200, Get);
        /// <summary>
        /// 内存池活动元素
        /// </summary>
        public static int countActive { get { return opNodes.countActive; } }

        /// <summary>
        /// 内存池总数
        /// </summary>
        public static int countAll { get { return opNodes.countAll; } }

        //节点列表，默认把数组容量设为比较大，做长链的时候不容易自动扩容
        private readonly List<ActionNode> nodes = new List<ActionNode>(16);

        //当前执行的节点索引
        private int curNodeIndex = 0;

        /// <summary>
        /// 获取ActionNode列表的元素个数
        /// </summary>
        public int nodesCount { get { return nodes.Count; } }

        /// <summary>
        /// 时间轴
        /// </summary>
        public float timeAxis { get; private set; }

        private bool hasID = false;

        /// <summary>
        /// 目标组件，组件销毁的时候，本动作序列也相应销毁
        /// </summary>
        public Component id { get; private set; }

        /// <summary>
        /// 设置一个句柄用于正确的停止序列
        /// </summary>
        internal ActionSequenceHandle handle { get; private set; }

        /// <summary>
        /// 需要循环的次数
        /// </summary>
        public int loopTime { get; private set; }

        /// <summary>
        /// 已经运行的次数
        /// </summary>
        public int cycles { get; private set; }

        /// <summary>
        /// 是否已经运行完
        /// </summary>
        internal bool isFinshed { get; private set; }

        /// <summary>
        /// 是否下一帧停止
        /// </summary>
        internal bool bSetStop { get; private set; }



        internal static ActionSequence GetInstance(Component component = null)
        {
            return opNodes.Get().Start(component);
        }

        #region Chaining
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


        /// <summary>
        /// 设置GameObject隐藏（SetActive）
        /// </summary>
        /// <returns></returns>
        public ActionSequence Hide()
        {
            nodes.Add(ActionNodeSetActive.Get(false));
            return this;
        }

        /// <summary>
        /// 设置GameObject显示（SetActive(false)）
        /// </summary>
        /// <returns></returns>
        public ActionSequence Show()
        {
            nodes.Add(ActionNodeSetActive.Get(true));
            return this;
        }

        /// <summary>
        /// 设置GameObject反显示隐藏（SetActive(!activeSelf)）
        /// </summary>
        /// <returns></returns>
        public ActionSequence ToggleActive()
        {
            nodes.Add(ActionNodeSetActive.Get(true, true));
            return this;
        }

        /// <summary>
        /// 增加一个运行节点
        /// </summary>
        /// <param name="interval">时间间隔</param>
        /// <returns></returns>
        public ActionSequence Interval(float interval)
        {
            nodes.Add(ActionNodeInterval.Get(interval));
            return this;
        }

        /// <summary>
        /// 增加一个行动节点
        /// </summary>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public ActionSequence Action(Action action)
        {
            nodes.Add(ActionNodeAction.Get(action));
            return this;
        }

        /// <summary>
        /// 增加一个带循环次数行动节点
        /// </summary>
        /// <param name="action">调用的函数带循环次数</param>
        /// <returns></returns>
        public ActionSequence Action(Action<int> action)
        {
            nodes.Add(ActionNodeAction.Get(action));
            return this;
        }

        /// <summary>
        /// 增加一个条件节点
        /// </summary>
        /// <param name="condition">判断条件函数，返回true则跳下一节点</param>
        /// <returns></returns>
        public ActionSequence WaitFor(Func<bool> condition)
        {
            nodes.Add(ActionNodeWaitFor.Get(condition));
            return this;
        }

        /// <summary>
        /// 设置循环
        /// </summary>
        /// <param name="loopTime">循环次数，-1为无限，0和1都是1次</param>
        /// <returns></returns>
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
        private ActionSequence Start(Component id = null)
        {
            hasID = id;
            this.id = id;
            curNodeIndex = 0;
            isFinshed = false;
            cycles = 0;
            timeAxis = 0;
            loopTime = 0;
            bSetStop = false;
            handle = null;
            return this;
        }

        /// <summary>
        /// 用指定的ID去尝试停止，ID不对的话，可能是已经停止的了
        /// </summary>
        /// <param name="callerID"></param>
        public void Stop(Component callerID)
        {
            if (hasID && id == callerID)
            {
                Stop();
            }
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
            if (handle != null)
            {
                handle.sequence = null;
                handle = null;
            }
        }

        //序列更新
        internal bool Update(float deltaTime)
        {
            //SetStop to kill || 没有id，Auto kill || 开了序列没有加任何节点
            if (bSetStop || hasID && id == null || nodes.Count == 0)
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
        internal void Release()
        {
            opNodes.Release(this);
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

    }
}