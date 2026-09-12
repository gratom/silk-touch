using System;
using UnityEngine;

namespace Tools
{
    [ExecuteAlways]
    public class BoundsVisualizer : MonoBehaviour
    {
        private static readonly (Vector3 dir, Color color, string label, Func<Transform, Vector3> getWorldPoint)[] directions =
        {
            (Vector3.up, Color.green, "Top", t => t.TopExtentPivot()),
            (Vector3.down, Color.cyan, "Bottom", t => t.BottomExtentPivot()),
            (Vector3.left, Color.magenta, "Left", t => t.LeftExtentPivot()),
            (Vector3.right, Color.red, "Right", t => t.RightExtentPivot()),
            (Vector3.forward, Color.blue, "Front", t => t.FrontExtentPivot()),
            (Vector3.back, Color.yellow, "Back", t => t.BackExtentPivot())
        };

        private void OnDrawGizmos()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            Bounds bounds = renderer.bounds;
            Vector3 center = transform.position;

            foreach ((Vector3 dir, Color color, string label, Func<Transform, Vector3> f) in directions)
            {
                Vector3 endPoint = f(transform);
                Gizmos.color = color;
                Gizmos.DrawLine(center, endPoint);
                Gizmos.DrawSphere(endPoint, bounds.size.magnitude * 0.01f);

#if UNITY_EDITOR
                UnityEditor.Handles.color = color;
                UnityEditor.Handles.Label(endPoint + dir * bounds.size.magnitude * 0.02f, label);
                UnityEditor.Handles.Label(endPoint.HalfWayTo(transform.position), Vector3.Distance(endPoint, transform.position).ToString("0.0"));
#endif
            }

            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}