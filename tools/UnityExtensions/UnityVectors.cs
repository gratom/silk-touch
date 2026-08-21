using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools
{

    using UnityEngine;

    public static class UnityVectorsTools
    {

        public static Vector3 WithX(this Vector3 vector, float newX)
        {
            return new Vector3(newX, vector.y, vector.z);
        }

        public static Vector3 AddX(this Vector3 vector, float newX)
        {
            return new Vector3(vector.x + newX, vector.y, vector.z);
        }

        public static Vector3 WithY(this Vector3 vector, float newY)
        {
            return new Vector3(vector.x, newY, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float newY)
        {
            return new Vector3(vector.x, vector.y + newY, vector.z);
        }

        public static Vector3 WithZ(this Vector3 vector, float newZ)
        {
            return new Vector3(vector.x, vector.y, newZ);
        }

        public static Vector3 AddZ(this Vector3 vector, float newZ)
        {
            return new Vector3(vector.x, vector.y, vector.z + newZ);
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static Vector2 Rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        public static Vector2 RandomVector2Normal()
        {
            return DegreeToVector2(Random.Range(0f, 360f));
        }

        public static Vector2 RandomVector2(Vector2 point1, Vector2 point2)
        {
            return new Vector2(Random.Range(point1.x, point2.x), Random.Range(point1.y, point2.y));
        }

        public static Vector3 RandomVector3(Vector3 point1, Vector3 point2)
        {
            return new Vector3(Random.Range(point1.x, point2.x), Random.Range(point1.y, point2.y), Random.Range(point1.z, point2.z));
        }

        public static float ReverseLerp(Vector3 start, Vector3 end, Vector3 pos)
        {
            Vector3 direction = end - start;
            float projection = Vector3.Dot(pos - start, direction) / direction.sqrMagnitude;
            return projection;
        }
#if UI_TMP
        public static Vector2 ScreenToLocalByRect(this Vector2 origin, RectComponent rect)
        {
            return rect.World2Local(origin);
        }
#endif
        public static Vector2Int ToInt(this Vector2 origin)
        {
            return new Vector2Int(Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y));
        }

        public static Vector3Int ToInt(this Vector3 origin)
        {
            return new Vector3Int(Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y), Mathf.RoundToInt(origin.z));
        }

        public static Vector2 WithX(this Vector2 vector, float newX)
        {
            return new Vector2(newX, vector.y);
        }

        public static Vector2 WithY(this Vector2 vector, float newY)
        {
            return new Vector2(vector.x, newY);
        }

        public static Vector3 ToX0Y(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static Vector3 ToXZY(this Vector2 v, float Z)
        {
            return new Vector3(v.x, Z, v.y);
        }

        public static Vector3 To0XY(this Vector2 v)
        {
            return new Vector3(0, v.x, v.y);
        }

        public static Vector3 ToZXY(this Vector2 v, float Z)
        {
            return new Vector3(Z, v.x, v.y);
        }

        public static Vector3 ToXY0(this Vector2 v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector3 ToXYZ(this Vector2 v, float Z)
        {
            return new Vector3(v.x, v.y, Z);
        }

        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 YZ(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }

        public static Vector3 HalfWayTo(this Vector3 origin, Vector3 target)
        {
            return origin + (target - origin) * 0.5f;
        }

        public static Quaternion ToQuaternion(this Vector3 v)
        {
            return Quaternion.Euler(v);
        }

    }

}