using System;
using UnityEngine;

namespace UnrealM
{
    //判定条件节点
    internal class ActionNodeWaitFor : ActionNode
    {
        internal static readonly ObjectPool<ActionNodeWaitFor> opNodes = new ObjectPool<ActionNodeWaitFor>(64);
        private Func<bool> condition;

        internal static ActionNodeWaitFor Get(Func<bool> condition)
        {
            ActionNodeWaitFor node = opNodes.Get();
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
            opNodes.Release(this);
        }
    }
}