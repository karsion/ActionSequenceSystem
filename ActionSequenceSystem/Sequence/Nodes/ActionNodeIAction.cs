using System;
using UnityEngine;

namespace UnrealM
{
	/// <summary>
	///     支持回调接口
	/// </summary>
	public interface IAction
	{
		/// <summary>
		/// </summary>
		/// <param name="id">用于区分多路回调</param>
		/// <param name="loopTime">循环次数，选用</param>
		void Action(int id, int loopTime);
	}

	internal class ActionNodeIAction : ActionNode
	{
		//全局池
		internal static readonly ObjectPool<ActionNodeIAction> opNodes = new ObjectPool<ActionNodeIAction>(64);
		private IAction action; //事件
		private int id;

		internal static ActionNodeIAction Get(IAction action, int id)
		{
			ActionNodeIAction node = opNodes.Get();
			node.id = id;
			node.action = action;
			return node;
		}

		internal override bool Update(ActionSequence actionSequence, float deltaTime)
		{
			actionSequence.UpdateTimeAxis(deltaTime);
			if (null != action)
			{
				try { action.Action(id, actionSequence.cycles); }
				catch (Exception e)
				{
					Debug.LogException(e);
					actionSequence.Stop();
				}
			}

			return true;
		}

		internal override void Release()
		{
			action = null;
			opNodes.Release(this);
		}
	}
}