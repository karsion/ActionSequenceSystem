// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-04-11 10:37
// ***************************************************************************

using UnityEngine;
using UnrealM;

public class ActionSequenceSystemIssue : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        //问题：停止后立即开启，运行列表显示不对
        //问题原因：Stop后，实例立即Release到Pool中，而System里面的运行列表没同步删除元素，
        //接着立即开新Sequence的话，列表又增加一个，此时列表中有两个Sequence，但实际上都是
        //用同一个实例（Pool中只有1个），会导致Update2次
        //解决办法：Release应该和List.Remove同步，就可以了
        //已修复：2018-3-2 10:33:48
        //ActionSequence looper = ActionSequenceSystem.Looper(0.2f, -1, false, () => Debug.Log("No id infinite looper"));
        //looper.Stop();
        //looper = ActionSequenceSystem.Looper(0.3f, -1, false, () => Debug.Log("No id infinite looper"));
        //looper.Stop();
        //looper = ActionSequenceSystem.Looper(0.5f, -1, false, () => Debug.Log("No id infinite looper"));

        //问题：conditionNode执行时，时间轴一直增加
        //问题原因：
        //解决办法：conditionNode执行时，时间轴停止计时，conditionNode执行完，时间轴恢复计时
        //已修复：2018-3-2 11:22:51
        this.Sequence()
            .Loop()
            .Interval(1f)
            .WaitFor(() => Input.GetKeyDown(KeyCode.Q))
            .Action(n => Debug.Log("Q键 按下次数" + n));

        this.Sequence()
            .Loop(5)
            .WaitFor(() => Input.GetKeyDown(KeyCode.W))
            .Action(n => Debug.Log("W键 按下次数" + n))
            .Interval(1f);

        //问题：Sequence执行完之后，把引用它的变量执行停止，会停止到其他东西？
        //问题原因：执行完之后自动回收到池子，这时引用还指向这个Sequence，当其他逻辑调用，
        //获取到这个Sequence做了别的事情的时候，调用引用停止它，就会停止到其他逻辑，务必小心
        //解决办法：加入ActionSequenceHanlde，用handle实例来正确的引用和停止Sequence
        //已修复：2018-4-11 10:36:35
    }

    private void Update()
    {
        //Stop all sequences is ActionSequenceSystem
        if (Input.GetKeyDown(KeyCode.End))
        {
            ActionSequenceSystem.StopSequenceAll();
        }
    }
}