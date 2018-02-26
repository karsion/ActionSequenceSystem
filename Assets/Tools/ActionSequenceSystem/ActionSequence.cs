// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-26
// Time: 11:38
// Author: Karsion

using System;
using System.Collections.Generic;
using UnityEngine;

//开动作序列类
public class ActionSequence
{
    internal static readonly ObjectPool<ActionSequence> opSequences = new ObjectPool<ActionSequence>(64);

    //节点列表，默认把数组容量设为8
    public readonly List<ActionNode> nodes = new List<ActionNode>(8);

    //当前执行的节点索引
    private int curNodeIndex = 0;
    public float timeAxis;

    //目标组件，组件销毁的时候，本动作序列也相应销毁
    public object id { get; private set; }

    //需要循环的次数
    public int loopTime { get; private set; }

    //已经运行的次数
    public int cycles { get; private set; }

    //是否已经运行完
    public bool isFinshed { get; private set; }

#if UNITY_EDITOR
    public static void GetObjectPoolInfo(out int countActive, out int countAll)
    {
        countActive = opSequences.countActive;
        countAll = opSequences.countAll;
    }
#endif

    //序列停止
    public void Stop()
    {
        id = null;
        isFinshed = true;
        cycles = 0;
        loopTime = 0;
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
    public ActionSequence Condition(Func<bool> condition)
    {
        nodes.Add(ActionNodeCondition.Get(condition));
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

    //开启序列
    private ActionSequence Start(object id)
    {
        this.id = id;
        curNodeIndex = 0;
        isFinshed = false;
        cycles = 0;
        timeAxis = 0;
        return this;
    }

    //序列更新
    public void Update(float deltaTime)
    {
        //不更新已经Stop的
        if (isFinshed)
        {
            return;
        }

        //这个情况就是id被销毁了
        if (id == null)
        {
            isFinshed = true;
            return;
        }

        timeAxis += deltaTime;

        //用索引更新节点
        if (nodes[curNodeIndex].Update(this))
        {
            curNodeIndex++;
            if (curNodeIndex >= nodes.Count)
            {
                //无限循环的节点
                if (loopTime < 0)
                {
                    Restart();
                    return;
                }

                //循环的节点需要重新启动，运行次数++
                if (loopTime > cycles)
                {
                    Restart();
                    return;
                }

                //运行次数>=循环次数了，就停止
                isFinshed = true;
            }
        }
    }

    //回收序列，回收序列中的节点
    internal void Release()
    {
        cycles = 0;
        opSequences.Release(this);
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Release();
        }

        nodes.Clear();
    }

    internal void UpdateTimeAxis(float interval)
    {
        timeAxis -= interval;
    }

    //重启序列
    private void Restart()
    {
        cycles++;
        curNodeIndex = 0;
        timeAxis = 0;
    }

    internal static ActionSequence GetInstance(object component)
    {
        return opSequences.Get().Start(component);
    }
}