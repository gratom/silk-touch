using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public abstract class AbstractRange<T> where T : IComparable, IFormattable, IConvertible
    {
        public event Action<T> OnReachMax;
        public event Action<T> OnReachMin;

        public T Max
        {
            get => max;
            set
            {
                max = value.CompareTo(min) <= 0 ? Inc(min) : value;
                Current = current;
            }
        }

        [SerializeField] protected T max;

        public T Current
        {
            get => current;
            set
            {
                current = value.CompareTo(max) > 0 ? max : value.CompareTo(min) < 0 ? min : value;
                if (IsMax)
                {
                    OnReachMax?.Invoke(max);
                }
                else
                {
                    if (IsMin)
                    {
                        OnReachMin?.Invoke(min);
                    }
                }
            }
        }

        [SerializeField] protected T current;

        public T Min
        {
            get => min;
            set
            {
                min = value.CompareTo(max) >= 0 ? Dec(max) : value;
                Current = current;
            }
        }

        [SerializeField] protected T min;

        public bool IsMax => Equals(current, max);
        public bool IsMin => Equals(current, min);

        public void SetMax()
        {
            Current = max;
        }

        public void SetMin()
        {
            Current = min;
        }

        public abstract float Percent { get; set; }
        public abstract T Deficit { get; }

        protected abstract T Inc(T value);

        protected abstract T Dec(T value);

        public static T1 From<T1>(T1 origin) where T1 : AbstractRange<T>, new()
        {
            return new T1
            {
                max = origin.max,
                current = origin.current,
                min = origin.min
            };
        }
    }

    [Serializable]
    public class RangeFloat : AbstractRange<float>
    {
        public override float Percent
        {
            get => (current - min) / (max - min);
            set => Current = min + value * (max - min);
        }

        public override float Deficit => max - current;

        protected override float Inc(float value)
        {
            return value + 1;
        }

        protected override float Dec(float value)
        {
            return value - 1;
        }
    }

    [Serializable]
    public class RangeInt : AbstractRange<int>
    {
        public override float Percent
        {
            get => (float)(current - min) / (max - min);
            set => Current = (int)(min + value * (max - min));
        }

        protected override int Dec(int value)
        {
            return value + 1;
        }

        public override int Deficit => max - current;

        protected override int Inc(int value)
        {
            return value - 1;
        }
    }

}