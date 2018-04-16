// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 9:34
// ***************************************************************************

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
            ActionNodeInterval node = opNodeInterval.Get();
            node.interval = interval;
            return node;
        }

        internal override bool Update(ActionSequence actionSequence, float deltaTime)
        {
            actionSequence.UpdateTimeAxis(deltaTime);
            if (actionSequence.timeAxis > interval)
            {
                actionSequence.UpdateTimeAxis(-interval);
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