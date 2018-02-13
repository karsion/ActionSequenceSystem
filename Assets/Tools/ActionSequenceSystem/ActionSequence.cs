// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-09
// Time: 14:54
// Author: Karsion

using System;
using System.Collections.Generic;
using UnityEngine;

//开动作序列类
public class ActionSequence
{
#if UNITY_EDITOR
    public static void GetObjectPoolInfo(out int countActive, out int countAll)
    {
        countActive = opSequences.countActive;
        countAll = opSequences.countAll;
    }
#endif

    internal static readonly ObjectPool<ActionSequence> opSequences = new ObjectPool<ActionSequence>(64);
    //节点列表，默认把数组容量设为8
    public readonly List<ActionNode> nodes = new List<ActionNode>(8);

    //目标组件，组件销毁的时候，本动作序列也相应销毁
    public Component id { get; private set; }
    //当前执行的节点索引
    private int nCurIndex = 0;
    //需要循环的次数
    public int nLoopTime { get; private set; }
    //已经运行的次数
    public int nRunLoopTime { get; private set; }
    //是否已经运行完
    public bool isFinshed { get; private set; }

    //序列停止
    public void Stop()
    {
        id = null;
        isFinshed = true;
        nRunLoopTime = 0;
        nLoopTime = 0;
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
        actionNodeAction.UpdateLoopTime(nRunLoopTime);
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
            nLoopTime = loopTime - 1;
            return this;
        }

        nLoopTime = loopTime;
        return this;
    }

    //开启序列
    private ActionSequence Start(Component id)
    {
        this.id = id;
        nCurIndex = 0;
        isFinshed = false;
        nRunLoopTime = 0;
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
        if (!id)
        {
            isFinshed = true;
            return;
        }

        //用索引更新节点
        if (nodes[nCurIndex].Update(deltaTime))
        {
            nCurIndex++;
            if (nCurIndex >= nodes.Count)
            {
                //无限循环的节点
                if (nLoopTime < 0)
                {
                    Restart();
                    return;
                }

                //循环的节点需要重新启动，运行次数++
                if (nLoopTime > nRunLoopTime)
                {
                    Restart();
                    return;
                }

                //运行次数>=循环次数了，就停止
                isFinshed = true;
                return;
            }

            nodes[nCurIndex].UpdateLoopTime(nRunLoopTime);
        }
    }

    //回收序列，回收序列中的节点
    internal void Release()
    {
        nRunLoopTime = 0;
        opSequences.Release(this);
        nodes.ForEach(node => node.Release());
        nodes.Clear();
    }

    //重启序列
    private void Restart()
    {
        nRunLoopTime++;
        nCurIndex = 0;
        nodes.ForEach(node => node.Restart());
    }

    internal static ActionSequence GetInstance(Component component)
    {
        return opSequences.Get().Start(component);
    }
}