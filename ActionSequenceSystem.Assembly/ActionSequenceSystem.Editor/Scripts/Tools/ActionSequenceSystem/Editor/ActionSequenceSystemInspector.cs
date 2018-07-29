// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Date: 2018-07-29 8:55
//  ****************************************************************************/

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
            GUILayout.Label(string.Format("Action: {0}/{1}", ActionSequenceSystem.countActiveAction, ActionSequenceSystem.countAllAction));
            GUILayout.Label(string.Format("Interval: {0}/{1}", ActionSequenceSystem.countActiveInterval, ActionSequenceSystem.countAllInterval));
            GUILayout.Label(string.Format("Condition: {0}/{1}", ActionSequenceSystem.countActiveWaitFor, ActionSequenceSystem.countAllWaitFor));
            GUILayout.Label(string.Format("SetActive: {0}/{1}", ActionSequenceSystem.countActiveSetActive, ActionSequenceSystem.countAllSetActive));
            GUILayout.Label(string.Format("Sequence: {0}/{1}", ActionSequence.countActive, ActionSequence.countAll));
            EditorGUILayout.Separator();

            //Showing ActionSequences in progress
            GUILayout.Label("ActionSequences in progress", EditorStyles.boldLabel);
            ActionSequenceSystem actionSequenceSystem = target as ActionSequenceSystem;
            isShowSequenceInfo = EditorGUILayout.Foldout(isShowSequenceInfo, string.Format("Sequence: {0}", actionSequenceSystem.ListSequenceAlive.Count));
            if (isShowSequenceInfo)
            {
                UpdateInfo(actionSequenceSystem.ListSequenceAlive);
            }

            isShowSequenceUnscaledInfo = EditorGUILayout.Foldout(isShowSequenceUnscaledInfo, string.Format("SequenceUnscaled: {0}", actionSequenceSystem.ListSequenceUnscaleAlive.Count));
            if (isShowSequenceUnscaledInfo)
            {
                UpdateInfo(actionSequenceSystem.ListSequenceUnscaleAlive);
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
                    GUILayout.Box(string.Format("  ID: {0}\n  Time({1:F2})", sequence.id, sequence.timeAxis), "TextArea");
                }
                else if (sequence.loopTime < 0) //It's a infinite looper
                {
                    GUILayout.Box(string.Format("  ID: {0}\n  Time({1:F2})   Loop({2}/{3})   Node[{4}]", sequence.id, sequence.timeAxis,
                                                sequence.cycles, sequence.loopTime, sequence.nodesCount), "TextArea");
                }
                else //It's a count looper
                {
                    GUILayout.Box(string.Format("  ID: {0}\n  Time({1:F2})   Loop({2}/{3})   Node[{4}]", sequence.id, sequence.timeAxis,
                                                sequence.cycles, sequence.loopTime + 1, sequence.nodesCount), "TextArea");
                }
            }
        }
    }
}