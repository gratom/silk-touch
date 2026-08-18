using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public static class AsyncSafeBridgeExtensions
    {
        public static UniTask<T> SafeAwait<T>(this UniTask<T> originalTask)
        {
            return AsyncSafeBridge.SafeAwait(originalTask);
        }
    }
    
    public class AsyncSafeBridge : MonoBehaviour
    {
        private ITaskContainer container;
        private int framesCount;
        private const int SAFE_FRAMES_DELAY = 5;

        public static UniTask<T> SafeAwait<T>(UniTask<T> originalTask)
        {
            GameObject go = new GameObject($"[AsyncSafeBridge_{typeof(T).Name}]");
            DontDestroyOnLoad(go);

            AsyncSafeBridge bridge = go.AddComponent<AsyncSafeBridge>();
            TaskContainer<T> container = new TaskContainer<T>(originalTask);
            bridge.container = container;
            return container.Task;
        }

        private void Update()
        {
            if (container == null)
            {
                Destroy(gameObject);
                return;
            }

            if (!container.IsDone)
            {
                return;
            }

            framesCount++;

            if (framesCount >= SAFE_FRAMES_DELAY)
            {
                container.Complete();
                Destroy(gameObject);
            }
        }
    }

    public interface ITaskContainer
    {
        bool IsDone { get; }
        void Complete();
    }

    public class TaskContainer<T> : ITaskContainer
    {
        private readonly UniTaskCompletionSource<T> fakeDoubleForOriginTask = new UniTaskCompletionSource<T>();
        private readonly UniTask<T> originalCachedTask;
        private T result;

        public bool IsDone { get; private set; }
        public UniTask<T> Task => fakeDoubleForOriginTask.Task;

        public TaskContainer(UniTask<T> originalTask)
        {
            originalCachedTask = originalTask;
            originalTask.ContinueWith(taskResult =>
            {
                result = taskResult;
                IsDone = true;
            });
        }

        public void Complete()
        {
            fakeDoubleForOriginTask.TrySetResult(result);
        }
    }
}