using System;

namespace SilkTouch.Managers.Data
{
    public interface IPathServerSaving
    {
        void SaveData(string data, string path);

        string LoadData(string path);
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Storage : Attribute
    {
        public enum StoragePathType
        {
            playerPrefs,
            applicationFolder,
            server
        }

        public StoragePathType type { get; }

        public string Path { get; }

        public Storage(StoragePathType type, string path)
        {
            this.type = type;
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CombinedStorage : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CustomStorage : Attribute
    {
        public enum StorageType
        {
            splitListValue
        }

        public StorageType type { get; }

        public CustomStorage(StorageType type)
        {
            this.type = type;
        }
    }
}