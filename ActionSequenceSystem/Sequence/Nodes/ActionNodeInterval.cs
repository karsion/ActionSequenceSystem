namespace UnrealM
{
    internal class ActionNodeInterval : ActionNode
    {
        internal static readonly ObjectPool<ActionNodeInterval> opNodes = new ObjectPool<ActionNodeInterval>(64);
        private float interval;

        //从池中获取实例并初始化运行时间
        internal static ActionNodeInterval Get(float interval)
        {
            ActionNodeInterval node = opNodes.Get();
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
            opNodes.Release(this);
        }
    }
}