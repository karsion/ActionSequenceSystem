// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-10
// Time: 22:01
// Author: Karsion

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionSequenceSystem))]
public class ActionSequenceSystemInspector : Editor
{
    //The timing function, callback function and judgment function are realized through the node.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int countAll,countActive;
        GUILayout.Label("ObjectPool State: Active/Inactive(All)", EditorStyles.boldLabel);
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
        GUILayout.Label(string.Format("Sequence Count: {0}", actionSequenceSystem.ListSequence.Count), EditorStyles.boldLabel);
        for (int i = 0; i < actionSequenceSystem.ListSequence.Count; i++)
        {
            if (!actionSequenceSystem.ListSequence[i].isFinshed)
            {
                GUILayout.Box(string.Format("{0} id: {1}\n Loop:{2}/{3}", i, actionSequenceSystem.ListSequence[i].id,
                    actionSequenceSystem.ListSequence[i].nRunLoopTime, actionSequenceSystem.ListSequence[i].nLoopTime), "TextArea");
            }
        }

        Repaint();
    }
}