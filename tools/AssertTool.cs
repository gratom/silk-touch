//developer -> gratomov@gmail.com

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Tools
{
#if UNITY_EDITOR

    using UnityEditor.SceneManagement;

    [InitializeOnLoad]
    public class AssertTool
    {
        static AssertTool()
        {
            EditorApplication.delayCall += GlobalAssert;
            EditorSceneManager.sceneSaved += x => GlobalAssert();
        }

        private static void GlobalAssert()
        {
            StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Error);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

            MonoBehaviour[] allMonoOnActiveScene = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            foreach (MonoBehaviour monoBehaviour in allMonoOnActiveScene)
            {
                if (Attribute.GetCustomAttribute(monoBehaviour.GetType(), typeof(Assert)) is Assert)
                {
                    SerializedObject serializedObject = new SerializedObject(monoBehaviour);
                    foreach (FieldInfo field in monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (field.FieldType.IsClass)
                        {
                            if (Attribute.GetCustomAttribute(field, typeof(SerializeField)) is SerializeField)
                            {
                                if (serializedObject.FindProperty(field.Name)?.propertyType == SerializedPropertyType.ObjectReference &&
                                    serializedObject.FindProperty(field.Name)?.objectReferenceValue == null)
                                {
                                    Debug.LogErrorFormat(monoBehaviour.gameObject, $"{monoBehaviour.gameObject.name}.{field.Name}, typeof {field.FieldType.Name} is null");
                                }
                            }
                        }
                    }
                }
            }
            Application.SetStackTraceLogType(LogType.Error, stackTraceLogType);
        }
    }

#endif

    /// <summary>
    /// Attribute-tool to automatically check for empty, uninitialized serialized reference fields in your Mono-objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Assert : Attribute
    {
    }



}