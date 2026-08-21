using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tools
{
#if UNITY_EDITOR

    using UnityEditor.SceneManagement;

    [InitializeOnLoad]
    public class NamedTool
    {
        static NamedTool()
        {
            EditorApplication.delayCall += GlobalNamed;
            EditorSceneManager.sceneSaved += x => GlobalNamed();
            PrefabStage.prefabSaved += x => GlobalNamed();
        }

        private static void GlobalNamed()
        {
            StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Error);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

            MonoBehaviour[] allMonoOnActiveScene = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            foreach (MonoBehaviour monoBehaviour in allMonoOnActiveScene)
            {
                if (Attribute.GetCustomAttribute(monoBehaviour.GetType(), typeof(NamedBehavior)) is NamedBehavior)
                {
                    SerializedObject serializedObject = new SerializedObject(monoBehaviour);
                    monoBehaviour.gameObject.name = monoBehaviour.GetType().Name;
                }
            }
            Application.SetStackTraceLogType(LogType.Error, stackTraceLogType);
        }
    }

#endif

    [AttributeUsage(AttributeTargets.Class)]
    public class NamedBehavior : Attribute
    {
    }
}