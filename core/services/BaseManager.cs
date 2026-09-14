using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SilkTouch.Managers
{
    using Tools;
    
    public interface IBaseManager
    {
        public Type ManagerType { get; }

        public bool IsInit { get; }

        public bool IsFunctional { get; }

        public Task Init();
    }

    public abstract class BaseManager : MonoBehaviour, IBaseManager
    {
        public abstract Type ManagerType { get; }
        public bool IsInit { get; private set; } = false;
        public abstract bool IsFunctional { get; }

        public async Task Init()
        {
            if (!IsInit)
            {
                DateTime startTime = DateTime.Now;
                Debug.Log($"Manager {ManagerType} : start init in {startTime}...".AsSystemEvent());

                IsInit = await OnInit();
                if (!IsInit)
                {
                    Debug.Log($"manager type of {ManagerType} is not initialized correctly!".AsError());
                    throw new Exception($"manager type of {ManagerType} is not initialized correctly!");
                }

                TimeSpan duration = DateTime.Now - startTime;
                Debug.Log($"Manager {ManagerType} : init {"successfully".WithColor(Color.green)} in {duration.TotalMilliseconds} ms.".AsSystemEvent());
            }
        }

        protected abstract Task<bool> OnInit();

        protected virtual void OnValidate()
        {
            gameObject.name = GetType().Name;
        }

    }
}