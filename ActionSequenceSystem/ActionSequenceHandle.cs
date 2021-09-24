using System;

namespace UnrealM
{
    /// <summary>
    /// 这个类设计为单独控制Sequence的停止，如果你想手动停止的话
    /// 也可以缓存Sequence，使用Sequence停止时，需要判断是否正在使用当前ID
    /// 目前设计为一对一控制，重复SetHandle的话，会丢失对上一个Sequence的控制权
    /// 考虑到如果设计为一对多的话，内部需要保存一个数组，会浪费内存，一对多的使用情况也很少，故不支持
    /// </summary>
    public class ActionSequenceHandle : IDisposable
    {
        internal ActionSequence sequence;
        /// <summary>
        /// 
        /// </summary>
        public bool signal { get; internal set; }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose() { StopSequence(); }

        /// <summary>
        /// 停止sequence
        /// </summary>
        /// <localize>
        /// <zh-CN>停止当前控制的sequence</zh-CN>
        /// <en>Stop the handling sequence</en>
        /// </localize>
        public void StopSequence()
        {
            sequence?.Stop();
        }

        /// <summary>
        /// 用Handle开序列
        /// </summary>
        /// <returns></returns>
        public ActionSequence Sequence()
        {
            return ActionSequenceSystem.Sequence().SetHandle(this);
        }

        /// <summary>
        /// 延迟调用IAction接口
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <param name="actionId">用于区分多路回调</param>
        /// <returns></returns>
        public ActionSequence Delayer(float delay, IAction action, int actionId)
        {
            return Sequence().Interval(delay).IAction(action, actionId);
        }

        /// <summary>
        /// 延迟调用函数
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public ActionSequence Delayer(float delay, Action action)
        {
            return Sequence().Interval(delay).Action(action);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="signal"></param>
        /// <returns></returns>
        public ActionSequence DelaySignal(float delay, bool signal = true)
        {
            return Sequence().Interval(delay).SetHandleSignal(signal);
        }

        public ActionSequenceHandle SetSignal(bool signal = true)
        {
            this.signal = signal;
            return this;
        }

        public void ResetSignal()
        {
            this.signal = false;
        }

        /// <summary>
        /// 延迟调用IAction接口，循环次数作为参数
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <param name="actionId">用于区分多路回调</param>
        /// <returns></returns>
        public ActionSequence Looper(float delay, IAction action, int actionId)
        {
            return Sequence().Interval(delay).IAction(action, actionId).Loop();
        }

        /// <summary>
        /// 无限循环调用函数
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, Action action)
        {
            return Sequence().Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 无限循环调用函数，循环次数作为参数
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, Action<int> action)
        {
            return Sequence().Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 循环调用函数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return isActionAtStart ?
                Sequence().Action(action).Interval(interval).Loop(loopTime) :
                Looper(interval, loopTime, action);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ?
                Sequence().Action(action).Interval(interval).Loop(loopTime) :
                Looper(interval, loopTime, action);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, int loopTime, Action action)
        {
            return Sequence().Interval(interval).Action(action).Loop(loopTime);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, int loopTime, Action<int> action)
        {
            return Sequence().Interval(interval).Action(action).Loop(loopTime);
        }
    }
}