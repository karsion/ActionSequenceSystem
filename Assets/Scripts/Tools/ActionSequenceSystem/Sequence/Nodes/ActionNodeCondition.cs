// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 9:34
// ***************************************************************************

using System;

namespace UnrealM
{
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

        //internal override void Start(ActionSequence actionSequence)
        //{
        //    actionSequence.isStopTimeAxis = true;
        //}

        internal override bool Update(ActionSequence actionSequence, float deltaTime)
        {
            return condition();
        }

        internal override void Release()
        {
            condition = null;
            opNodeCondition.Release(this);
        }
    }
}