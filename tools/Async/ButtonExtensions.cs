#if UI_TMP
using System;
using System.Threading;
#if UNITASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using UnityEngine.UI;

namespace SilkTouch.Tools
{
    public static class ButtonExtensions
    {
#if UNITASK
        public static async UniTask WaitOnClickAsync(this Button button, CancellationToken cancellationToken = default)
        {
            UniTaskCompletionSource utcs = new UniTaskCompletionSource();

            void OnClick()
            {
                utcs.TrySetResult();
            }

            button.onClick.AddListener(OnClick);

            using (cancellationToken.Register(() => utcs.TrySetCanceled(cancellationToken)))
            {
                try
                {
                    await utcs.Task;
                }
                finally
                {
                    button.onClick.RemoveListener(OnClick);
                }
            }
        }
#else
        public static async Task WaitOnClickAsync(this Button button, CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            void OnClick()
            {
                tcs.TrySetResult(true);
            }

            button.onClick.AddListener(OnClick);

            using (cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken)))
            {
                try
                {
                    await tcs.Task;
                }
                finally
                {
                    button.onClick.RemoveListener(OnClick);
                }
            }
        }
#endif
    }
}
#endif