using System;
using UnityEngine;

namespace Tools
{
    public struct Vector3Full
    {
        #region constructors

        public Vector3Full(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        public Vector3Full(Vector3 start, Vector3 direction, float lenght)
        {
            Start = start;
            End = start + direction.normalized * lenght;
        }

        public Vector3Full(Vector3 vector)
        {
            Start = Vector3.zero;
            End = vector;
        }

        #endregion

        public Vector3[] ArrayValues => new Vector3[2] { Start, End };

        public Vector3 Start { get; set; }

        public Vector3 End { get; set; }

        public Vector3 Vector => End - Start;
        public float Length => Vector3.Distance(Start, End);

        public void Translate(Vector3 v)
        {
            Start += v;
            End += v;
        }

        #region indexator

        public Vector3 this[int index]
        {
            get => index == 0 ? Start : index == 1 ? End : throw new IndexOutOfRangeException("Invalid Vector3Full index!");
            set
            {
                switch (index)
                {
                    case 0:
                        Start = value;
                        break;
                    case 1:
                        End = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3Full index!");
                }
            }
        }

        #endregion

        #region self operators

        public static Vector3Full operator +(Vector3Full a, Vector3Full b)
        {
            return new Vector3Full(a.Start, a.End + b.Vector);
        }

        public static Vector3Full operator -(Vector3Full a, Vector3Full b)
        {
            return new Vector3Full(a.Start, a.End - b.Vector);
        }

        public static Vector3Full operator -(Vector3Full a)
        {
            return new Vector3Full(a.Start, a.Start - a.Vector);
        }

        public static Vector3Full operator *(Vector3Full a, float d)
        {
            return new Vector3Full(a.Start, a.Start + a.Vector * d);
        }

        public static Vector3Full operator *(float d, Vector3Full a)
        {
            return new Vector3Full(a.Start, a.Start + a.Vector * d);
        }

        public static Vector3Full operator /(Vector3Full a, float d)
        {
            return new Vector3Full(a.Start, a.Start + a.Vector / d);
        }

        #endregion

        #region operators with vector3

        public static Vector3Full operator +(Vector3Full a, Vector3 b)
        {
            return new Vector3Full(a.Start, a.End + b);
        }

        public static Vector3Full operator +(Vector3 a, Vector3Full b)
        {
            return new Vector3Full(b.Start, b.End + a);
        }

        public static Vector3Full operator -(Vector3Full a, Vector3 b)
        {
            return new Vector3Full(a.Start, a.End - b);
        }

        public static Vector3Full operator -(Vector3 a, Vector3Full b)
        {
            return (Vector3Full)a - b;
        }

        #endregion

        #region implicit explicit

        public static implicit operator Vector3Full(Vector3 v)
        {
            return new Vector3Full(v);
        }

        public static explicit operator Vector3(Vector3Full v)
        {
            return v.Vector;
        }

        #endregion
    }
}