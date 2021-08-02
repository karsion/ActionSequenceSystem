namespace UnrealM
{
    internal class ActionNodeSetActive : ActionNode
    {
        internal static readonly ObjectPool<ActionNodeSetActive> opNodes = new ObjectPool<ActionNodeSetActive>(64);
        private bool isToggle;
        private bool isActive;

        internal static ActionNodeSetActive Get(bool isActive, bool isToggle = false)
        {
            ActionNodeSetActive active = opNodes.Get();
            active.isActive = isActive;
            active.isToggle = isToggle;
            return active;
        }

        internal override bool Update(ActionSequence actionSequence, float deltaTime)
        {
            actionSequence.UpdateTimeAxis(deltaTime);
            if (actionSequence.id)
            {
                bool final = isToggle ? !actionSequence.id.gameObject.activeSelf : isActive;
                actionSequence.id.gameObject.SetActive(final);
            }

            return true;
        }

        internal override void Release()
        {
            opNodes.Release(this);
        }
    }
}