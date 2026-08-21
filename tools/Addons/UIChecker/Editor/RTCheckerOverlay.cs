#if UNITY_EDITOR && UI_TMP

using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor.Toolbars;

namespace Tools.UIChecker
{
    /// <summary>
    /// Оверлей для Scene View, который добавляет кнопку в панель инструментов.
    /// Использует [InitializeOnLoad] чтобы Unity подхватила оверлей автоматически.
    /// </summary>
    [InitializeOnLoad]
    [Overlay(typeof(SceneView), "RT Checker Overlay", defaultDisplay = true)]
    public class RTCheckerOverlay : ToolbarOverlay
    {
        // Конструктор по умолчанию необходим для ToolbarOverlay.
        // Он регистрирует ID элементов, которые будут внутри этого оверлея.
        protected RTCheckerOverlay() : base(RTButton.s_ElementName) { }

        // Класс, описывающий саму кнопку внутри оверлея
        [EditorToolbarElement(s_ElementName, typeof(SceneView))]
        public class RTButton : Button
        {
            public const string s_ElementName = "RTCheckSceneButton";
            private static RTButton s_Instance;

            // Стилизация: чтобы кнопка не растягивалась и выглядела аккуратно
            private static readonly StyleLength s_FixedWidth = new Length(110, LengthUnit.Pixel);

            public RTButton()
            {
                s_Instance = this;
                style.width = s_FixedWidth;

                // 1. Устанавливаем изначальный текст
                UpdateCount();

                // 2. Обработка клика: открываем твое окно
                clicked += OpenWindow;
            }

            private void OpenWindow()
            {
                // Вызываем твой статический метод, который уже есть
                // Если он private, сделай его internal или public.
                EditorWindow.GetWindow<RTCheckerWindow>(false, "RT Checker");
            }

            // Статический метод, который ты сможешь вызвать из другого класса
            public static void UpdateCount()
            {
                if (s_Instance == null)
                {
                    return;
                }

                int count = RaycastTargetChecker.potentiallyProblematic.Count;
                s_Instance.text = $"RTcheck-{count}";
            }
        }
    }

}
#endif