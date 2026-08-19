#if UNITY_EDITOR
using UnityEditor;
using Object = UnityEngine.Object;

namespace Tools.Anim.Editor
{
    using EDGL = EditorGUILayout;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimatorInvoker))]
    public class AnimatorInvokerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EDGL.PropertyField(serializedObject.FindProperty("animator"));
            EDGL.PropertyField(serializedObject.FindProperty("list"));

            Object animator = serializedObject.FindProperty("animator").objectReferenceValue;
            SerializedProperty serializedList = serializedObject.FindProperty("list");

            for (int i = 0; i < serializedList.arraySize; i++)
            {
                SerializedProperty element = serializedList.GetArrayElementAtIndex(i);
                SerializedProperty value = element.FindPropertyRelative("value");
                SerializedProperty parameter = value.FindPropertyRelative("parameter");
                SerializedProperty animatorParameter = parameter.FindPropertyRelative("animator");
                animatorParameter.objectReferenceValue = animator;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif