using System.Collections;
using UnityEngine;

namespace Tools
{
    public class Follower : MonoBehaviour
    {
        public GameObject FollowThat;

        private Coroutine followingCoroutineInstance;

        #region public functions

        public void StartFollow()
        {
            if (followingCoroutineInstance == null)
            {
                followingCoroutineInstance = StartCoroutine(FollowingCoroutine());
            }
        }

        public void EndFollow()
        {
            if (followingCoroutineInstance != null)
            {
                StopCoroutine(followingCoroutineInstance);
                followingCoroutineInstance = null;
            }
        }

        #endregion public functions

        #region private functions

        private IEnumerator FollowingCoroutine()
        {
            while (true)
            {
                if (FollowThat != null)
                {
                    transform.position = FollowThat.transform.position;
                }
                else
                {
                    EndFollow();
                }
                yield return null;
            }
        }

        #endregion private functions
    }
}