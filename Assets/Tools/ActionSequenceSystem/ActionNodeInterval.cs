// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-13
// Time: 14:41
// Author: Karsion

public class ActionNodeInterval : ActionNode
{
#if UNITY_EDITOR
    public static void GetObjectPoolInfo(out int countActive, out int countAll)
    {
        countActive = opNodeInterval.countActive;
        countAll = opNodeInterval.countAll;
    }
#endif

    private static readonly ObjectPool<ActionNodeInterval> opNodeInterval = new ObjectPool<ActionNodeInterval>(64);

    private float interval;
    private float timeline;

    //从池中获取实例并初始化运行时间
    internal static ActionNodeInterval Get(float interval)
    {
        return opNodeInterval.Get().SetInterval(interval);
    }

    private ActionNodeInterval SetInterval(float interval)
    {
        this.interval = interval;
        timeline = interval;
        return this;
    }

    internal override bool Update(float deltaTime)
    {
        return (timeline -= deltaTime) < 0;
    }

    //释放回池子
    internal override void Release()
    {
        opNodeInterval.Release(this);
    }

    internal override void Restart()
    {
        timeline = interval;
    }
}