// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-20 11:39
// ***************************************************************************

using System;
using UnityEngine;

namespace UnrealM
{
    //事件节点
    public class ActionNodeAction : ActionNode
    {
        //全局池
        private static readonly ObjectPool<ActionNodeAction> opNodeAction = new ObjectPool<ActionNodeAction>(64);
        private Action action; //事件
        private Action<int> actionLoop; //带循环次数的时间
#if UNITY_EDITOR
        public static void GetObjectPoolInfo(out int countActive, out int countAll)
        {
            countActive = opNodeAction.countActive;
            countAll = opNodeAction.countAll;
        }
#endif

        internal static ActionNodeAction Get(Action action)
        {
            return opNodeAction.Get().SetAction(action);
        }

        internal static ActionNodeAction Get(Action<int> action)
        {
            return opNodeAction.Get().SetAction(action);
        }

        internal override bool Update(ActionSequence actionSequence, float deltaTime)
        {
            actionSequence.UpdateTimeAxis(deltaTime);
            if (null != action)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    actionSequence.Stop();
                }
            }
            else if (null != actionLoop)
            {
                try
                {
                    actionLoop(actionSequence.cycles);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    actionSequence.Stop();
                }
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
}