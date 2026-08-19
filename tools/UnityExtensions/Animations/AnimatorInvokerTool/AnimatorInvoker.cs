using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Tools.Anim
{
    public class AnimatorInvoker : MonoBehaviour
    {
        private static Dictionary<AnimatorControllerParameterTypeFunc, Action<Animator, AnimatorAction>> defaultActionsDictionary =
            new Dictionary<AnimatorControllerParameterTypeFunc, Action<Animator, AnimatorAction>>()
            {
                { AnimatorControllerParameterTypeFunc.Bool, (a, p) => { a.SetBool(p.Value.Parameter.Name, (bool)p.Value.Value); } },
                { AnimatorControllerParameterTypeFunc.Float, (a, p) => { a.SetFloat(p.Value.Parameter.Name, (float)p.Value.Value); } },
                { AnimatorControllerParameterTypeFunc.Int, (a, p) => { a.SetInteger(p.Value.Parameter.Name, (int)p.Value.Value); } },
                { AnimatorControllerParameterTypeFunc.TriggerSet, (a, p) => { a.SetTrigger(p.Value.Parameter.Name); } },
                { AnimatorControllerParameterTypeFunc.TriggerReset, (a, p) => { a.ResetTrigger(p.Value.Parameter.Name); } }
            };

#pragma warning disable

        [SerializeField] private Animator animator;
        [SerializeField] private List<AnimatorAction> list;

#pragma warning restore

        private bool isFinishInvoking = true;
        private Coroutine currentCoroutine;
        private Action finishInvokeCallback;

        public virtual void Interrupt()
        {
            OnDisable();
        }

        public virtual void InvokeDefault()
        {
            if (isFinishInvoking)
            {
                isFinishInvoking = false;
                ContinueInvoke(0);
            }
        }

        public virtual void InvokeDefault(Action finishCallback)
        {
            finishInvokeCallback = finishCallback;
            InvokeDefault();
        }

        private IEnumerator DelayCoroutine(float delayTime, int index)
        {
            yield return new WaitForSeconds(delayTime);
            currentCoroutine = null;
            ContinueInvoke(index);
        }

        private void ContinueInvoke(int index)
        {
            for (; index < list.Count; index++)
            {
                if (list[index].ActionType == AnimatorActionType.Animation)
                {
                    defaultActionsDictionary[list[index].ParameterTypeFunc](animator, list[index]);
                }
                else
                {
                    currentCoroutine = StartCoroutine(DelayCoroutine(list[index].DelayTime, ++index));
                    return;
                }
            }

            isFinishInvoking = true;
            if (finishInvokeCallback != null)
            {
                finishInvokeCallback.Invoke();
                finishInvokeCallback = null;
            }
        }

        public virtual void InvokeCustom(Action<Animator> action)
        {
        }

        #region Unity functions

        private void OnValidate()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnDisable()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
                isFinishInvoking = true;
            }
        }

        #endregion
    }
}