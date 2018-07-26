[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/karsion/ActionSequenceSystem/master/LICENSE.TXT)
[![release](https://img.shields.io/badge/release-v1.0.2-blue.svg)](https://github.com/karsion/ActionSequenceSystem/master/releases)

# ActionSequenceSystem
A Unity3D C# multifunctional timer system<br>
一个U3D C# 多功能计时器系统

## 简要说明 
- 使用了内存池存放序列和节点，一般情况下不会产生GC
- 使用了Component或其子类作为ID受控，一旦物体被销毁，计时器也会随之自动回收
- 一般情况我们启动之后就不去控制停止了，计时器运行完或ID被销毁会自动回收
- <font color=red>如果需要手动停止特定的计时器的话需要引入句柄概念，既Handle，请看使用方法的最后</font>

---

## 设计架构 
### ActionSequenceSystem 序列系统
ActionSequenceSystem（使用了内存池）分配ActionSequence，刷新存活的ActionSequence，自动回收销毁的ActionSequence<br>
### ActionSequence 序列
ActionSequence内部有一个List<ActionNode>容器，增加不同的ActionNode实现不同的行为<br>
ActionSequence对Component实例做了依赖和扩展，使其可以随Component实例的销毁自动回收

### ActionNode 节点
1. Action：执行函数
2. Interval：延迟一段时间
3. SetActive：激活物体/返激活物体
4. WaitFor：知道条件判断为true才跳下一个节点

### 结构图
```
graph TD
ActionSequenceSystem-->ActionSequence
ActionSequence-->ActionNode
Pooling1-->ActionSequence
Pooling2-->ActionNode

ActionNode-->Action
ActionNode-->Interval
ActionNode-->SetActive
ActionNode-->WaitFor
```

---

## 使用方法
说明：下面使用方法中的this的类型为Component或其子类（this is Component）
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
this.Infiniter(1, i => Debug.Log("Infiniter" + i));
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
//Stop all sequences start by this ID
this.StopSequence();

//Allso transform as ID
transform.StopSequence();
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

### ActionSequenceHandle用法
简单来说就是引用一个计时器，让我们可以随时手动停止它<br>
但ActionSequence自身有停止的方法，为什么还要使用ActionSequenceHandle？
1. ActionSequenceSystem内部使用的是内存池来共享ActionSequence实例，ActionSequence运行完之后会自动回收到池子里
2. 比如【对象1】使用ActionSequence seq保存对计时器的引用，seq = this.Delayer(...)，计时器跑完之后回到池子里，这时seq引用的计时器在逻辑已经不再对自己有效，但内存中还保持着引用
3. 接下来【对象2】向系统申请分配一个ActionSequence，取出【对象1】用过还保持引用着的那个ActionSequence实例，【对象2】使用ActionSequence时如果【对象1】去停止seq，就会造成逻辑上的失误，因为自身的计时器早已运行完被系统回收，现在停掉的是别人的计时器
4. 因为使用了内存池，ActionSequenceHandle的引入就是解决上述问题的
``` csharp
public class ActionSequenceHandleExample : MonoBehaviour
{
    private readonly ActionSequenceHandle infiniteSequenceHandle = new ActionSequenceHandle();

    private void Start()
    {
        //Notes：An instance must be preserved to manually stop an infinite loop sequence.
        ActionSequenceSystem.Looper(infiniteSequenceHandle, 0.2f, -1, false, () => Debug.Log("No id infinite looper"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            infiniteSequenceHandle.StopSequence();
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
