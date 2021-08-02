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