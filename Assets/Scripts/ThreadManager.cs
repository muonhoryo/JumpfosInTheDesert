using Registry;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Registry
{
    public sealed class ThreadManager : MonoBehaviour, ISingltone<ThreadManager>
    {
        public static object locker=new object();
        public sealed class ThreadQueryReservator
        {
            public ThreadQueryReservator() 
            {
                Id = NextId++;
            }
            public readonly short Id;
            public bool isBeingUsed { get; private set; } = false;
            public void SetUse() 
            {
                isBeingUsed = true;
            }
        }
        private static short NextId = short.MinValue;
        private sealed class ThreadActionQueve : IEnumerable<Action>
        {
            private sealed class ThreadActionQueveEnumerator : IEnumerator<Action>
            {
                private ThreadActionQueveEnumerator() { }
                public ThreadActionQueveEnumerator(List<Action> ActionsQueve)
                {
                    this.ActionsQueve = ActionsQueve;
                }
                private Action GetCurrentAction()
                {
                    if (ActionsQueve.Count <= 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    Action action = ActionsQueve[0];
                    ActionsQueve.RemoveAt(0);
                    return action;
                }
                object IEnumerator.Current => GetCurrentAction();
                Action IEnumerator<Action>.Current => GetCurrentAction();
                bool IEnumerator.MoveNext()
                {
                    if (ActionsQueve.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                void IEnumerator.Reset() { }
                void IDisposable.Dispose() { }
                private readonly List<Action> ActionsQueve;
            }
            private ThreadActionQueve() { }
            public ThreadActionQueve(List<Action> actionsList, Action OnQueveDone) : this(actionsList, OnQueveDone, NextId++) { }
            public ThreadActionQueve(List<Action> actionsList,Action OnQueryDone,short id)
            {
                QueveEnumerator = new ThreadActionQueveEnumerator(actionsList);
                OnQueveDone = OnQueryDone;
                QueveId = id;
            }
            private readonly ThreadActionQueveEnumerator QueveEnumerator;
            IEnumerator<Action> IEnumerable<Action>.GetEnumerator() => QueveEnumerator;
            IEnumerator IEnumerable.GetEnumerator() => QueveEnumerator;
            public readonly short QueveId;
            public readonly Action OnQueveDone;
        }
        readonly List<ThreadActionQueve> ThreadActionsQueve = new List<ThreadActionQueve> { };
        static ThreadManager Singltone;
        ThreadManager ISingltone<ThreadManager>.Singltone { get => Singltone; set => Singltone = value; }
        public short AddActionsQuery(List<Action> actionsList, Action onQueveDoneAction)
        {
            lock (locker)
            {
                ThreadActionQueve queve = new ThreadActionQueve(actionsList, onQueveDoneAction);
                ThreadActionsQueve.Add(queve);
                return queve.QueveId;
            }
        }
        public void AddActionsQuery(List<Action> actionsList, Action onQueveDoneAction,ThreadQueryReservator reservator)
        {
            if (!reservator.isBeingUsed)
            {
                lock (locker)
                {
                    ThreadActionQueve queve = new ThreadActionQueve(actionsList, onQueveDoneAction, reservator.Id);
                    ThreadActionsQueve.Add(queve);
                    reservator.SetUse();
                }
            }
        }
        public void RemoveActionsQueve(short queveId)
        {
            lock (locker)
            {
                for (int i = 0; i < ThreadActionsQueve.Count; i++)
                {
                    if (ThreadActionsQueve[i].QueveId == queveId)
                    {
                        ThreadActionsQueve.RemoveAt(i);
                    }
                }
            }
        }
        private void Awake()
        {
            SingltoneStatic.Awake(this, delegate { Destroy(this); }, delegate { });
        }
        private void Update()
        {
            while (ThreadActionsQueve.Count > 0)
            {
                lock (locker)
                {
                    foreach (Action action in ThreadActionsQueve[0])
                    {
                        action();
                        if (Time.timeSinceLevelLoad > Registry.СonstData.ThreadManagerMaxTimeFrameWork)
                        {
                            return;
                        }
                    }
                    ThreadActionsQueve[0].OnQueveDone.Invoke();
                    ThreadActionsQueve.RemoveAt(0);
                }
            }
        }
    }
}