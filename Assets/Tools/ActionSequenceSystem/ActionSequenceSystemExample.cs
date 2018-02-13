// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-09
// Time: 14:56
// Author: Karsion

using UnityEngine;

public class ActionSequenceSystemExample : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        //测试一次延迟一秒调用函数
        this.Delayer(1, () => Debug.Log(1));

        //测试一次延迟一秒调用函数
        this.Looper(2, -1, true, () => Debug.Log(-1));

        //测试一次延迟一秒调用函数
        this.Sequence().Interval(1).Action(() => Debug.Log(1));
        //测试三次循环0.5秒调用函数
        this.Sequence().Loop(3).Interval(0.5f).Action(() => Debug.Log(0.5f));
        //测试长链
        this.Sequence()
            .Interval(2)
            .Action(() => Debug.Log("测试长链"))
            .Interval(3)
            .Action(() => Debug.Log("测试长链3"))
            .Interval(1)
            .Action(() => Debug.Log("测试长链结束"))
            ;

        //测试每隔0.2秒，二次条件判断
        this.Sequence()
            .Loop(2)
            .Interval(0.2f)
            .Condition(() => Input.GetKeyDown(KeyCode.F))
            .Action(n => Debug.Log("F键 按下次数" + n));

        //测试无限循环2秒调用函数
        this.Sequence().Loop().Interval(2).Action(() => Debug.Log("Loop2S"));
        //测试无限循环1秒调用函数，并输出循环次数
        this.Sequence().Loop().Interval(1).Action(n => Debug.Log("Loop" + n));
    }

    // Update is called once per frame
    private void Update()
    {
        //测试中途开序列，用transform做ID
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Sequence().Loop(1).Interval(1).Action(() => Debug.Log(1));
        }

        //测试中途序列
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.Sequence().Loop().Interval(1).Action(() => Debug.Log("S"));
        }

        //测试中途停止所有本脚本开启的Sequence
        if (Input.GetKeyDown(KeyCode.D))
        {
            this.StopSequence();
            //Debug.Break();
        }

        //如果中途删除本脚本，关联的Sequence会自动停止并回收到池
    }
}