using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class Dijkstra
    {
        public static List<int> GetWay(List<Dot> cities, int startIndex, int endIndex)
        {
            float[] dist = new float[cities.Count];
            int[] parent = new int[cities.Count];
            bool[] processed = new bool[cities.Count];
            for (int i = 0; i < dist.Length; ++i)
            {
                dist[i] = i == startIndex ? 0 : float.MaxValue;
                parent[i] = -1;
            }

            int currentVertex = startIndex;
            while (processed[endIndex] == false)
            {
                float curMin = float.MaxValue;
                for (int i = 0; i < dist.Length; ++i)
                {
                    if (curMin > dist[i] && processed[i] == false)
                    {
                        curMin = dist[i];
                        currentVertex = i;
                    }
                }

                processed[currentVertex] = true;

                for (int i = 0; i < cities[currentVertex].Roads.Count; ++i)
                {
                    int nextVert = cities[currentVertex].Roads[i];

                    if (processed[nextVert])
                    {
                        continue;
                    }

                    if (dist[nextVert] > dist[currentVertex] + (cities[nextVert].Pos - cities[currentVertex].Pos).magnitude)
                    {
                        dist[nextVert] = dist[currentVertex] + (cities[nextVert].Pos - cities[currentVertex].Pos).magnitude;
                        parent[nextVert] = currentVertex;
                    }
                }
            }

            List<int> way = new List<int>();
            currentVertex = endIndex;

            while (currentVertex != -1)
            {
                way.Add(currentVertex);
                currentVertex = parent[currentVertex];
            }

            way.Reverse();

            return way;
        }
    }

    public class Dot
    {
        public Vector2 Pos;
        public List<int> Roads;
    }
}