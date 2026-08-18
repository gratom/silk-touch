using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class ScriptsExtensions
    {
        public static void GetAllMonoScriptsRecursively(this GameObject rootGameObject, bool includeInactive, ref List<MonoBehaviour> scripts)
        {
            if (rootGameObject == null)
            {
                Debug.LogError("Prefab is null!");
                return;
            }

            if (scripts == null)
            {
                scripts = new List<MonoBehaviour>();
            }

            Transform[] childTransforms = rootGameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform childTransform in childTransforms)
            {
                MonoBehaviour[] prefabScripts = childTransform.GetComponents<MonoBehaviour>();
                if (prefabScripts != null && prefabScripts.Length > 0)
                {
                    foreach (MonoBehaviour script in prefabScripts)
                    {
                        if (script != null && (includeInactive || script.enabled))
                        {
                            if (!script.GetType().Assembly.GetName().Name.Contains("UnityEngine"))
                            {
                                scripts.Add(script);
                            }
                        }
                    }
                }

            }
        }
    }
}