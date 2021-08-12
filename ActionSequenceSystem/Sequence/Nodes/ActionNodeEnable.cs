using UnityEngine;

namespace UnrealM
{
	internal class ActionNodeEnable : ActionNode
	{
		internal static readonly ObjectPool<ActionNodeEnable> opNodes = new ObjectPool<ActionNodeEnable>(64);
		private bool isToggle;
		private bool isEnable;

		internal static ActionNodeEnable Get(bool isEnable, bool isToggle = false)
		{
			ActionNodeEnable active = opNodes.Get();
			active.isEnable = isEnable;
			active.isToggle = isToggle;
			return active;
		}

		internal override bool Update(ActionSequence actionSequence, float deltaTime)
		{
			actionSequence.UpdateTimeAxis(deltaTime);
			Behaviour behaviour = actionSequence.id as Behaviour;
			if (behaviour)
			{
				bool final = isToggle ? !behaviour.enabled : isEnable;
                behaviour.enabled = final;
            }

			return true;
		}

		internal override void Release()
		{
			opNodes.Release(this);
		}
	}
}