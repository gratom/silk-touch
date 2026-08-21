using UnityEngine;

namespace Tools
{
    public class SmoothFollower : MonoBehaviour
    {
        public GameObject followThat;
        public AnimationCurve smooth;

        private void Update()
        {
            Following();
        }

        private void Following()
        {
            Vector3 currPos = transform.position;
            currPos = Vector3.Lerp(currPos, followThat.transform.position, Time.deltaTime * smooth.Evaluate(Vector2.Distance(currPos, followThat.transform.position)));
            currPos.z = transform.position.z;
            transform.position = currPos;
        }

        public void SetPosition(Vector2 newPosition)
        {
            Vector3 currPos = newPosition;
            currPos.z = transform.position.z;
            transform.position = currPos;
        }
    }
}