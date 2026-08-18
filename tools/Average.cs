//developer -> gratomov@gmail.com

using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Base class for all average types
    /// </summary>
    /// <typeparam name="T">The type of object to be averaged</typeparam>
    public abstract class AbstractAverage<T> where T : new()
    {
        /// <summary>
        /// Number of values to average
        /// </summary>
        public int Count => All.Length;

        /// <summary>
        /// Average value
        /// </summary>
        public abstract T Average { get; }

        /// <summary>
        /// Array for all values
        /// </summary>
        public readonly T[] All;
        public int Counter
        {
            get => counter;
            private set
            {
                counter = value;
                if (counter >= All.Length)
                {
                    counter = 0;
                }
            }
        }
        private int counter = 0;

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
                All[i] = new T();
            }
        }

        public List<T> GetAsRow()
        {
            List<T> row = new List<T>(All.Length);
            for (int i = 0; i < All.Length; i++)
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
            All[Counter++] = value;
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
                float sum = 0;
                int indexes = 0;
                foreach (float value in All)
                {
                    if (value != 0)
                    {
                        sum += value;
                        indexes++;
                    }
                }
                return indexes != 0 ? sum / indexes : 0;
            }
        }

        public float AverageIgnoreSpikes
        {
            get
            {
                float aver = Average;
                float sum = 0;
                int indexes = 0;
                foreach (float value in All)
                {
                    float div = aver / value;
                    if (value != 0 && div > 0.85f && div < 1.15f)
                    {
                        sum += value;
                        indexes++;
                    }
                }
                return indexes != 0 ? sum / indexes : 0;
            }
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
                int sum = 0;
                int indexes = 0;
                foreach (int value in All)
                {
                    if (value != 0)
                    {
                        sum += value;
                        indexes++;
                    }
                }
                return indexes != 0 ? sum / indexes : 0;
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
                Vector2 sum = Vector2.zero;
                int indexes = 0;
                foreach (Vector2 value in All)
                {
                    if (value != Vector2.zero)
                    {
                        sum += value;
                        indexes++;
                    }
                }
                return indexes != 0 ? sum / indexes : Vector2.zero;
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
                Vector3 sum = Vector3.zero;
                int indexes = 0;
                foreach (Vector3 value in All)
                {
                    if (value != Vector3.zero)
                    {
                        sum += value;
                        indexes++;
                    }
                }
                return indexes != 0 ? sum / indexes : Vector3.zero;
            }
        }
    }
}