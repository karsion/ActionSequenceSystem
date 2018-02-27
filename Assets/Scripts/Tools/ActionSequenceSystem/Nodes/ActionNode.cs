// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-26
// Time: 16:21
// Author: Karsion

namespace UnrealM
{
    public abstract class ActionNode
    {
        protected ActionNode()
        {
        }

        internal abstract bool Update(ActionSequence actionSequence);

        internal abstract void Release();
    }
}