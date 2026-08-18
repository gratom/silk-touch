using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [Assert]
    [RequireComponent(typeof(Dropdown))]
    public class DropDownEnum : MonoBehaviour
    {
#pragma warning disable
        [SerializeField] private Dropdown dropdown;
#pragma warning restore

        public event Action<int> OnValueChanged;

        public int EnumIntValue
        {
            get => enumValue;
            set
            {
                enumValue = value;
                dropdown.value = enumValue;
                OnValueChanged?.Invoke(enumValue);
            }
        }

        private int enumValue;
        private Type enumType;
        private string[] stringValues;

        #region Unity functions

        private void OnValidate()
        {
            dropdown = GetComponent<Dropdown>();
        }

        private void Awake()
        {
            dropdown.onValueChanged.AddListener(OnValueChangedDropDown);
        }

        #endregion Unity functions

        #region public functions

        public void InitWith<T>() where T : Enum
        {
            enumType = typeof(T);
            InitNewType();
        }

        public T Get<T>() where T : Enum
        {
            if (enumType == typeof(T))
            {
                return (T)(object)enumValue;
            }

            throw new Exception("incorrect type exception.\nCurrent type : " + enumType.FullName + ";\nCalled type : " + typeof(T).FullName);
        }

        public void Set<T>(T value) where T : Enum
        {
            if (enumType == typeof(T))
            {
                EnumIntValue = (int)(object)value;
            }
        }

        #endregion public functions

        #region private functions

        #region buttons function

        private void OnValueChangedDropDown(int value)
        {
            EnumIntValue = (int)(object)value;
            dropdown.value = enumValue;
        }

        #endregion buttons function

        private void InitNewType()
        {
            EnumIntValue = (int)Enum.GetValues(enumType).GetValue(0);
            stringValues = Enum.GetNames(enumType);
            List<Dropdown.OptionData> options = stringValues.Select(enumNames => new Dropdown.OptionData(enumNames)).ToList();
            dropdown.options = options;
            dropdown.value = 0;
        }

        #endregion private functions
    }
}