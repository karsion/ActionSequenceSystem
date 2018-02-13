// Copyright: ZhongShan KPP Technology Co
// Date: 2018-02-10
// Time: 23:45
// Author: Karsion

//抽象节点
public abstract class ActionNode
{
    protected ActionNode()
    {
    }

    internal abstract bool Update(float deltaTime);

    internal abstract void Release();

    internal virtual void Restart(int cycles)
    {
    }
}