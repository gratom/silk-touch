using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    using Tools;
    public static class StorageSaveLoadHelper
    {
        #region public properties

        public static Dictionary<Storage.StoragePathType, Action<string, string>> StorageSaveFunctions { get; } = new Dictionary<Storage.StoragePathType, Action<string, string>>()
        {
            { Storage.StoragePathType.applicationFolder, SaveInAppFolder },
            { Storage.StoragePathType.playerPrefs, SaveInPlayerPrefs },
            { Storage.StoragePathType.server, SaveOnServer }
        };

        public static Dictionary<Storage.StoragePathType, Func<string, string>> StorageLoadFunctions { get; } = new Dictionary<Storage.StoragePathType, Func<string, string>>()
        {
            { Storage.StoragePathType.applicationFolder, LoadFromAppFolder },
            { Storage.StoragePathType.playerPrefs, LoadFromPlayerPrefs },
            { Storage.StoragePathType.server, LoadFromServer }
        };

        public static Dictionary<CustomStorage.StorageType, Action<string, FieldInfo, object>> CustomStorageSaveFunctions { get; } = new Dictionary<CustomStorage.StorageType, Action<string, FieldInfo, object>>()
        {
            { CustomStorage.StorageType.splitListValue, CustomStorageSaveSplitListValue }
        };

        public static Dictionary<CustomStorage.StorageType, Func<string, FieldInfo, object>> CustomStorageLoadFunctions { get; } = new Dictionary<CustomStorage.StorageType, Func<string, FieldInfo, object>>()
        {
            { CustomStorage.StorageType.splitListValue, CustomStorageLoadSplitListValue }
        };

        #endregion public properties

        #region private funtions

        private static void SaveInAppFolder(string path, string data)
        {
            SaverLoaderModule.SaveMyDataToFile(path, data);
        }

        private static void SaveInPlayerPrefs(string path, string data)
        {
            PlayerPrefs.SetString(path, data);
        }

        private static void SaveOnServer(string path, string data)
        {
            throw new NotImplementedException("Server saving non-realized in this project.");
        }

        private static void CustomStorageSaveSplitListValue(string path, FieldInfo field, object data)
        {
            if (field.FieldType.GetInterfaces().FirstOrDefault(x => x == typeof(IEnumerable)) != null)
            {
                int i = 0;
                foreach (object item in (IEnumerable)data)
                {
                    StorageSaveFunctions[field.GetCustomAttribute<Storage>().type](
                        path + "/" + i.ToString() + "/" + field.GetCustomAttribute<Storage>().Path,
                        JsonUtility.ToJson(item ?? Activator.CreateInstance(field.FieldType.GetGenericArguments()[0])));
                    i++;
                }
            }
        }

        private static string LoadFromAppFolder(string path)
        {
            return SaverLoaderModule.LoadMyDataFrom(SaverLoaderModule.StandardSavesPath + "/" + path);
        }

        private static string LoadFromPlayerPrefs(string path)
        {
            return PlayerPrefs.GetString(path, "");
        }

        private static string LoadFromServer(string path)
        {
            throw new NotImplementedException("Server loading non-realized in this project.");
        }

        private static object CustomStorageLoadSplitListValue(string path, FieldInfo field)
        {
            IList list = (IList)Activator.CreateInstance(field.FieldType);
            if (field.FieldType.GetInterfaces().FirstOrDefault(x => x == typeof(IEnumerable)) != null)
            {
                int i = 0;
                string[] directories = Directory.GetDirectories(SaverLoaderModule.StandardSavesPath + "/" + path + "/");
                foreach (string item in directories)
                {
                    if (int.TryParse(item.Split('/').Last(), out i))
                    {
                        string jsonString = StorageLoadFunctions[field.GetCustomAttribute<Storage>().type]
                            (item.Substring(SaverLoaderModule.StandardSavesPath.Length) + "/" + field.GetCustomAttribute<Storage>().Path);
                        list.Add(JsonUtility.FromJson(jsonString, field.FieldType.GetGenericArguments()[0]));
                    }
                }
            }
            return list;
        }

        #endregion private funtions
    }
}