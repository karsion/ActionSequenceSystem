// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-20 11:39
// ***************************************************************************

namespace UnrealM
{
    public class ActionNodeSetActive : ActionNode
    {
        //全局池
        private static readonly ObjectPool<ActionNodeSetActive> opNodeHide = new ObjectPool<ActionNodeSetActive>(64);
#if UNITY_EDITOR
        public static void GetObjectPoolInfo(out int countActive, out int countAll)
        {
            countActive = opNodeHide.countActive;
            countAll = opNodeHide.countAll;
        }
#endif
        private bool isToggle;
        private bool isActive;

        internal static ActionNodeSetActive Get(bool isActive, bool isToggle = false)
        {
            ActionNodeSetActive active = opNodeHide.Get();
            active.isActive = isActive;
            active.isToggle = isToggle;
            return active;
        }

        internal override bool Update(ActionSequence actionSequence, float deltaTime)
        {
            actionSequence.UpdateTimeAxis(deltaTime);
            if (actionSequence.id)
            {
                actionSequence.id.gameObject.SetActive(false);
            }

            return true;
        }

        internal override void Release()
        {
            opNodeHide.Release(this);
        }
    }
}