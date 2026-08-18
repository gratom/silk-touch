using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public struct SerializableDateTime : IComparable<SerializableDateTime>, ISerializationCallbackReceiver
    {
        [SerializeField] private long ticks;

        private bool initialized;
        private DateTime dateTime;

        public DateTime Value
        {
            get
            {
                if (!initialized)
                {
                    dateTime = new DateTime(ticks);
                    initialized = true;
                }

                return dateTime;
            }
        }

        public SerializableDateTime(DateTime dateTime)
        {
            ticks = dateTime.Ticks;
            this.dateTime = dateTime;
            initialized = true;
        }

        public void OnBeforeSerialize()
        {
            if (initialized)
            {
                ticks = dateTime.Ticks;
            }
        }

        public void OnAfterDeserialize()
        {
            initialized = false;
        }

        public int CompareTo(SerializableDateTime other) { return ticks.CompareTo(other.ticks); }

        public static implicit operator DateTime(SerializableDateTime x) { return x.Value; }

        public static implicit operator SerializableDateTime(DateTime x) { return new SerializableDateTime(x); }

        public bool IsSameDay(DateTime other)
        {
            return Value.Date == other.Date;
        }

        public bool IsToday()
        {
            return IsSameDay(DateTime.UtcNow);
        }

        public bool IsSameDay(SerializableDateTime other)
        {
            return Value.Date == other.Value.Date;
        }

        public override string ToString()
        {
            return Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}