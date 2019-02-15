// /****************************************************************************
//  * Copyright (c) 2018 ZhongShan KPP Technology Co
//  * Date: 2018-07-27 18:14
//  ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealM
{
    /// <summary>
    /// ActionSequenceSystem is a multifunctional chaining timer system
    /// </summary>
    public class ActionSequenceSystem : MonoBehaviour
    {
        internal class SequenceUpdater
        {
            internal readonly List<ActionSequence> listSequenceAlive = new List<ActionSequence>(256);

            internal void Add(ActionSequence sequence)
            {
                if (listSequenceAlive.Contains(sequence))
                {
                    Debug.LogError("ActionSequence 重复");
                    return;
                }

                listSequenceAlive.Add(sequence);
            }

            internal void Stop(Component id)
            {
                for (int i = 0; i < listSequenceAlive.Count; i++)
                {
                    listSequenceAlive[i].Stop(id);
                }
            }

            internal void StopAll()
            {
                for (int i = 0; i < listSequenceAlive.Count; i++)
                {
                    listSequenceAlive[i].Stop();
                }
            }

            internal void Update(float deltaTime)
            {
                //Update Sequence(Auto release)
                bool isSomeSequenceStoped = false;
                for (int i = 0; i < listSequenceAlive.Count; i++)
                {
                    //It's stopped when Update return false and release self
                    isSomeSequenceStoped |= !listSequenceAlive[i].Update(deltaTime);
                }

                //Remove Finshed Sequence(Finshed is Released)
                if (isSomeSequenceStoped)
                {
                    for (int i = 0; i < listSequenceAlive.Count; i++)
                    {
                        ActionSequence actionSequence = listSequenceAlive[i];
                        if (actionSequence.isFinshed)
                        {
                            actionSequence.Release();
                        }
                    }

                    listSequenceAlive.RemoveAll(seq => seq.isFinshed);
                }
            }
        }

        internal static bool isInited;
        internal static bool isQuitting;
        internal static bool isDuplicate;
        internal static ActionSequenceSystem instance;
        private readonly SequenceUpdater updater = new SequenceUpdater();
        private readonly SequenceUpdater updaterUnscale = new SequenceUpdater();

        #region UNITY_EDITOR
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public List<ActionSequence> ListSequenceAlive { get { return updater.listSequenceAlive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public List<ActionSequence> ListSequenceUnscaleAlive { get { return updaterUnscale.listSequenceAlive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countActiveSequence { get { return ActionSequence.opNodes.countActive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countAllSequence { get { return ActionSequence.opNodes.countAll; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countActiveAction { get { return ActionNodeAction.opNodes.countActive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countAllAction { get { return ActionNodeAction.opNodes.countAll; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countActiveInterval { get { return ActionNodeInterval.opNodes.countActive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countAllInterval { get { return ActionNodeInterval.opNodes.countAll; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countActiveSetActive { get { return ActionNodeSetActive.opNodes.countActive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countAllSetActive { get { return ActionNodeSetActive.opNodes.countAll; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countActiveWaitFor { get { return ActionNodeWaitFor.opNodes.countActive; } }
        /// <summary>
        /// Used by the editor. Display relevant information.
        /// </summary>
        public static int countAllWaitFor { get { return ActionNodeWaitFor.opNodes.countAll; } }
        #endregion

        private static void CreateCheck()
        {
            if (isInited || !Application.isPlaying || isQuitting) return;
            isInited = true;
            Create();
        }

        private static void Create()
        {
            if (instance != null) return;

            GameObject go = new GameObject("[ActionSequenceSystem]");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<ActionSequenceSystem>();
        }

        private static ActionSequence GetSequence(Component component = null)
        {
            CreateCheck();
            ActionSequence seq = ActionSequence.GetInstance(component);
            instance.updater.Add(seq);
            return seq;
        }

        private static ActionSequence GetSequenceUnscaled(Component component = null)
        {
            CreateCheck();
            ActionSequence seq = ActionSequence.GetInstance(component);
            instance.updaterUnscale.Add(seq);
            return seq;
        }

        internal static ActionSequence Sequence(Component component)
        {
            return GetSequence(component);
        }

        internal static ActionSequence SequenceUnscaled(Component component)
        {
            return GetSequenceUnscaled(component);
        }

        internal static void StopSequence(Component id)
        {
            if (!instance)
            {
                return;
            }

            instance.updater.Stop(id);
            instance.updaterUnscale.Stop(id);
        }

        #region Unity Methods
        private void Start()
        {
            if (instance != this)
            {
                isDuplicate = true;
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            updater.Update(Time.deltaTime);
            updaterUnscale.Update(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            if (isDuplicate) return;
            if (instance == this) instance = null;
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }
        #endregion

        #region deltaTime
        /// <summary>
        /// 停止所有Sequence
        /// </summary>
        public static void StopSequenceAll()
        {
            if (!instance)
            {
                return;
            }

            instance.updater.StopAll();
            instance.updaterUnscale.StopAll();
        }

        //#region 无ID启动（注意要用Handle手动关闭无限循环的序列，不然机器就会爆炸……）
        /// <summary>
        /// 无ID启动
        /// </summary>
        /// <returns></returns>
        public static ActionSequence Sequence()
        {
            return GetSequence();
        }

        /// <summary>
        /// 延迟调用函数
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Delayer(float delay, Action action)
        {
            return Sequence().Interval(delay).Action(action);
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
        /// 延迟调用函数，循环次数作为参数
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public ActionSequence Looper(float interval, Action<int> action)
        {
            return Sequence().Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(float interval, int loopTime, Action action)
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
        public static ActionSequence Looper(float interval, int loopTime, Action<int> action)
        {
            return Sequence().Interval(interval).Action(action).Loop(loopTime);
        }

        /// <summary>
        /// 循环调用函数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action action)
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
        public static ActionSequence Looper(float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ? 
                Sequence().Action(action).Interval(interval).Loop(loopTime) : 
                Looper(interval, loopTime, action);
        }
        #endregion

        #region unscaledDeltaTime
        //#region 无ID启动（注意要用Handle手动关闭无限循环的序列，不然机器就会爆炸……）
        /// <summary>
        /// 无ID启动
        /// </summary>
        /// <returns></returns>
        public static ActionSequence SequenceUnscaled()
        {
            return GetSequenceUnscaled();
        }

        /// <summary>
        /// 延迟调用函数
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence DelayerUnscaled(float delay, Action action)
        {
            return SequenceUnscaled().Interval(delay).Action(action);
        }

        /// <summary>
        /// 无限循环调用函数
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public ActionSequence LooperUnscaled(float interval, Action action)
        {
            return SequenceUnscaled().Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 延迟调用函数，循环次数作为参数
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public ActionSequence LooperUnscaled(float interval, Action<int> action)
        {
            return SequenceUnscaled().Interval(interval).Action(action).Loop();
        }

        /// <summary>
        /// 循环调用函数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数</param>
        /// <returns></returns>
        public static ActionSequence LooperUnscaled(float interval, int loopTime, bool isActionAtStart, Action action)
        {
            return isActionAtStart ? 
                SequenceUnscaled().Action(action).Interval(interval).Loop(loopTime) : 
                SequenceUnscaled().Interval(interval).Action(action).Loop(loopTime);
        }

        /// <summary>
        /// 循环调用函数，循环次数作为参数，并设置次数，是否立即开始
        /// </summary>
        /// <param name="interval">延迟</param>
        /// <param name="loopTime">循环调用次数</param>
        /// <param name="isActionAtStart">是否立即开始</param>
        /// <param name="action">调用的函数，循环次数作为参数</param>
        /// <returns></returns>
        public static ActionSequence LooperUnscaled(float interval, int loopTime, bool isActionAtStart, Action<int> action)
        {
            return isActionAtStart ? 
                SequenceUnscaled().Action(action).Interval(interval).Loop(loopTime) : 
                SequenceUnscaled().Interval(interval).Action(action).Loop(loopTime);
        }
        #endregion
    }
}