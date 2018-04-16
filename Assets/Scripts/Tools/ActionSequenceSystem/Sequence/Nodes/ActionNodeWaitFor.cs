// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-20 11:38
// ***************************************************************************

using System;
using UnityEngine;

namespace UnrealM
{
    //判定条件节点
    public class ActionNodeWaitFor : ActionNode
    {
        internal static readonly ObjectPool<ActionNodeWaitFor> opNodeWaitFor = new ObjectPool<ActionNodeWaitFor>(64);

        internal Func<bool> condition;
#if UNITY_EDITOR
        public static void GetObjectPoolInfo(out int countActive, out int countAll)
        {
            countActive = opNodeWaitFor.countActive;
            countAll = opNodeWaitFor.countAll;
        }
#endif

        internal static ActionNodeWaitFor Get(Func<bool> condition)
        {
            ActionNodeWaitFor node = opNodeWaitFor.Get();
            node.condition = condition;
            return node;
        }

        internal override bool Update(ActionSequence actionSequence, float deltaTime)
        {
            bool res = false;
            try
            {
                res = condition();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                actionSequence.Stop();
                return true;
            }

            return res;
        }

        internal override void Release()
        {
            condition = null;
            opNodeWaitFor.Release(this);
        }
    }
}