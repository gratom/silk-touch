#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools.Anim.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorAction))]
    public class AnimatorActionPropertyDrawer : PropertyDrawer
    {
        private static string[] names = { "Trigger set", "Trigger reset" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rectOrigin = position;

            Rect actionTypeLabel = rectOrigin;
            actionTypeLabel.xMax = 120;
            actionTypeLabel.height = 20;
            actionTypeLabel.yMin -= 3;
            EditorGUI.LabelField(actionTypeLabel, "Action type");

            Rect actionTypeRect = rectOrigin;
            actionTypeRect.xMin += 70;
            actionTypeRect.height = 20;
            string[] options = Enum.GetNames(typeof(AnimatorActionType));
            SerializedProperty actionType = property.FindPropertyRelative("actionType");
            actionType.enumValueIndex = EditorGUI.Popup(actionTypeRect, actionType.enumValueIndex, options);

            #region by type

            if ((AnimatorActionType)actionType.enumValueIndex == AnimatorActionType.Animation)
            {
                Rect valueRect = rectOrigin;
                valueRect.yMin += 22;
                SerializedProperty value = property.FindPropertyRelative("value");
                EditorGUI.PropertyField(valueRect, value);

                //parameter type func
                Rect typeFuncRect = rectOrigin;
                typeFuncRect.height = 20;

                SerializedProperty typeFunc = property.FindPropertyRelative("parameterTypeFunc");
                typeFuncRect.yMin += value.FindPropertyRelative("parameter").FindPropertyRelative("animator").objectReferenceValue == null ? 60 : 42;
                typeFuncRect.yMax += value.FindPropertyRelative("parameter").FindPropertyRelative("animator").objectReferenceValue == null ? 60 : 42;

                int parameterTypeValue = value.FindPropertyRelative("parameter").FindPropertyRelative("parameterType").enumValueIndex;
                switch (parameterTypeValue)
                {
                    case 3:
                        int fakeIndex = typeFunc.enumValueIndex > 1 || typeFunc.enumValueIndex < 0 ? 0 : typeFunc.enumValueIndex;
                        typeFunc.enumValueIndex = EditorGUI.Popup(typeFuncRect, fakeIndex, names);
                        break;
                    case 0:
                        typeFunc.enumValueIndex = (int)AnimatorControllerParameterTypeFunc.Float;
                        break;
                    case 1:
                        typeFunc.enumValueIndex = (int)AnimatorControllerParameterTypeFunc.Int;
                        break;
                    case 2:
                        typeFunc.enumValueIndex = (int)AnimatorControllerParameterTypeFunc.Bool;
                        break;
                }

                if (value.FindPropertyRelative("parameter").FindPropertyRelative("animator").objectReferenceValue != null)
                {
                    Rect animatorLabel = rectOrigin;
                    animatorLabel.height = 20;
                    animatorLabel.yMin += 60;
                    animatorLabel.yMax += 60;
                    EditorGUI.LabelField(animatorLabel, "-------------Animator:" + value.FindPropertyRelative("parameter").FindPropertyRelative("animator").objectReferenceValue.name + "------------");
                }
            }
            else
            {
                Rect delayRect = rectOrigin;
                delayRect.yMin += 22;
                delayRect.height = 20;
                SerializedProperty delay = property.FindPropertyRelative("delayTime");
                delay.floatValue = EditorGUI.FloatField(delayRect, "delay time", delay.floatValue);
            }

            #endregion

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 85;
        }
    }
}
#endif