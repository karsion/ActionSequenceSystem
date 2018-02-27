// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-26
// Time: 16:22
// Author: Karsion

namespace UnrealM
{
    public class ActionNodeInterval : ActionNode
    {
        private static readonly ObjectPool<ActionNodeInterval> opNodeInterval = new ObjectPool<ActionNodeInterval>(64);

        private float interval;
#if UNITY_EDITOR
        public static void GetObjectPoolInfo(out int countActive, out int countAll)
        {
            countActive = opNodeInterval.countActive;
            countAll = opNodeInterval.countAll;
        }
#endif

        //从池中获取实例并初始化运行时间
        internal static ActionNodeInterval Get(float interval)
        {
            return opNodeInterval.Get().SetInterval(interval);
        }

        private ActionNodeInterval SetInterval(float interval)
        {
            this.interval = interval;
            return this;
        }

        internal override bool Update(ActionSequence actionSequence)
        {
            if (actionSequence.timeAxis > interval)
            {
                actionSequence.UpdateTimeAxis(interval);
                return true;
            }

            return false;
        }

        internal override void Release()
        {
            opNodeInterval.Release(this);
        }
    }
}