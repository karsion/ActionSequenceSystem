// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 9:34
// ***************************************************************************

namespace UnrealM
{
    internal abstract class ActionNode
    {
        protected ActionNode()
        {
        }

        internal abstract bool Update(ActionSequence actionSequence, float deltaTime);

        internal abstract void Release();
    }
}