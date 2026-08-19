using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public static class TaskPauseExtensions
    {
        public static async Task PauseAwareDelay(int delayTime)
        {
            int elapsedTime = 0;

            do
            {
                int timeLeft = delayTime - elapsedTime;
                await Pause.UnPaused;

                float startAwaitingTime = Time.realtimeSinceStartup;
                Task delay = Task.Delay(timeLeft);
                Task pause = Pause.Paused;
                await Task.WhenAny(delay, pause);

                int passedTime = (int)((Time.realtimeSinceStartup - startAwaitingTime) * 1000);
                elapsedTime += passedTime;

            } while (elapsedTime < delayTime);
        }

        public static async Task PauseAwareDelay(int delayTime, CancellationToken cancellationToken)
        {
            int elapsedTime = 0;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                int timeLeft = delayTime - elapsedTime;

                await Pause.UnPaused;
                cancellationToken.ThrowIfCancellationRequested();

                float startAwaitingTime = Time.realtimeSinceStartup;

                Task delay = Task.Delay(timeLeft, cancellationToken);
                Task pause = Pause.Paused;

                Task cancellationTask = Task.Delay(-1, cancellationToken);

                await Task.WhenAny(delay, pause, cancellationTask);

                cancellationToken.ThrowIfCancellationRequested();

                int passedTime = (int)((Time.realtimeSinceStartup - startAwaitingTime) * 1000);
                elapsedTime += passedTime;

            } while (elapsedTime < delayTime);
        }
    }
}