// ***************************************************************************
// Copyright (c) 2018 ZhongShan KPP Technology Co
// Copyright (c) 2018 Karsion
//   
// https://github.com/karsion
// Date: 2018-03-02 10:09
// ***************************************************************************

using UnityEngine;
using UnrealM;

public class ActionSequenceSystemExample : MonoBehaviour
{

    public Transform tfShowHideExample;
    // Use this for initialization
    private void Start()
    {
        //Start a once timer
        this.Delayer(1, () => Debug.Log(1));
        this.Sequence().Interval(1).Action(() => Debug.Log(1)); //Same

        //Allso use transform as a ID to start a sequence
        transform.Delayer(1, () => Debug.Log(1));

        //Start a loop timer
        this.Looper(0.5f, 3, false, () => Debug.Log(-1));
        this.Sequence().Loop(3).Interval(0.5f).Action(() => Debug.Log(-1)); //Same

        //Start a infinite loop timer
        this.Looper(1, i => Debug.Log("Infiniter" + i));

        //Start a long sequence
        this.Sequence()
            .Interval(2)
            .Action(() => Debug.Log("Test1"))
            .Interval(3)
            .Action(() => Debug.Log("Test2"))
            .Interval(1)
            .Action(() => Debug.Log("Test3 end"))
            ;

        //Check Q key per 0.2 seconds
        this.Sequence()
            .Loop()
            .Interval(1f)
            .WaitFor(() => Input.GetKeyDown(KeyCode.Q))
            .Action(n => Debug.Log("Q键 按下次数" + n));

        //Start a sequence without id.
        ActionSequenceSystem.Delayer(5, () => Debug.Log("No id delayer"));
        ActionSequenceSystem.Looper(0.2f, 10, false, () => Debug.Log("No id looper"));

        //Start a toggle GameObject active sequence
        tfShowHideExample.Hider(0.5f);
        tfShowHideExample.Sequence().Interval(0.5f).ToggleActive().Loop();
    }

    // Update is called once per frame
    private void Update()
    {
        //Start a loop in Update, using transform as ID
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Looper(1, -1, true, count => Debug.Log("A" + count));
        }

        //Start a loop in Update
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.Looper(1, -1, true, count => Debug.Log("S" + count));
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            this.Sequence().Interval(1).Action(() => this.Sequence().Interval(1).Action(() => Debug.Log("C")))
                .Interval(2)
                .Action(i => Debug.Log(i));
        }

        //Stop all sequences start by this ID
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.StopSequence();
        }

        //Stop all sequences start by transform
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.StopSequence();
        }

        //Stop all sequences is ActionSequenceSystem
        if (Input.GetKeyDown(KeyCode.End))
        {
            Debug.Log("手动停止所有序列");
            ActionSequenceSystem.StopSequenceAll();
        }

        //If this Component is destroyed, the associated Sequence will automatically stop and recycle to the pool.
    }
}