#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using ACPT = UnityEngine.AnimatorControllerParameterType;

namespace Tools.Anim.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorParameter))]
    public class AnimatorParameterPropertyDrawer : PropertyDrawer
    {
        private static readonly string[] EnumTypeNames = Enum.GetNames(typeof(ACPT));

        private bool isNeedToShowAnimator = true;

        private static readonly Dictionary<int, int> ToEnum = new Dictionary<int, int>()
        {
            { 0, 1 },
            { 1, 3 },
            { 2, 4 },
            { 3, 9 }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int offset = label.text.Length * 7 + 60;

            Rect rectOrigin = position;
            position.xMin += offset - 40;

            Rect labelRect = rectOrigin;
            labelRect.xMax = offset;
            labelRect.height = 18;
            GUI.Label(labelRect, label.text);
            labelRect.xMin += offset;
            labelRect.xMax += rectOrigin.xMax;

            #region try get animator

            Rect animatorFieldPosition = rectOrigin;
            animatorFieldPosition.height = 18;
            animatorFieldPosition.yMax += 19;
            animatorFieldPosition.yMin += 19;


            Animator animator = property.FindPropertyRelative("animator").objectReferenceValue as Animator;
            if (animator == null)
            {
                animator = ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<Animator>();
                if (animator != null)
                {
                    property.FindPropertyRelative("animator").objectReferenceValue = animator;
                }
            }

            if (animator == null)
            {
                isNeedToShowAnimator = true;
                EditorGUI.PropertyField(animatorFieldPosition, property.FindPropertyRelative("animator"));
            }
            else
            {
                isNeedToShowAnimator = false;
                Rect rectButton = rectOrigin;
                rectButton.xMin -= 15;
                rectButton.yMin += 2;
                rectButton.xMax = rectButton.xMin + 15;
                rectButton.yMax = rectButton.yMin + 15;
                if (GUI.Button(rectButton, ">"))
                {
                    property.FindPropertyRelative("animator").objectReferenceValue = null;
                }
            }

            #endregion

            if (animator == null)
            {
                EditorGUI.LabelField(labelRect, "No Animator component found!");
                EditorGUI.EndProperty();
                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                EditorGUI.LabelField(labelRect, "The animator has no controller link");
                EditorGUI.EndProperty();
                return;
            }

            AnimatorController editorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(animator.runtimeAnimatorController));
            SerializedProperty currentType = property.FindPropertyRelative("parameterType");
            Rect propertyTypePopup = position;
            propertyTypePopup.width /= 2;
            propertyTypePopup.height = 20;
            int returnedPopupValue = EditorGUI.Popup(propertyTypePopup, currentType.enumValueIndex, EnumTypeNames);
            currentType.enumValueIndex = returnedPopupValue != -1 ? returnedPopupValue : 0;

            SerializedProperty currentHash = property.FindPropertyRelative("hash");

            AnimatorControllerParameter[] parameters = editorController.parameters;
            List<AnimatorControllerParameter> animatorParameters = parameters.Where(x => x.type == (ACPT)ToEnum[currentType.enumValueIndex]).ToList();

            List<string> animatorParameterNames = new List<string>();
            int currentlySelectedParameterIndex = 0;

            for (int i = 0; i < animatorParameters.Count; i++)
            {
                animatorParameterNames.Add(animatorParameters[i].name);
                if (currentHash.intValue == animatorParameters[i].nameHash)
                {
                    currentlySelectedParameterIndex = i;
                }
            }

            if (animatorParameterNames.Count == 0)
            {
                animatorParameterNames = new List<string>() { "not found" };
            }

            Rect namePopup = position;
            namePopup.width /= 2;
            namePopup.x += namePopup.width;
            namePopup.height = 20;
            int selectedIndex = EditorGUI.Popup(namePopup, currentlySelectedParameterIndex, animatorParameterNames.ToArray());
            if (selectedIndex < animatorParameters.Count)
            {
                currentHash.intValue = animatorParameters[selectedIndex].nameHash;
                property.FindPropertyRelative("name").stringValue = animatorParameterNames[selectedIndex];
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return isNeedToShowAnimator ? 42 : 22;
        }
    }
}
#endif