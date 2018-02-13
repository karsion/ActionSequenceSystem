// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-10
// Time: 23:45
// Author: Karsion

using System;

//判定条件节点
public class ActionNodeCondition : ActionNode
{
#if UNITY_EDITOR
    public static void GetObjectPoolInfo(out int countActive, out int countAll)
    {
        countActive = opNodeCondition.countActive;
        countAll = opNodeCondition.countAll;
    }
#endif

    internal static readonly ObjectPool<ActionNodeCondition> opNodeCondition = new ObjectPool<ActionNodeCondition>(64);

    internal Func<bool> condition;

    internal static ActionNodeCondition Get(Func<bool> condition)
    {
        return opNodeCondition.Get().SetCondition(condition);
    }

    private ActionNodeCondition SetCondition(Func<bool> condition)
    {
        this.condition = condition;
        return this;
    }

    internal override bool Update(float deltaTime)
    {
        return condition();
    }

    internal override void Release()
    {
        condition = null;
        opNodeCondition.Release(this);
    }
}