using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools.Anim
{
    [Serializable]
    public class AnimatorInvokerEventContainer
    {

#pragma warning disable

        [SerializeField] private List<AnimatorInvoker> invokers;

#pragma warning restore

        private int finishedCount = 0;

        public void Invoke()
        {
            foreach (AnimatorInvoker invoker in invokers)
            {
                invoker.InvokeDefault();
            }
        }

        public void Invoke(Action finishCallback)
        {
            if (invokers.Count == 0)
            {
                finishCallback?.Invoke();
            }
            else
            {
                int invokersCount = invokers.Count(invoker => invoker);

                void Action()
                {
                    finishedCount++;
                    if (finishedCount == invokersCount)
                    {
                        finishCallback.Invoke();
                        finishedCount = 0;
                    }
                }

                foreach (AnimatorInvoker invoker in invokers)
                {
                    invoker.InvokeDefault(Action);
                }
            }
        }
    }
}