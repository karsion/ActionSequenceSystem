// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-10
// Time: 23:46
// Author: Karsion

using System;

//事件节点
public class ActionNodeAction : ActionNode
{
#if UNITY_EDITOR
    public static void GetObjectPoolInfo(out int countActive, out int countAll)
    {
        countActive = opNodeAction.countActive;
        countAll = opNodeAction.countAll;
    }
#endif

    //全局池
    private static readonly ObjectPool<ActionNodeAction> opNodeAction = new ObjectPool<ActionNodeAction>(64);
    private Action action; //事件
    private Action<int> actionLoop; //带循环次数的时间
    private int cycles;

    //刷新当前循环次数
    internal override void Restart(int cycles)
    {
        this.cycles = cycles;
    }

    internal static ActionNodeAction Get(Action action)
    {
        return opNodeAction.Get().SetAction(action);
    }

    internal static ActionNodeAction Get(Action<int> action)
    {
        return opNodeAction.Get().SetAction(action);
    }

    internal override bool Update(float deltaTime)
    {
        if (null != action)
        {
            action();
        }
        else if (null != actionLoop)
        {
            actionLoop(cycles);
        }

        return true;
    }

    internal override void Release()
    {
        actionLoop = null;
        action = null;
        opNodeAction.Release(this);
    }

    private ActionNodeAction SetAction(Action action)
    {
        actionLoop = null;
        this.action = action;
        return this;
    }

    private ActionNodeAction SetAction(Action<int> action)
    {
        this.action = null;
        actionLoop = action;
        return this;
    }
}