#if UNITASK

using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public class AsyncSelfDestroyer : MonoBehaviour
    {
        [SerializeField] private float timeToDestroy = 1;

        private async void Awake()
        {
            await Task.Delay((int)(timeToDestroy * 1000));
            await UniTask.SwitchToMainThread();
            if (this != null && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}

#endif