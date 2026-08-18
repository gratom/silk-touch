using UnityEngine;

namespace Tools
{
    public static class Pointing

    {
        public static Vector3 MostLikelyPoint(Vector3[] arrayOfPoints, Ray ray, float angleWeight = 0.8f, float distanceWeight = 0.2f)
        {
            return arrayOfPoints[MostLikelyIndex(arrayOfPoints, ray.origin, ray.direction, angleWeight, distanceWeight)];
        }

        public static Vector3 MostLikelyPoint(Vector3[] arrayOfPoints, Vector3 rayOrigin, Vector3 rayDirection, float angleWeight = 0.8f, float distanceWeight = 0.2f)
        {
            return arrayOfPoints[MostLikelyIndex(arrayOfPoints, rayOrigin, rayDirection, angleWeight, distanceWeight)];
        }

        public static int MostLikelyIndex(Vector3[] arrayOfPoints, Ray ray, float angleWeight = 0.8f, float distanceWeight = 0.2f)
        {
            return MostLikelyIndex(arrayOfPoints, ray.origin, ray.direction, angleWeight, distanceWeight);
        }

        public static int MostLikelyIndex(Vector3[] arrayOfPoints, Vector3 rayOrigin, Vector3 rayDirection, float angleWeight = 0.8f, float distanceWeight = 0.2f)
        {
            float bestValue = float.MaxValue;
            int bestValueIndex = 0;
            for (int i = 0; i < arrayOfPoints.Length; i++)
            {
                float currentAngle = Vector3.Angle(rayDirection, arrayOfPoints[i] - rayOrigin);
                float currentDistance = Vector3.Distance(arrayOfPoints[i], rayOrigin);
                float currentValue = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * angleWeight * currentDistance + currentDistance * distanceWeight;
                if (currentValue < bestValue)
                {
                    bestValue = currentValue;
                    bestValueIndex = i;
                }
            }
            return bestValueIndex;
        }
    }
}