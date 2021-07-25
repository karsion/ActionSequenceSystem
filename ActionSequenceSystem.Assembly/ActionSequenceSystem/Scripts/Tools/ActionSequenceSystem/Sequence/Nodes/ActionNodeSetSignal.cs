namespace UnrealM
{
    internal class ActionNodeSetSignal : ActionNode
    {
        internal static readonly ObjectPool<ActionNodeSetSignal> opNodes = new ObjectPool<ActionNodeSetSignal>(64);
        private bool isToggle;
        private bool isActive;

        internal static ActionNodeSetSignal Get(bool isActive, bool isToggle = false)
        {
            ActionNodeSetSignal active = opNodes.Get();
            active.isActive = isActive;
            active.isToggle = isToggle;
            return active;
        }

        internal override bool Update(ActionSequence sequence, float deltaTime)
        {
            sequence.UpdateTimeAxis(deltaTime);
            if (sequence.handle != null)
            {
                bool final = isToggle ? !sequence.handle.signal : isActive;
                sequence.handle.signal = final;
            }

            return true;
        }

        internal override void Release()
        {
            opNodes.Release(this);
        }
    }
}