using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnrealM
{
    [CustomEditor(typeof(ActionSequenceSystem))]
    public class ActionSequenceSystemInspector : Editor
    {
        public bool isShowSequenceInfo;
        public bool isShowSequenceUnscaledInfo;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("NodePool State: Active/All", EditorStyles.boldLabel);
            GUILayout.Label($"Action: {ActionSequenceSystem.countActiveAction}/{ActionSequenceSystem.countAllAction}");
            GUILayout.Label($"IAction: {ActionSequenceSystem.countActiveIAction}/{ActionSequenceSystem.countAllIAction}");
            GUILayout.Label($"Interval: {ActionSequenceSystem.countActiveInterval}/{ActionSequenceSystem.countAllInterval}");
            GUILayout.Label($"Condition: {ActionSequenceSystem.countActiveWaitFor}/{ActionSequenceSystem.countAllWaitFor}");
            GUILayout.Label($"Enable: {ActionSequenceSystem.countActiveEnable}/{ActionSequenceSystem.countAllEnable}");
            GUILayout.Label($"SetActive: {ActionSequenceSystem.countActiveSetActive}/{ActionSequenceSystem.countAllSetActive}");
            GUILayout.Label($"Sequence: {ActionSequence.countActive}/{ActionSequence.countAll}");
            EditorGUILayout.Separator();

            //Showing ActionSequences in progress
            GUILayout.Label("ActionSequences in progress", EditorStyles.boldLabel);
            ActionSequenceSystem actionSequenceSystem = target as ActionSequenceSystem;
            isShowSequenceInfo = EditorGUILayout.Foldout(isShowSequenceInfo,
                $"Sequence: {actionSequenceSystem.ListSequenceAlive.Count}");
            if (isShowSequenceInfo)
            {
                UpdateInfo(actionSequenceSystem.ListSequenceAlive);
            }

            Repaint();
        }

        private static void UpdateInfo(List<ActionSequence> listSequenceAlive)
        {
            for (int i = 0; i < listSequenceAlive.Count; i++)
            {
                ActionSequence sequence = listSequenceAlive[i];

                if (sequence.loopTime == 0) //It's a delayer
                {
                    GUILayout.Box($"  ID: {sequence.id}\n  Time({sequence.timeAxis:F2})", "TextArea");
                }
                else if (sequence.loopTime < 0) //It's a infinite looper
                {
                    GUILayout.Box(
                        $"  ID: {sequence.id}\n  Time({sequence.timeAxis:F2})   Loop({sequence.cycles}/{sequence.loopTime})   Node[{sequence.nodesCount}]", "TextArea");
                }
                else //It's a count looper
                {
                    GUILayout.Box(
                        $"  ID: {sequence.id}\n  Time({sequence.timeAxis:F2})   Loop({sequence.cycles}/{sequence.loopTime + 1})   Node[{sequence.nodesCount}]", "TextArea");
                }
            }
        }
    }
}