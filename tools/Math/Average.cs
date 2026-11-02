using System.Collections.Generic;
using UnityEngine;

namespace SilkTouch.Tools
{
    /// <summary>
    /// Base class for all average types
    /// </summary>
    /// <typeparam name="T">The type of object to be averaged</typeparam>
    public abstract class AbstractAverage<T>
    {
        /// <summary>
        /// Capacity of the inner buffer
        /// </summary>
        public int Capacity => All.Length;

        /// <summary>
        /// Number of slots actually filled with real values (bounded by Capacity)
        /// </summary>
        public int FilledCount { get; private set; }

        /// <summary>
        /// Average value
        /// </summary>
        public abstract T Average { get; }

        /// <summary>
        /// Array for all values
        /// </summary>
        public readonly T[] All;

        private int counter;

        protected AbstractAverage(int count = 5)
        {
            All = new T[count];
        }

        /// <summary>
        /// Empty all values
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < All.Length; i++)
            {
                All[i] = default;
            }
            counter = 0;
            FilledCount = 0;
        }

        public List<T> GetAsRow()
        {
            List<T> row = new List<T>(FilledCount);
            for (int i = 0; i < FilledCount; i++)
            {
                int index = (i + counter - 1 + All.Length) % All.Length;
                row.Add(All[index]);
            }

            return row;
        }

        /// <summary>
        /// Add next value to inner array
        /// </summary>
        public void AddNext(T value)
        {
            All[counter] = value;
            counter = counter + 1 == All.Length ? 0 : counter + 1;
            if (FilledCount < All.Length)
            {
                FilledCount++;
            }
        }
    }

    /// <summary>
    /// Average realization for float-type
    /// </summary>
    public class AverageFloat : AbstractAverage<float>
    {
        public AverageFloat(int count = 5) : base(count)
        {
        }

        public override float Average
        {
            get
            {
                if (FilledCount == 0)
                {
                    return 0f;
                }

                float sum = 0f;
                for (int i = 0; i < FilledCount; i++)
                {
                    sum += All[i];
                }
                return sum / FilledCount;
            }
        }

        public float AverageIgnoreSpikes(float ignoreVal = 0.15f)
        {
            if (FilledCount == 0)
            {
                return 0f;
            }

            float aver = Average;
            float sum = 0f;
            int indexes = 0;

            float min = 1 - ignoreVal;
            float max = 1 + ignoreVal;
            for (int i = 0; i < FilledCount; i++)
            {
                float value = All[i];
                if (value == 0f)
                {
                    continue;
                }

                float div = aver / value;
                if (div > min && div < max)
                {
                    sum += value;
                    indexes++;
                }
            }
            return indexes != 0 ? sum / indexes : 0f;
        }
    }

    /// <summary>
    /// Average realization for int-type
    /// </summary>
    public class AverageInt : AbstractAverage<int>
    {
        public AverageInt(int count = 5) : base(count)
        {
        }

        public override int Average
        {
            get
            {
                if (FilledCount == 0)
                {
                    return 0;
                }

                int sum = 0;
                for (int i = 0; i < FilledCount; i++)
                {
                    sum += All[i];
                }
                return sum / FilledCount;
            }
        }
    }

    /// <summary>
    /// Average realization for Vector2-type
    /// </summary>
    public class AverageVector2 : AbstractAverage<Vector2>
    {
        public AverageVector2(int count = 5) : base(count)
        {
        }

        public override Vector2 Average
        {
            get
            {
                if (FilledCount == 0)
                {
                    return Vector2.zero;
                }

                Vector2 sum = Vector2.zero;
                for (int i = 0; i < FilledCount; i++)
                {
                    sum += All[i];
                }
                return sum / FilledCount;
            }
        }
    }

    /// <summary>
    /// Average realization for Vector3-type
    /// </summary>
    public class AverageVector3 : AbstractAverage<Vector3>
    {
        public AverageVector3(int count = 5) : base(count)
        {
        }

        public override Vector3 Average
        {
            get
            {
                if (FilledCount == 0)
                {
                    return Vector3.zero;
                }

                Vector3 sum = Vector3.zero;
                for (int i = 0; i < FilledCount; i++)
                {
                    sum += All[i];
                }
                return sum / FilledCount;
            }
        }
    }
}
