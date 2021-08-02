﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealM
{
    internal class ObjectPool<T> where T : class
    {
        private readonly Stack<T> stack;

        internal delegate T GetT();

        private readonly GetT getT;

        internal ObjectPool()
        {
            stack = new Stack<T>();
        }

        internal ObjectPool(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            stack = new Stack<T>(count);
        }


        internal ObjectPool(int count, GetT getT)
        {
            this.getT = getT;
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            stack = new Stack<T>(count);
        }

        public int countAll { get; private set; }

        public int countActive => countAll - countInactive;

        public int countInactive => stack.Count;

        public T Get()
        {
            T t;
            if (stack.Count == 0)
            {
                t = getT != null ? getT() : Activator.CreateInstance<T>();
                countAll++;
            }
            else
            {
                t = stack.Pop();
            }

            return t;
        }

        public void Release(T element)
        {
            if (stack.Count > 0 && ReferenceEquals(stack.Peek(), element))
            {
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
                return;
            }

            stack.Push(element);
        }
    }
}