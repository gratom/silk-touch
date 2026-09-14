using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    using Tools;
    using Helper = StorageSaveLoadHelper;

    public sealed class DataManager : BaseManager
    {
#pragma warning disable
        [SerializeField] private StaticData staticData;
        [SerializeField][CombinedStorage] private DynamicData dynamicData;
#pragma warning restore

        public override Type ManagerType => typeof(DataManager);

        public override bool IsFunctional => true;

        /// <summary>
        /// Data for stable setting, like default settings, URLs, non-changable enemy damage,
        /// constants, something, that you not change after build
        /// </summary>
        public StaticData StaticData => staticData;

        /// <summary>
        /// Data for changable values, like money, score, count of boosts, power ups, etc
        /// </summary>
        public DynamicData DynamicData => dynamicData;

        #region Uniny functions

#if UNITY_EDITOR
        [ContextMenu("Open saves folder")]
        private void OpenSavesFolder()
        {
            EditorUtility.RevealInFinder(SaverLoaderModule.StandardSavesPath);
        }

        [ContextMenu("DebugData")]
        private void DebugData()
        {
            Debug.Log("data today date ticks:" + DateTime.UtcNow.Date.Ticks);
            Debug.Log("data today ticks:" + DateTime.UtcNow.Ticks);
        }
#endif

        public void ForceSaveData()
        {
            SaveDynamicData();
        }

        private void OnApplicationQuit()
        {
            SaveDynamicData();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveDynamicData();
            }
        }

        #endregion Uniny functions

        #region protected functions

#pragma warning disable

        protected override async Task<bool> OnInit()
        {
            if (!IsInit)
            {
                LoadDynamicData();
                staticData.LocalizationData.InitDictionary();
            }

            return true;
        }

#pragma warning restore

        #endregion protected functions

        #region private functions

        private void OnBeforeSave()
        {
            dynamicData.GameData.mainData.BeforeSaveData();
        }

        private void OnAfterLoading()
        {
        }

        private void SaveDynamicData()
        {
            OnBeforeSave();
            FieldInfo dynamicDataField = typeof(DataManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == "dynamicData");
            if (dynamicDataField != null)
            {
                if (dynamicDataField.GetCustomAttribute<CombinedStorage>() != null)
                {
                    MakeDifferentSave(staticData.DynamicDataLocation, dynamicDataField, dynamicData);
                }
                else
                {
                    Helper.StorageSaveFunctions[Storage.StoragePathType.playerPrefs](staticData.DynamicDataLocation, JsonUtility.ToJson(dynamicData));
                    PlayerPrefs.Save();
                }
            }
        }

        private static void MakeDifferentSave<T>(string currentPath, FieldInfo field, T data)
        {
            IEnumerable<FieldInfo> fields = field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<SerializeField>() != null && x.GetCustomAttribute<Storage>() != null);
            foreach (FieldInfo item in fields)
            {
                if (item.GetCustomAttribute<CustomStorage>() != null)
                {
                    Helper.CustomStorageSaveFunctions[item.GetCustomAttribute<CustomStorage>().type]
                        (currentPath + "/" + item.Name, item, item.GetValue(data) ?? Activator.CreateInstance(item.FieldType));
                }
                else
                {
                    Helper.StorageSaveFunctions[item.GetCustomAttribute<Storage>().type](
                        currentPath + "/" + item.GetCustomAttribute<Storage>().Path,
                        JsonUtility.ToJson(item.GetValue(data) ?? Activator.CreateInstance(item.FieldType)));
                }
            }

            IEnumerable<FieldInfo> fieldsCombined = field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<SerializeField>() != null && x.GetCustomAttribute<CombinedStorage>() != null);
            foreach (FieldInfo item in fieldsCombined)
            {
                MakeDifferentSave(currentPath + "/" + item.Name, item, item.GetValue(data) ?? Activator.CreateInstance(item.FieldType));
            }
        }

        private void LoadDynamicData()
        {
            FieldInfo dynamicDataField = typeof(DataManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == "dynamicData");
            if (dynamicDataField != null)
            {
                if (dynamicDataField.GetCustomAttribute<CombinedStorage>() != null)
                {
                    dynamicData = (DynamicData)MakeDifferentLoad(staticData.DynamicDataLocation, dynamicDataField);
                }
                else
                {
                    LoadOrCreateData(ref dynamicData, staticData.DynamicDataLocation);
                }
            }

            OnAfterLoading();
        }

        private static object MakeDifferentLoad(string currentPath, FieldInfo field)
        {
            object data = Activator.CreateInstance(field.FieldType);
            IEnumerable<FieldInfo> fields = field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<SerializeField>() != null && x.GetCustomAttribute<Storage>() != null);
            foreach (FieldInfo item in fields)
            {
                if (item.GetCustomAttribute<CustomStorage>() != null)
                {
                    item.SetValue(data, Helper.CustomStorageLoadFunctions[item.GetCustomAttribute<CustomStorage>().type]
                        (currentPath + "/" + item.Name, item));
                }
                else
                {
                    string jsonValue = Helper.StorageLoadFunctions[item.GetCustomAttribute<Storage>().type](
                        currentPath + "/" + item.GetCustomAttribute<Storage>().Path);
                    if (!string.IsNullOrEmpty(jsonValue))
                    {
                        object insertingValue = JsonUtility.FromJson(jsonValue, item.FieldType);
                        item.SetValue(data, insertingValue);
                    }
                }
            }

            IEnumerable<FieldInfo> fieldsCombined = field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<SerializeField>() != null && x.GetCustomAttribute<CombinedStorage>() != null);
            foreach (FieldInfo item in fieldsCombined)
            {
                item.SetValue(data, MakeDifferentLoad(currentPath + "/" + item.Name, item));
            }

            return data;
        }

        private static void LoadOrCreateData<T>(ref T value, string location) where T : new()
        {
            string data = PlayerPrefs.GetString(location, "");
            value = data != "" ? JsonUtility.FromJson<T>(data) : new T();
        }

        #endregion private functions
    }
}