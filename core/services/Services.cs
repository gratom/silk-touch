using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SilkTouch.Managers
{
    public static class Services
    {
        private static bool isInit = false;
        private static Dictionary<Type, IBaseManager> managersDictionary;

        public static async Task InitAppWith(List<ManagerContainer> containers)
        {
            if (isInit)
            {
#if UNITY_EDITOR
                Debug.LogError("Services already initiated.");
#endif
                return;
            }

            GameObject root = new GameObject("Managers");
            Object.DontDestroyOnLoad(root);

            managersDictionary = new Dictionary<Type, IBaseManager>();
            List<Task> initTasks = new List<Task>();

            foreach (ManagerContainer container in containers)
            {
                BaseManager instance = Object.Instantiate(container.functionalManager, root.transform);
                managersDictionary.Add(instance.ManagerType, instance);
                initTasks.Add(InitializeWithFallback(instance, container, root.transform));
            }
            try
            {
                await Task.WhenAll(initTasks);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Services] Critical error during parallel initialization: {e.Message}");
                throw;
            }

            isInit = true;
            Debug.Log("[Services] All managers initiated successfully.");
        }

        private static async Task InitializeWithFallback(BaseManager manager, ManagerContainer container, Transform parent)
        {
            try
            {
                await manager.Init();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Services] {manager.GetType().Name} initialization failed. Error: {e.Message}");

                if (container.fakeManager != null)
                {
                    Object.Destroy(manager.gameObject);
                    BaseManager fakeInstance = Object.Instantiate(container.fakeManager, parent);
                    managersDictionary[manager.ManagerType] = fakeInstance;
                    await fakeInstance.Init();
                    Debug.Log($"[Services] {manager.ManagerType.Name} swapped to Fake implementation.");
                }
                else
                {
                    Debug.LogError($"[Services] Critical Manager {manager.GetType().Name} failed and has no Fake fallback!");
                    throw;
                }
            }
        }

        public static T GetManager<T>() where T : IBaseManager
        {
            if (managersDictionary.TryGetValue(typeof(T), out IBaseManager manager))
            {
                return (T)manager;
            }

#if UNITY_EDITOR
            Debug.LogError($"Manager of type {typeof(T).Name} not found in Services!");
#endif
            return default;
        }
    }

    [Serializable]
    public class ManagerContainer
    {
        public BaseManager functionalManager;
        public BaseManager fakeManager;
    }
}