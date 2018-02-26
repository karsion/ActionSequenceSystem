// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-26
// Time: 11:38
// Author: Karsion

using System;

//判定条件节点
public class ActionNodeCondition : ActionNode
{
    internal static readonly ObjectPool<ActionNodeCondition> opNodeCondition = new ObjectPool<ActionNodeCondition>(64);

    internal Func<bool> condition;
#if UNITY_EDITOR
    public static void GetObjectPoolInfo(out int countActive, out int countAll)
    {
        countActive = opNodeCondition.countActive;
        countAll = opNodeCondition.countAll;
    }
#endif

    internal static ActionNodeCondition Get(Func<bool> condition)
    {
        return opNodeCondition.Get().SetCondition(condition);
    }

    private ActionNodeCondition SetCondition(Func<bool> condition)
    {
        this.condition = condition;
        return this;
    }

    internal override bool Update(ActionSequence actionSequence)
    {
        return condition();
    }

    internal override void Release()
    {
        condition = null;
        opNodeCondition.Release(this);
    }
}