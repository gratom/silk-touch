using UnityEngine;

namespace Tools
{
    public class Follower : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        public bool isFollowing;
        
        private void Update()
        {
            if (!isFollowing)
            {
                return;
            }

            if (followTarget != null)
            {
                transform.position = followTarget.position;
                transform.rotation = followTarget.rotation;
            }
            else
            {
                isFollowing = false;
            }
        }
    }
}