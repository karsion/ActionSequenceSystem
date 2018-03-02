// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 9:33
// ***************************************************************************

using UnityEditor;
using UnityEngine;

namespace UnrealM
{
    [CustomEditor(typeof(ActionSequenceSystem))]
    public class ActionSequenceSystemInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            int countAll, countActive;
            GUILayout.Label("ObjectPool State: Active/All", EditorStyles.boldLabel);
            ActionNodeAction.GetObjectPoolInfo(out countActive, out countAll);
            GUILayout.Label(string.Format("ActionNode: {0}/{1}", countActive, countAll));
            ActionNodeInterval.GetObjectPoolInfo(out countActive, out countAll);
            GUILayout.Label(string.Format("IntervalNode: {0}/{1}", countActive, countAll));
            ActionNodeCondition.GetObjectPoolInfo(out countActive, out countAll);
            GUILayout.Label(string.Format("ConditionNode: {0}/{1}", countActive, countAll));
            ActionSequence.GetObjectPoolInfo(out countActive, out countAll);
            GUILayout.Label(string.Format("Sequence: {0}/{1}", countActive, countAll));

            //Showing ActionSequences in progress
            ActionSequenceSystem actionSequenceSystem = target as ActionSequenceSystem;
            GUILayout.Label(string.Format("Sequence Count: {0}", actionSequenceSystem.ListSequenceAlive.Count), EditorStyles.boldLabel);
            for (int i = 0; i < actionSequenceSystem.ListSequenceAlive.Count; i++)
            {
                ActionSequence sequence = actionSequenceSystem.ListSequenceAlive[i];
                if (!sequence.isFinshed)
                {
                    if (sequence.loopTime == 0) //It's a delayer
                    {
                        GUILayout.Box(string.Format("  ID: {0}\n  Time({1:F2})", sequence.id, sequence.timeAxis), "TextArea");
                    }
                    else if (sequence.loopTime < 0)//It's a infinite looper
                    {
                        GUILayout.Box(string.Format("  ID: {0}\n  Time({1:F2})   Loop({2}/{3})   Node[{4}]", sequence.id, sequence.timeAxis,
                                                    sequence.cycles, sequence.loopTime, sequence.nodes.Count), "TextArea");
                    }
                    else//It's a count looper
                    {
                        GUILayout.Box(string.Format("  ID: {0}\n  Time({1:F2})   Loop({2}/{3})   Node[{4}]", sequence.id, sequence.timeAxis,
                            sequence.cycles, sequence.loopTime + 1, sequence.nodes.Count), "TextArea");
                    }
                }
            }

            Repaint();
        }
    }
}