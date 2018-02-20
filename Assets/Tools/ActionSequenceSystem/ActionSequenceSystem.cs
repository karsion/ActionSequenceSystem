// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-13
// Time: 22:43
// Author: Karsion

using System;
using System.Collections.Generic;
using UnityEngine;

//开动作序列扩展
public static class ActionSequenceSystemEx
{
    //用Component作为ID开序列
    public static ActionSequence Sequence(this Component id)
    {
        ActionSequence seq = ActionSequenceSystem.GetSequence(id);
        return seq;
    }

    //用Component作为ID停止序列
    public static void StopSequence(this Component id)
    {
        ActionSequenceSystem.SetStopSequenceID(id);
    }

    //直接延迟动作
    public static ActionSequence Delayer(this Component id, float delay, Action action)
    {
        ActionSequence seq = ActionSequenceSystem.GetSequence(id);
        seq.Interval(delay).Action(action);
        return seq;
    }

    //直接循环动作
    public static ActionSequence Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action action)
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
    public static Component Looper(this Component id, float interval, int loopTime, bool isActionAtStart, Action<int> action)
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
        return id;
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
    public static ActionSequence GetSequence(Component component)
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
}