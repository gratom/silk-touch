#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SilkTouch.EditorScripts
{
    using Boot;
    using Tools;

    [CustomEditor(typeof(BootSettings))]
    public class BootSettingEditor : Editor
    {
        private ReorderableList reorderableList;
        private string[] scenesArray;

        private const string bootTimeName = "bootTime";
        private const string managersName = "managerContainers";
        private const string sceneIndexName = "nextSceneIndex";

        private void OnEnable()
        {
            InitScenes();
            InitList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Scene Selection
            SerializedProperty sceneProp = serializedObject.FindProperty(sceneIndexName);
            int selected = Mathf.Clamp(sceneProp.intValue, 0, scenesArray.Length - 1);
            sceneProp.intValue = EditorGUILayout.Popup("Next scene after boot", selected, scenesArray);

            // Boot Time
            EditorGUILayout.PropertyField(serializedObject.FindProperty(bootTimeName));

            EditorGUILayout.Space(10);
            
            // Render the custom ReorderableList
            reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void InitList()
        {
            SerializedProperty prop = serializedObject.FindProperty(managersName);
            
            reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);

            // Header of the list
            reorderableList.drawHeaderCallback = (Rect rect) => {
                float half = rect.width / 2;
                EditorGUI.LabelField(new Rect(rect.x + 14, rect.y, half, rect.height), "Functional Manager");
                EditorGUI.LabelField(new Rect(rect.x + 14 + half, rect.y, half, rect.height), "Fake Manager (Optional)");
            };

            // Drawing a single line (container)
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = prop.GetArrayElementAtIndex(index);
                rect.y += 2;
                
                float spacing = 5;
                float width = (rect.width / 2) - spacing;

                // Functional Manager field
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("functionalManager"), 
                    GUIContent.none
                );

                // Fake Manager field
                EditorGUI.PropertyField(
                    new Rect(rect.x + width + spacing * 2, rect.y, width, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("fakeManager"), 
                    GUIContent.none
                );
            };

            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
        }

        private void InitScenes()
        {
            scenesArray = SceneTool.GetScenesNamesInBuild();
            if (scenesArray == null || scenesArray.Length == 0)
            {
                scenesArray = new[] { "No scenes in build" };
            }
            else
            {
                scenesArray[0] = "null";
            }
        }
    }
}
#endif