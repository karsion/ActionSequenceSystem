[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/karsion/ActionSequenceSystem/master/LICENSE.TXT)
[![release](https://img.shields.io/badge/release-v1.0.2-blue.svg)](https://github.com/karsion/ActionSequenceSystem/master/releases)

！！！Mac端的使用没问题，但是iOS打包之后无法使用，异常为MissingMethodexception：default constructor not found for type UnrealM.ActionNodeInterval
！！！Mac use of the Mac side is no problem, but the iOS can not be used after packaging, return MissingMethodexception：default constructor not found for type UnrealM.ActionNodeInterval
！！！未支持Editor模式
！！！Editor model not supported

# ActionSequenceSystem
A Unity3D C# multifunctional chaining timer system<br>
一个U3D C# 多功能链式计时器
- 支持Unity Package Manager，在manifest.json中添加以下代码
- "com.jpp.actionsequence": "https://github.com/karsion/ActionSequenceSystem.git#1.0.3",

## 简要说明 
- 简单上手：在脚本中使用this作为入口，e.g. this.Delayer(1, DoSomething);
- 自定义：e.g. this.Sequence().Loop(2).Interval(1).Action(DoSomething1).Interval(0.5f).Action(DoSomething2);
- 无GC：使用了内存池存放序列和节点，在数组容量足够的情况下，运行时无GC（数组自动扩容会产生GC）
- 自动回收：使用Component或其子类作为ID受控，一旦ID被销毁，计时器会随之自动回收
- 精准受控：准确停止指定的计时器（请看使用方法的最后）
- 支持时间缩放：支持deltaTime和unscaledDeltaTime

---

## 设计架构 
### ActionSequenceSystem 序列系统
- Node都使用了内存池
- 支持Time.unscaledDeltaTime

### ActionSequence 序列
- 对Component做了扩展，在脚本中使用this作为入口 
- 对Component实例做了依赖，使其可以随Component实例的销毁自动回收
- 可自定义，加入不同的ActionNode实现不同的行为链

### ActionNode 节点
- Action：执行函数
- Interval：延迟一段时间
- SetActive：激活物体/返激活物体
- WaitFor：知道条件判断为true才跳下一个节点
- SetActive：激活GameObject，ID用的是gameObject.transform
- Enable：激活Behaviour
- IAction：调用IAction接口，使用ID区分多路事件。通常委托都会造成GC，需要此接口优化。

---

## 使用方法
说明：下面使用方法中的this的类型为Component或其子类（其实this is MonoBehaviour）
### 延迟开关GameObject功能
既然用了Component来做ID，控制gameObject显示隐藏就是举手之劳，何乐而不为
``` csharp
//Start a toggle gameObject active/deactive sequence
tfShowHideExample.Shower(0.5f);
tfShowHideExample.Hider(1.5f);

//Start a infinite loop toggle active gameObject
tfShowHideExample.Sequence().Interval(0.5f).ToggleActive().Loop();
```

### 开启单次计时器（延迟）
``` csharp
//Start a once timer
this.Delayer(1, () => Debug.Log(1));
this.Sequence().Interval(1).Action(() => Debug.Log(1));//Same

//Allso use transform as a ID to start a sequence
transform.Delayer(1, () => Debug.Log(1));
```

### 开启计次计时器（次数)
``` csharp
//Start a loop timer
this.Looper(0.5f, 3, false, () => Debug.Log(-1));
this.Sequence().Loop(3).Interval(0.5f).Action(() => Debug.Log(-1));//Same
```

### 开启无限计时器
相当于计次计时器的缩写，循环次数设置为-1
``` csharp
//Start a infinite loop timer
this.Looper(1, i => Debug.Log("Infiniter" + i));
```

### 开启自定义序列
``` csharp
//Start a custom sequence
this.Sequence()
    .Interval(2)
    .Action(() => Debug.Log("Test1"))
    .Interval(3)
    .Action(() => Debug.Log("Test2"))
    .Interval(1)
    .Action(() => Debug.Log("Test3 end"));

//Check Q key per 0.2 seconds
this.Sequence()
    .Loop()
    .Interval(0.2f)
    .WaitFor(() => Input.GetKeyDown(KeyCode.Q))
    .Action(n => Debug.Log("Q键 按下次数" + n));
```

### 停止序列
``` csharp
//Stop the sequence with the specified ID
private ActionSequence sequence;
sequence = this.Looper(0.5f, 3, false, () => Debug.Log(-1));
sequence.Stop(this);

//Stop all sequences by this ID
this.StopSequence();

//Allso transform as ID
transform.StopSequence();
```

### IAction
``` csharp
public class User : MonoBehaviour, IAction
{
	private int times;
	public void OnEnable()
	{
		this.Delayer(1, this, 0);
		this.Looper(1, this, 1);
	}

	public void Action(int id, int loopTime)
	{
		switch (id)
		{
			case 0:
				//Do something when id is 0
				break;
			case 1:
				times++;
				Debug.Log(times);
				break;
		}
	}
}
```

### 可以不用ID直接开序列
``` csharp
//Start a sequence without id.
ActionSequenceSystem.Delayer(5, () => Debug.Log("No id delayer"));
ActionSequenceSystem.Looper(0.2f, 10, false, () => Debug.Log("No id looper"));

//Notes：An instance must be preserved to manually stop an infinite loop sequence.
ActionSequenceHandle infiniteSequenceHandle = new ActionSequenceHandle();
ActionSequenceSystem.Looper(0.2f, -1, false, () => Debug.Log("No id infinite looper")).SetHandle(infiniteSequenceHandle);
infiniteSequenceHandle.StopSequence();
```

### 停止序列特殊技巧：ActionSequenceHandle用法
简单来说就是引用一个计时器，让我们可以随时手动停止它<br>
但ActionSequence自身有停止的方法，为什么还要使用ActionSequenceHandle？<br>
ActionSequenceHandle是为了处理“开启全局计时器时不传入ID，又要控制这个计时器”的情况
``` csharp
public class ActionSequenceHandleExample : MonoBehaviour
{
    private readonly ActionSequenceHandle infiniteSequenceHandle = new ActionSequenceHandle();
    private ActionSequence sequence;
    
    private void Start()
    {
        //Notes：An instance must be preserved to manually stop an infinite loop sequence.
        ActionSequenceSystem.Looper(infiniteSequenceHandle, 0.2f, -1, false, () => Debug.Log("No id infinite looper"));
        sequence = this.Looper(0.5f, 3, false, () => Debug.Log("this looper"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //使用ActionSequenceHandle停止
            infiniteSequenceHandle.StopSequence();
            //使用ActionSequence停止
            sequence.Stop(this);
            this.StopSequence(sequence);//Same as sequence.Stop(this);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.Looper(infiniteSequenceHandle, 1, 5, false, i =>
            {
                if (i == 2)
                {
                    infiniteSequenceHandle.StopSequence();
                }
            });
        }
    }
}
```
