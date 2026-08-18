using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public struct SerializableGuid :
        IComparable,
        IComparable<Guid>,
        IEquatable<Guid>,
        IFormattable,
        ISerializationCallbackReceiver
    {
#pragma warning disable

        [SerializeField] private string stringGuid;

#pragma warning restore

        public readonly Guid Guid;

        public static SerializableGuid Empty = new SerializableGuid(Guid.Empty);

        public SerializableGuid(Guid guid)
        {
            Guid = guid;
            stringGuid = guid.ToString();
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return -1;
                case SerializableGuid serializableGuid:
                    return serializableGuid.Guid.CompareTo(Guid);
                case Guid guid:
                    return guid.CompareTo(Guid);
                default:
                    return -1;
            }
        }

        public int CompareTo(Guid other)
        {
            return Guid.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case SerializableGuid serializableGuid:
                    return serializableGuid == Guid;
                case Guid guid:
                    return guid == Guid;
                default:
                    return false;
            }
        }

        public bool Equals(Guid other)
        {
            return Guid == other;
        }

        public override string ToString()
        {
            return Guid.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Guid.ToString(format, formatProvider);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public static bool operator ==(SerializableGuid a, SerializableGuid b)
        {
            return a.Guid == b.Guid;
        }
        public static bool operator !=(SerializableGuid a, SerializableGuid b)
        {
            return a.Guid != b.Guid;
        }
        public static explicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid);
        }
        public static implicit operator Guid(SerializableGuid sGuid)
        {
            return sGuid.Guid;
        }
        public static implicit operator SerializableGuid(string str)
        {
            return Guid.TryParse(str, out Guid guid) ? new SerializableGuid(guid) : new SerializableGuid();
        }
        public static implicit operator string(SerializableGuid sGuid)
        {
            return sGuid.ToString();
        }

        public void OnBeforeSerialize()
        {
            stringGuid = this;
        }

        public void OnAfterDeserialize()
        {
            this = stringGuid;
        }
    }
}